using System.Text.RegularExpressions;
using System.Web;

public class UrlManageUtil
{
    private static readonly HashSet<string> DefaultPorts = new HashSet<string>
    {
        "http:80", "https:443", "ftp:21"
    };
    
    public static string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty");
        
        try
        {
            var uri = new Uri(url);
            
            // 1. Normalize scheme and host
            var scheme = uri.Scheme.ToLowerInvariant();
            var host = uri.Host.ToLowerInvariant();
            
            // 2. Handle port
            var port = "";
            if (!uri.IsDefaultPort)
            {
                var schemePort = $"{scheme}:{uri.Port}";
                if (!DefaultPorts.Contains(schemePort))
                {
                    port = $":{uri.Port}";
                }
            }
            
            // 3. Normalize path
            var path = NormalizePath(uri.AbsolutePath);
            
            // 4. Normalize query string
            var query = NormalizeQuery(uri.Query);
            
            // 5. Rebuild the URL
            var normalizedUrl = $"{scheme}://{host}{port}{path}{query}";
            
            return normalizedUrl;
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException($"Invalid URL format: {url}", ex);
        }
    }
    
    private static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "/")
            return "/";
        
        // URL decode
        path = HttpUtility.UrlDecode(path);
        
        // Remove redundant slashes
        path = Regex.Replace(path, @"/+", "/");
        
        // Remove trailing slash (except for root path)
        if (path.Length > 1 && path.EndsWith("/"))
            path = path.TrimEnd('/');
        
        // Resolve relative path segments (/../ and /./)
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var normalizedSegments = new List<string>();
        
        foreach (var segment in segments)
        {
            if (segment == ".")
                continue;
            
            if (segment == "..")
            {
                if (normalizedSegments.Count > 0)
                    normalizedSegments.RemoveAt(normalizedSegments.Count - 1);
            }
            else
            {
                normalizedSegments.Add(segment);
            }
        }
        
        return "/" + string.Join("/", normalizedSegments);
    }
    
    private static string NormalizeQuery(string query)
    {
        if (string.IsNullOrEmpty(query))
            return "";
        
        // Remove leading '?'
        if (query.StartsWith("?"))
            query = query.Substring(1);
        
        if (string.IsNullOrEmpty(query))
            return "";
        
        // Parse query parameters
        var parameters = new List<KeyValuePair<string, string>>();
        var pairs = query.Split('&');
        
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=', 2);
            var key = HttpUtility.UrlDecode(parts[0]);
            var value = parts.Length > 1 ? HttpUtility.UrlDecode(parts[1]) : "";
            
            parameters.Add(new KeyValuePair<string, string>(key, value));
        }
        
        // Sort parameters by key
        parameters.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal));
        
        // Rebuild the query string
        var normalizedQuery = string.Join("&", 
            parameters.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
        
        return string.IsNullOrEmpty(normalizedQuery) ? "" : "?" + normalizedQuery;
    }
}