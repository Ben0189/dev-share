using Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;

namespace Services;

public interface IVectorService
{
    Task InitializeAsync();
    Task<UpdateResult> IndexingAsync(string collectionName, string fieldName);
    Task UpsertResourceAsync(string id, string url, string Content, Dictionary<string, Vector> vectors);
    Task UpsertInsightAsync(string id, string url, string Content, string resourceId, Dictionary<string, Vector> vectors);
    Task<List<VectorResourceDto>> SearchResourceAsync(string query, int topK);
    Task<List<VectorInsightDto>> SearchInsightAsync(string query, int topK);
}

public class VectorService : IVectorService
{
    private readonly QdrantClient _client;
    private readonly string _resourceCollection = "BlotzShare_Resource";
    private readonly string _insightCollection = "BlotzShare_Insight";
    private readonly ulong _dimensions = 384;
    private readonly IEmbeddingService _embeddingService;

    public VectorService(QdrantClient qdrantClient, IEmbeddingService embeddingService)
    {
        _client = qdrantClient;
        _embeddingService = embeddingService;
    }

    // init if there is no collection in vector db
    public async Task InitializeAsync()
    {
        // Create resource collection
        await _client.CreateCollectionAsync(
            _resourceCollection,
            vectorsConfig: new VectorParamsMap
            {
                Map =
                {
                    ["dense_vector"] = new VectorParams { Size = _dimensions, Distance = Distance.Cosine },
                }
            },
            sparseVectorsConfig:
            (
                "sparse_vector",
                new SparseVectorParams
                {
                    Index = new SparseIndexConfig
                    {
                        OnDisk = false,
                    }
                }
            )
        );

        await IndexingAsync(_resourceCollection, "content");

        // Create insight collection
        await _client.CreateCollectionAsync(
            _insightCollection,
            vectorsConfig: new VectorParamsMap
            {
                Map =
                {
                    ["dense_vector"] = new VectorParams { Size = _dimensions, Distance = Distance.Cosine },
                }
            },
            sparseVectorsConfig:
            (
                "sparse_vector",
                new SparseVectorParams
                {
                    Index = new SparseIndexConfig
                    {
                        OnDisk = false,
                    }
                }
            )
        );

        await IndexingAsync(_insightCollection, "content");
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
                    MinTokenLen = 2,
                    MaxTokenLen = 10,
                    Lowercase = true
                }
            }
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
        await _client.UpsertAsync(_resourceCollection, new List<PointStruct> { point });
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
        await _client.UpsertAsync(_insightCollection, new List<PointStruct> { point });
    }

    public async Task<List<VectorResourceDto>> SearchResourceAsync(string query, int topK)
    {
        // Hybrid search on resource collection
        var denseQueryVector = await _embeddingService.GetDenseEmbeddingAsync(query);
        var (sparseIndices, sparseValues) = await _embeddingService.GetSparseEmbeddingAsync(query);

        var sparseTupleArray = sparseValues.Select((val, i) => (val, sparseIndices[i])).ToArray();
        var prefetch = new List<PrefetchQuery>
        {
            new() { Query = sparseTupleArray, Using = "sparse_vector", Limit = (ulong)topK },
            new() { Query = denseQueryVector, Using = "dense_vector", Limit = (ulong)topK }
        };


        var resourceResults = await _client.QueryAsync(
            collectionName: _resourceCollection,
            prefetch: prefetch,
            query: Fusion.Rrf,
            limit: (ulong)topK,
            scoreThreshold: (float)0.7, //todo: make this dynamic
            payloadSelector: true,
            vectorsSelector: false
        );

        return resourceResults.Select(result =>
        {
            var payload = result.Payload;
            return new VectorResourceDto
            {
                Id = result.Id.ToString(),
                Url = payload.TryGetValue("url", out var urlVal) && urlVal.KindCase == Value.KindOneofCase.StringValue ? urlVal.StringValue : string.Empty,
                Content = payload.TryGetValue("content", out var contentVal) && contentVal.KindCase == Value.KindOneofCase.StringValue ? contentVal.StringValue : string.Empty,
                Score = result.Score
            };
        }).ToList();
    }

    public async Task<List<VectorInsightDto>> SearchInsightAsync(string query, int topK)
    {
        var denseQueryVector = await _embeddingService.GetDenseEmbeddingAsync(query);
        var (sparseIndices, sparseValues) = await _embeddingService.GetSparseEmbeddingAsync(query);

        var sparseTupleArray = sparseValues.Select((val, i) => (val, sparseIndices[i])).ToArray();
        var prefetch = new List<PrefetchQuery>
        {
            new() { Query = sparseTupleArray, Using = "sparse_vector", Limit = (ulong)topK },
            new() { Query = denseQueryVector, Using = "dense_vector", Limit = (ulong)topK }
        };


        var insightResults = await _client.QueryAsync(
            collectionName: _resourceCollection,
            prefetch: prefetch,
            query: Fusion.Rrf,
            limit: (ulong)topK,
            scoreThreshold: (float)0.7, //todo: make this dynamic
            payloadSelector: true,
            vectorsSelector: false
        );

        return insightResults.Select(result =>
        {
            var payload = result.Payload;
            return new VectorInsightDto
            {
                Id = result.Id.ToString(),
                Url = payload.TryGetValue("url", out var urlVal) && urlVal.KindCase == Value.KindOneofCase.StringValue ? urlVal.StringValue : string.Empty,
                Content = payload.TryGetValue("content", out var contentVal) && contentVal.KindCase == Value.KindOneofCase.StringValue ? contentVal.StringValue : string.Empty,
                ResourceId = payload.TryGetValue("resourceId", out var resourceIdVal) && resourceIdVal.KindCase == Value.KindOneofCase.StringValue ? resourceIdVal.StringValue : string.Empty,
                Score = result.Score
            };
        }).ToList();
    }
}