using DevShare.Api.Configuration;
using Microsoft.Extensions.Options;
using Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Text.Json;

namespace Services;

public interface IVectorService
{
    Task InitializeAsync();
    Task<UpdateResult> IndexingAsync(string collectionName, string fieldName);
    Task UpdateCollectionAsync(string collectionName);

    Task UpsertResourceAsync(string id, string url, string Content, Dictionary<string, Vector> vectors);
    Task UpsertInsightAsync(string id, string url, string Content, string resourceId, Dictionary<string, Vector> vectors);
    Task<List<VectorResourceDto>> SearchResourceAsync(string query, int topK);
    Task<List<VectorInsightDto>> SearchInsightAsync(string query, int topK);
}

public class VectorService : IVectorService
{
    private readonly QdrantClient _client;
    private readonly IEmbeddingService _embeddingService;
    private readonly VectorDbSettings _settings;
    private readonly ILogger<VectorService> _logger;

    public VectorService(
        QdrantClient qdrantClient,
        IEmbeddingService embeddingService,
        IOptions<VectorDbSettings> settings,
        ILogger<VectorService> logger)
    {
        _client = qdrantClient ?? throw new ArgumentNullException(nameof(qdrantClient));
        _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // init if there is no collection in vector db
    public async Task InitializeAsync()
    {
        await CreateCollectionAsync(_settings.ResourceCollection);
        await CreateCollectionAsync(_settings.InsightCollection);
    }

    public async Task<UpdateResult> IndexingAsync(string collectionName, string fieldName)
    {
        return await _client.CreatePayloadIndexAsync(
            collectionName: collectionName,
            fieldName: fieldName,
            schemaType: PayloadSchemaType.Text,
            indexParams: new PayloadIndexParams
            {
                TextIndexParams = new TextIndexParams
                {
                    Tokenizer = TokenizerType.Word,
                    MinTokenLen = _settings.Sparse.MinTokenLength,
                    MaxTokenLen = _settings.Sparse.MaxTokenLength,
                    Lowercase = true
                }
            }
        );
    }

    public async Task UpdateCollectionAsync(string collectionName)
    {
        await _client.UpdateCollectionAsync(
            collectionName: collectionName,
            sparseVectorsConfig: CreateSparseVectorConfig()
        );
    }

    public async Task UpsertResourceAsync(string id, string url, string content, Dictionary<string, Vector> vectors)
    {
        var point = new PointStruct
        {
            Id = ulong.Parse(id),
            Vectors = vectors,
            Payload = {
                ["url"] = url,
                ["content"] = content
            }
        };
        await _client.UpsertAsync(_settings.ResourceCollection, new List<PointStruct> { point });
    }

    public async Task UpsertInsightAsync(string id, string url, string content, string resourceId, Dictionary<string, Vector> vectors)
    {
        var point = new PointStruct
        {
            Id = ulong.Parse(id),
            Vectors = vectors,
            Payload = {
                ["url"] = url,
                ["content"] = content,
                ["resourceId"] = resourceId
            }
        };
        await _client.UpsertAsync(_settings.InsightCollection, new List<PointStruct> { point });
    }

    public async Task<List<VectorResourceDto>> SearchResourceAsync(string query, int topK)
    {
        var (denseVector, sparseVector) = await GetQueryVectorsAsync(query);
        var prefetchQueries = CreatePrefetchQueries(denseVector, sparseVector, topK);

        var results = await _client.QueryAsync(
            collectionName: _settings.ResourceCollection,
            prefetch: prefetchQueries,
            query: Fusion.Rrf,
            limit: (ulong)topK,
            payloadSelector: true,
            vectorsSelector: false
        );

        return results.Select(MapToResourceDto).ToList();
    }

    public async Task<List<VectorInsightDto>> SearchInsightAsync(string query, int topK)
    {
        var (denseVector, sparseVector) = await GetQueryVectorsAsync(query);
        var prefetchQueries = CreatePrefetchQueries(denseVector, sparseVector, topK);

        var insightResults = await _client.QueryAsync(
            collectionName: _settings.InsightCollection,
            prefetch: prefetchQueries,
            query: Fusion.Rrf,
            limit: (ulong)topK,
            payloadSelector: true,
            vectorsSelector: false
        );

        return insightResults.Select(MapToInsightDto).ToList();
    }

    private async Task CreateCollectionAsync(string collectionName)
    {
        try
        {
            await _client.CreateCollectionAsync(
                collectionName,
                vectorsConfig: CreateVectorConfig(),
                sparseVectorsConfig: CreateSparseVectorConfig()
            );

            await IndexingAsync(collectionName, "content");
            _logger.LogInformation("Successfully created collection: {CollectionName}", collectionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create collection: {CollectionName}", collectionName);
            throw;
        }
    }

    private VectorParamsMap CreateVectorConfig() => new()
    {
        Map =
        {
            [_settings.Dense.Name] = new VectorParams
            {
                Size = _settings.Dimensions,
                Distance = Distance.Cosine
            }
        }
    };

    private (string name, SparseVectorParams config) CreateSparseVectorConfig() =>
    (
        _settings.Sparse.Name,
        new SparseVectorParams
        {
            Modifier = Modifier.Idf,
            Index = new SparseIndexConfig
            {
                OnDisk = _settings.Sparse.OnDisk
            }
        }
    );

    private async Task<(float[] dense, (uint[] indices, float[] values) sparse)>
        GetQueryVectorsAsync(string query)
    {
        var denseTask = _embeddingService.GetDenseEmbeddingAsync(query);
        var sparseTask = _embeddingService.GetSparseEmbeddingAsync(query);

        await Task.WhenAll(denseTask, sparseTask);
        return (await denseTask, await sparseTask);
    }

    private List<PrefetchQuery> CreatePrefetchQueries(
        float[] denseVector,
        (uint[] indices, float[] values) sparseVector,
        int topK)
    {
        var sparseTuples = sparseVector.values
            .Select((val, i) => (val, sparseVector.indices[i]))
            .ToArray();

        return new List<PrefetchQuery>
        {
            new()
            {
                Query = sparseTuples,
                Using = _settings.Sparse.Name,
                Limit = (ulong)topK,
                ScoreThreshold = _settings.SparseScoreThreshold
            },
            new()
            {
                Query = denseVector,
                Using = _settings.Dense.Name,
                Limit = (ulong)topK,
                ScoreThreshold = _settings.DenseScoreThreshold
            }
        };
    }

    private static VectorResourceDto MapToResourceDto(ScoredPoint result) => new()
    {
        Id = result.Id.Num.ToString(),
        Url = GetPayloadValue(result.Payload, "url"),
        Content = GetPayloadValue(result.Payload, "content"),
        Score = result.Score
    };

    private static string GetPayloadValue(IDictionary<string, Value> payload, string key) =>
        payload.TryGetValue(key, out var val) && val.KindCase == Value.KindOneofCase.StringValue
            ? val.StringValue
            : string.Empty;

    private static VectorInsightDto MapToInsightDto(ScoredPoint result) => new()
    {
        Id = result.Id.Num.ToString(),
        Url = GetPayloadValue(result.Payload, "url"),
        Content = GetPayloadValue(result.Payload, "content"),
        ResourceId = GetPayloadValue(result.Payload, "resourceId"),
        Score = result.Score
    };
}