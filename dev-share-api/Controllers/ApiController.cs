using HtmlAgilityPack;
using Microsoft.Playwright;
using Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using Qdrant.Client.Grpc;
using System.Text;
using Executor;
using System.Collections.Concurrent;
using System.Text.Json;
using Newtonsoft.Json.Linq;


namespace Controllers;

[ApiController]
[Route("api")]
public class ExtractController : ControllerBase
{

    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorService _vectorService;
    private readonly IResourceService _resourceService;
    private readonly IOnlineResearchService _onlineResearchService;
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly ConcurrentDictionary<string, ShareTask> TaskStore = new();

    public ExtractController(
        IEmbeddingService embeddingService,
        IVectorService vectorService,
        IOnlineResearchService onlineResearchService,
        IServiceScopeFactory scopeFactory,
        IResourceService resourceService)
    {
        _embeddingService = embeddingService;
        _vectorService = vectorService;
        _onlineResearchService = onlineResearchService;
        _scopeFactory = scopeFactory;
        _resourceService = resourceService;
    }

    [HttpPost("share")]
    public async Task<IActionResult> Share([FromBody] UrlRequest request)
    {
        var url = request.Url;

        //URL check
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest("URL is required.");
        }
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
        (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            return BadRequest("URL must start with http:// or https:// and be a valid absolute URL.");
        }

        Console.WriteLine($"Extracting: {url}");

        var taskId = Guid.NewGuid().ToString();
        var task = new ShareTask
        {
            TaskId = taskId,
            Url = url,
            Status = "pending"
        };
        TaskStore[taskId] = task;
        Console.WriteLine($"[POST] Saving task: {taskId}");
        Console.WriteLine($"[POST] TaskStore count: {TaskStore.Count}");

        _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var executor = scope.ServiceProvider.GetRequiredService<ShareChainExecutor>();
                try
                {
                    await executor.ExecuteAsync(new ResourceShareContext
                    {
                        Url = url,
                        Insight = request.Insight
                    });

                    task.Status = "success";
                    task.Message = "Processed successfully";
                }
                catch (Exception ex)
                {
                    task.Status = "failed";
                    task.Message = ex.Message;
                }
            });
        return Ok(new { taskId });
    }

    [HttpGet("share/status/{taskId}")]
    public IActionResult GetStatus(string taskId)
    {
        Console.WriteLine($"[GET] Checking task: {taskId}");
        Console.WriteLine($"[GET] TaskStore count: {TaskStore.Count}");
        if (!TaskStore.TryGetValue(taskId, out var task))
            return NotFound(new { message = "Task not found" });

        return Ok(new
        {
            taskId = task.TaskId,
            status = task.Status,
            message = task.Message
        });
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { message = "Search text cannot be empty." });
        }
        if (request.TopRelatives <= 0 || request.TopRelatives > 100)
            return BadRequest("TopRelatives must be between 1 and 100.");

        try
        {
            //get vectordb data results
            var resourceResults = await _vectorService.SearchResourceAsync(
                query: request.Text,
                topK: request.TopRelatives);

            var insightResults = await _vectorService.SearchInsightAsync(
                query: request.Text,
                topK: request.TopRelatives);

            if (resourceResults == null
                || resourceResults.Count == 0
                || insightResults == null
                || insightResults.Count == 0)
            {
                // Fallback to online research
                var onlineResult = await _onlineResearchService.PerformOnlineResearchAsync(request.Text, request.TopRelatives);
                return Ok(new { source = "online", result = onlineResult.ToList() });
            }
            else
            {
                //2. do rerank and get reranked list
                var rerankResults = GetRerankedList(resourceResults, insightResults);

                //3. get　finalResults from sql server by id
                var results = new List<ResourceDto>();
                foreach (var item in rerankResults)
                {
                    var resourceId = item.ResourceId;
                    var resource = await _resourceService.GetResourceById(long.Parse(resourceId));
                    if (resource != null)
                    {
                        results.Add(resource);
                    }
                }
                return Ok(new { source = "vector", result = results });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Search failed due to an internal error.");
        }
    }


    [HttpPost("vector/init")]
    public async Task<ActionResult<float[]>> InitVectorDB()
    {
        await _vectorService.InitializeAsync();
        return Ok();
    }

    [HttpPost("embedding/indexing")]
    public async Task<ActionResult<UpdateResult>> Indexing([FromBody] string collectionName, string field)
    {
        return Ok(await _vectorService.IndexingAsync(collectionName, field));
    }

    [HttpPost("insight/share")]
    public async Task<IActionResult> ShareInsight([FromBody] ShareInsightRequest request)
    {
        var insightId = request.InsightId ?? Guid.NewGuid().ToString();
        var denseEmbedding = await _embeddingService.GetDenseEmbeddingAsync(request.Content);
        var (indices, values) = await _embeddingService.GetSparseEmbeddingAsync(request.Content);

        var denseVector = new DenseVector();
        denseVector.Data.AddRange(denseEmbedding);

        var sparseVector = new SparseVector();
        sparseVector.Indices.AddRange(indices);
        sparseVector.Values.AddRange(values);

        var vectors = new Dictionary<string, Vector>
        {
            ["dense_vector"] = new() { Dense = denseVector },
            ["sparse_vector"] = new() { Sparse = sparseVector }
        };

        request.Vectors = vectors;
        await _vectorService.UpsertInsightAsync(insightId, request.Url, request.Content, request.ResourceId, request.Vectors);
        return Ok();
    }

    //todo make sure the return data from service is List<Resource> and List<Insight>
    private static List<Rerank> GetRerankedList(List<VectorResourceDto> resources, List<VectorInsightDto> insights)
    {
        // averge comment.score
        var insightGroups = insights
            .GroupBy(c => c.ResourceId)
            .ToDictionary(
                g => g.Key,
                g => g.Average(c => c.Score)
            );

        // content.score find table
        var resourceScores = resources
            .ToDictionary(c => c.Id, c => c.Score);

        // union all contentId
        var allResourceIds = resourceScores.Keys
            .Union(insightGroups.Keys)
            .Distinct();

        var result = allResourceIds
            .Select(id => new Rerank
            {
                ResourceId = id,
                Score =
                    (resourceScores.TryGetValue(id, out var rScore) ? rScore : 0) * 0.7 +
                    (insightGroups.TryGetValue(id, out var iAvg) ? iAvg : 0) * 0.3
            })
            .OrderByDescending(r => r.Score)
            .ToList();

        return result;
    }
}