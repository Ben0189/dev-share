namespace Services;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
}