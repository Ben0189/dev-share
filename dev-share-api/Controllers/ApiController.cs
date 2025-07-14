using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Playwright;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Services;
using Qdrant.Client.Grpc;
using System.Text;
using Executor;
using System.Collections.Concurrent;


namespace UrlExtractorApi.Controllers;

[ApiController]
[Route("api")]
public class ExtractController : ControllerBase
{
    private readonly ISummaryService _summaryService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorService _vectorService;
    private readonly ShareChainExecutor _shareChainExecutor;
    private readonly IUserInsightService _userInsightService;
    private readonly IResourceService _resourceService;
    private static readonly ConcurrentDictionary<string, ShareTask> TaskStore = new();

    public ExtractController(
        ISummaryService summaryService,
        IEmbeddingService embeddingService,
        IVectorService vectorService,
        IUserInsightService _userInsightService;
        IResourceService _resourceService;
        ShareChainExecutor shareChainExecutor)
    {
        _summaryService = summaryService;
        _embeddingService = embeddingService;
        _vectorService = vectorService;
        _shareChainExecutor = shareChainExecutor;
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
                try
                {
                Console.WriteLine($"Extracting: {url}");
                var result = TryHtmlAgilityPack(url);
                if (string.IsNullOrWhiteSpace(result))
                    result = await TryPlaywright(url);
                if (string.IsNullOrWhiteSpace(result))
                    throw new Exception("Content extraction failed.");

                var prompt = new StringBuilder()
                    .AppendLine("You will receive an input text and your task is to summarize the article in no more than 100 words.")
                    .AppendLine("Only return the summary. Do not include any explanation.")
                    .AppendLine("# Article content:")
                    .AppendLine($"{result}")
                    .ToString();

                await _shareChainExecutor.ExecuteAsync(new ResourceShareContext
                {
                    Url = url,
                    Prompt = prompt
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
    public async Task<List<ResourceDTO>> Search([FromBody] SearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Search text cannot be empty.");
        }
        if (request.TopRelatives <= 0 || request.TopRelatives > 100)
            return BadRequest("TopRelatives must be between 1 and 100.");

        try
        {
            //1.gen prompt embedding
            var denseEmbedding = await _embeddingService.GetDenseEmbeddingAsync(request.Text);
            var (indices, values) = await _embeddingService.GetSparseEmbeddingAsync(request.Text);
                       
            //2.search prompt and get result(content and comment)
            var ContentResults = await _vectorService.SearchEmbeddingAsync(denseQueryVector: denseEmbedding, sparseIndices: indices, sparseValues: values, topK: request.TopRelatives);
            //todo
            var InsightResults;

            //3. do rerank and get reranked list
            var rerankResults = GetRerankedList(ContentResults,InsightResults);

            //4. get　finalResults from sql server by id
            var results = new List<ResourceDTO>();
            foreach(var item in reranResults)
            {
                var contentId = item.ContentId;
                var resource = await _resourceService.GetResourceById(contentId);
                if(resource != null){
                    results.Add(resource);
                }
            }
            return Ok(results);
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

    [HttpPost("embedding/generate")]
    public async Task<ActionResult> GenerateEmbedding([FromBody] GenerateEmbeddingRequest request)
    {
        var denseEmbedding = await _embeddingService.GetDenseEmbeddingAsync(request.Text);
        var sparseEmbedding = await _embeddingService.GetSparseEmbeddingAsync(request.Text);

        var denseVector = new DenseVector();
        denseVector.Data.AddRange(denseEmbedding);

        var sparseVector = new SparseVector();
        sparseVector.Indices.AddRange(sparseEmbedding.indices); // Item2 = indices
        sparseVector.Values.AddRange(sparseEmbedding.values);  // Item1 = values

        var vectors = new Dictionary<string, Vector>
        {
            ["dense_vector"] = new() { Dense = denseVector },
            ["sparse_vector"] = new() { Sparse = sparseVector }
        };

        return Ok(vectors);
    }

    [HttpPut("embedding/put")]
    public async Task<ActionResult<UpdateResult>> InsertEmbedding([FromBody] InsertEmbeddingRequest request)
    {
        return Ok(await _vectorService.UpsertEmbeddingAsync(request.Url, request.NoteId, request.Text, request.Vectors));
    }

    // [HttpPut("embedding/put")]
    // public async Task<ActionResult<UpdateResult>> InsertEmbedding([FromBody] InsertEmbeddingRequest request)
    // {
    //     return Ok(await _vectorService.UpsertEmbeddingAsync(request.Url, request.NoteId, request.Text, request.Vectors));
    // }

    [HttpPost("embedding/indexing")]
    public async Task<ActionResult<UpdateResult>> Indexing([FromBody] string field)
    {
        return Ok(await _vectorService.IndexingAsync(field));
    }

    private string? TryHtmlAgilityPack(string url)
    {
        try
        {
            var web = new HtmlWeb
            {
                // 设置 User-Agent，防止部分网站屏蔽爬虫
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                            "AppleWebKit/537.36 (KHTML, like Gecko) " +
                            "Chrome/120.0.0.0 Safari/537.36"
            };
            var doc = web.Load(url);

            //TODO 编码问题
            // using var client = new HttpClient();
            //         var bytes = client.GetByteArrayAsync(url).Result;
            //         var html = System.Text.Encoding.UTF8.GetString(bytes);
            //         var doc = new HtmlDocument();
            //         doc.LoadHtml(html);


            // 提取网页标题
            var titleNode = doc.DocumentNode.SelectSingleNode("//title");
            Console.WriteLine("Title: " + titleNode?.InnerText);

            // 提取所有段落文本
            var paragraphs = doc.DocumentNode.SelectNodes("//p");
            if (paragraphs == null) return null;

            var title = titleNode?.InnerText.Trim() ?? "";
            var paragraphText = string.Join("\n", paragraphs
                .Select(p => p.InnerText.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t)));

            return title + "\n\n" + paragraphText;
        }
        catch
        {
            return null;
        }
    }

    // 使用 Playwright 模拟浏览器加载网页并提取段落内容（用于 CSR 页面）
    private async Task<string> TryPlaywright(string url)
    {
        // 启动 Playwright 浏览器（无头模式）
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

        // 打开新页面并导航到目标地址，等待网络空闲（页面渲染完成）
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // 提取所有 <p> 元素的 innerText，去除空行
        var text = await page.EvalOnSelectorAllAsync<string[]>("p", "els => els.map(e => e.innerText).filter(t => t.trim().length > 0)");
        return string.Join("\n", text);
    }
    
    //todo make sure the return data from service is List<Content> and List<Comment>
    public List<Rerank> GetRerankedList(List<Content> contents, List<Comment> comments)
    {
        // averge comment.score
        var commentGroups = comments
            .GroupBy(c => c.ContentId)
            .ToDictionary(
                g => g.Key,
                g => g.Average(c => c.Score)
            );

        // content.score find table
        var contentScores = contents
            .ToDictionary(c => c.Id, c => c.Score);

        // union all contentId
        var allContentIds = contentScores.Keys
            .Union(commentGroups.Keys)
            .Distinct();

        var result = allContentIds
            .Select(id => new Rerank
            {
                ContentId = id,
                Score = 
                    (contentScores.TryGetValue(id, out var cScore) ? cScore : 0) * 0.7 +
                    (commentGroups.TryGetValue(id, out var comAvg) ? comAvg : 0) * 0.3
            })
            .OrderByDescending(r => r.Score)
            .ToList();

        return result;
    }
}