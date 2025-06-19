using System.Net.Http;
using System.Text;
using System.Text.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly OpenAIClient _client;
    private readonly HttpClient _httpClient;

    private const string embeddingModelId = "text-embedding-3-small";
    private const int _dimensions = 4;

    public EmbeddingService(OpenAIClient openAIClient, IHttpClientFactory httpClientFactory)
    {
        _client = openAIClient;
        _httpClient = httpClientFactory.CreateClient("FastEmbed"); ;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        //https://api-inference.huggingface.co/pipeline/feature-extraction/
        //https://router.huggingface.co/hf-inference/models/intfloat/e5-small-v2/pipeline/sentence-similarity
        // var resp = await _client.PostAsJsonAsync($"/pipeline/feature-extraction/{Model}", new { inputs = text });
        // resp.EnsureSuccessStatusCode();
        //var raw = await resp.Content.ReadFromJsonAsync<float[][]>();
        EmbeddingGenerationOptions options = new() { Dimensions = _dimensions };
        OpenAIEmbedding resp = await _client.GetEmbeddingClient(model: embeddingModelId).GenerateEmbeddingAsync(text, options);
        var vector = resp.ToFloats();

        return vector.ToArray();
    }

    public async Task<float[]> GetDenseEmbeddingAsync(string input)
    {
        var payload = new { texts = new[] { input } };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/embed/dense", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<EmbeddingResponse>(responseString);

        if (result?.Embeddings == null || result.Embeddings.Count == 0)
        {
            throw new Exception("No embeddings returned. Raw response: " + responseString);
        }
        return result.Embeddings[0];
    }

    public async Task<(uint[] indices, float[] values)> GetSparseEmbeddingAsync(string input)
    {
        var payload = new { texts = new[] { input } };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/embed/sparse", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<SparseEmbeddingResponse>(responseString);
        if (result?.Embeddings == null || result.Embeddings.Count == 0)
        {
            throw new Exception("No embeddings returned. Raw response: " + responseString);
        }
        var embedding = result.Embeddings[0];
        return (embedding.Indices, embedding.Values);
    }

    public class EmbeddingResponse
    {

        [JsonProperty("embeddings")]
        public List<float[]>? Embeddings { get; set; }
    }

    public class SparseEmbeddingResponse
    {
        public List<SparseEmbedding>? Embeddings { get; set; }
    }

    public class SparseEmbedding
    {
        public uint[]? Indices { get; set; }
        public float[]? Values { get; set; }
    }
}
