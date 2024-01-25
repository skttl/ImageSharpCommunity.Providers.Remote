namespace ImageSharpCommunity.Providers.Remote.Configuration;

public class RemoteImageProviderSetting
{
    public RemoteImageProviderSetting(string prefix)
    {
        if (!prefix.StartsWith("/"))
        {
            prefix = $"/{prefix}";
        }

        Prefix = prefix;
    }

    /// <summary>
    /// The local path to prefix all remote image requests with. Eg. /remote makes eg. /remote/https://test.com/test.png pass through this provider.
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// (Optional) Prefix the url on the server, eg. set this to https://test.com/, and /remote/test.png will download https://test.com/test.png
    /// </summary>
    public string? RemoteUrlPrefix { get; set; }

    /// <summary>
    /// Maximum allowable download in bytes. (default 10485760 = 10MB)
    /// </summary>
    public int MaxBytes { get; set; } = 10485760;

    /// <summary>
    /// Timeout for a request in milliseconds. (default 180000 = 180s = 3 minutes)
    /// </summary>
    public int Timeout { get; set; } = 180000;

    /// <summary>
    /// Sets a useragent value for the request. Useful for social networks. See http://www.useragentstring.com/ for available values.
    /// </summary>
    public string UserAgent { get; set; } = "ImageSharpRemoteProvider/0.1";

    /// <summary>
    /// Sets the name of the HttpClient to use when downloading images.
    /// </summary>
    public string HttpClientName { get; set; } = "ImageSharpRemoteProvider/HttpClient";

    /// <summary>
    /// Sets allowable domains to process images from.
    /// </summary>
    public List<string> AllowedDomains { get; set; } = new List<string>();

    internal string ClientDictionaryKey => $"{HttpClientName}_{UserAgent}_{Timeout}_{MaxBytes}";
}
