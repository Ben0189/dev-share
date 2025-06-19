namespace Services;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
    Task<float[]> GetDenseEmbeddingAsync(string input);
    Task<(uint[] indices, float[] values)> GetSparseEmbeddingAsync(string input);
}