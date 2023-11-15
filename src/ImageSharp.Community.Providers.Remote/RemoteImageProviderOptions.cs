namespace ImageSharp.Community.Providers.Remote;
public class RemoteImageProviderOptions
{
    /// <summary>
    /// The local path to prefix all remote image requests with. Eg. /remote makes eg. /remote/https://test.com/test.png pass through this provider.
    /// </summary>
    public string Prefix { get; set; } = "/remote";

    /// <summary>
    /// (Optional) Prefix the url on the server, eg. set this to https://test.com/, and /remote/test.png will download https://test.com/test.png
    /// </summary>
    public string? RemoteUrlPrefix { get; set; }

    /// <summary>
    /// Maximum allowable download in bytes.
    /// </summary>
    public int MaxBytes { get; set; } = 4194304;

    /// <summary>
    /// Timeout for a request in milliseconds
    /// </summary>
    public int Timeout { get; set; } = 3000;

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

    public List<RemoteImageProviderOptions>? AdditionalOptions { get; set; }
}