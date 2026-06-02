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
    public string UserAgent { get; set; } = "ImageSharpRemoteProvider/" + typeof(RemoteImageProviderSetting).Assembly.GetName().Version;

    /// <summary>
    /// Sets the name of the HttpClient to use when downloading images.
    /// </summary>
    public string HttpClientName { get; set; } = "ImageSharpRemoteProvider/HttpClient";

    /// <summary>
    /// Sets allowable domains to process images from.
    /// </summary>
    public List<string> AllowedDomains { get; set; } = new List<string>();

    /// <summary>
    /// Sets allowable domains to process images from.
    /// </summary>
    public List<string> AllowedDomainsRegex { get; set; } = new List<string>();

    /// <summary>
    /// Allows all domains to be processed.
    /// </summary>
    public bool AllowAllDomains { get; set; }

    /// <summary>
    /// Verify that the input url returns a succesful status code.
    /// </summary>
    public bool VerifyUrl { get; set; } = true;

    /// <summary>
    /// If set to true, all eligible query string parameters from the incoming request are forwarded to the remote URL.
    /// When true, <see cref="PassThroughParameters"/> is ignored.
    /// </summary>
    public bool PassThroughAllParameters { get; set; }

    /// <summary>
    /// Query string parameter names from the incoming request that should be passed through to the remote URL.
    /// Ignored when <see cref="PassThroughAllParameters"/> is true. Empty by default (forward none).
    /// </summary>
    public List<string> PassThroughParameters { get; set; } = new List<string>();

    internal string ClientDictionaryKey => $"{HttpClientName}_{UserAgent}_{Timeout}_{MaxBytes}";
}
