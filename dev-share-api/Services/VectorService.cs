using Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;

namespace Services;

public class VectorService : IVectorService
{
    private readonly QdrantClient _client;
    private readonly string _collection = "blotz-dev";
    private readonly ulong _dimensions = 384;

    public VectorService(QdrantClient qdrantClient)
    {
        _client = qdrantClient;
    }

    // init if there is no collection in vector db
    public async Task InitializeAsync()
    {
        await _client.CreateCollectionAsync(
            _collection,
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
    }

    public async Task<UpdateResult> IndexingAsync(string fieldName)
    {
        return await _client.CreatePayloadIndexAsync(
            collectionName: _collection,
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

    public async Task<UpdateResult> UpsertEmbeddingAsync(string url, string noteId, string content, Dictionary<string, Vector> vector)
    {
        // Convert string ID to ulong, or use incremental numeric IDs
        var point = new PointStruct
        {
            Id = ulong.Parse(noteId),
            Vectors = vector,
            Payload = {
                ["url"] = url,
                ["content"] = content
            }
        };
        return await _client.UpsertAsync(_collection, new List<PointStruct> { point });
    }

    public async Task<List<VectorSearchResultDto>> SearchEmbeddingAsync(float[] denseQueryVector, uint[] sparseIndices, float[] sparseValues, int topK)
    {
        // var filter = new Filter(
        //         MatchText("content", queryText)
        //     );

        if (sparseIndices.Length != sparseValues.Length)
        {
            throw new ArgumentException("sparse indices and sparse values must be same length");
        }

        var sparseTupleArray = sparseValues
            .Select((val, i) => (val, sparseIndices[i]))
            .ToArray();

        var prefetch = new List<PrefetchQuery>
        {
            new()
            {
                Query = sparseTupleArray,
                Using = "sparse_vector",
                Limit = (ulong)topK
            },
            new()
            {
                Query = denseQueryVector,
                Using = "dense_vector",
                Limit = (ulong)topK
            }
        };

        var results = await _client.QueryAsync(
            collectionName: _collection,
            prefetch: prefetch,
            query: Fusion.Rrf,
            limit: (ulong)topK
        );

        return results.Select(result =>
        {
            var payload = result.Payload;
            return new VectorSearchResultDto
            {
                Url = payload.TryGetValue("url", out var urlVal) && urlVal.KindCase == Value.KindOneofCase.StringValue
                    ? urlVal.StringValue
                    : string.Empty,
                Content = payload.TryGetValue("content", out var contentVal) && contentVal.KindCase == Value.KindOneofCase.StringValue
                    ? contentVal.StringValue
                    : string.Empty
            };
        }).ToList();
    }
}