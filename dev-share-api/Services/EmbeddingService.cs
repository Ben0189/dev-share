using System.Text;
using OpenAI;
using OpenAI.Embeddings;
using Newtonsoft.Json;
using Models;

namespace Services;

public interface IEmbeddingService
{
    Task<float[]> GetDenseEmbeddingAsync(string input);
    Task<(uint[] indices, float[] values)> GetSparseEmbeddingAsync(string input);
}

public class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;

    private const string embeddingModelId = "text-embedding-3-small";
    private const int _dimensions = 4;

    public EmbeddingService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("FastEmbed"); ;
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
}
