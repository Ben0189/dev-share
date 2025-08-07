public static class UrlTypeDetector
{
    private static readonly string[] VideoExtensions = { ".mp4", ".webm", ".m3u8", ".mov", ".avi" };
    private static readonly string[] VideoDomains = { "youtube.com", "youtu.be", "vimeo.com", "dailymotion.com" };

    public static bool IsLikelyVideoFromPattern(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        var host = uri.Host.ToLowerInvariant();
        if (VideoDomains.Any(domain => host.Contains(domain)))
            return true;

        var path = uri.AbsolutePath.ToLowerInvariant();
        if (VideoExtensions.Any(ext => path.EndsWith(ext)))
            return true;

        return false;
    }

    public static async Task<bool> IsVideoByContentTypeAsync(string url, HttpClient? client = null)
    {
        client ??= new HttpClient();

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0");

            using var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return false;

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            return mediaType?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) == true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> IsVideoUrlAsync(string url, HttpClient? client = null)
    {
        return IsLikelyVideoFromPattern(url) || await IsVideoByContentTypeAsync(url, client);
    }
}
