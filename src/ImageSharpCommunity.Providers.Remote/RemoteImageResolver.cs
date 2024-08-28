using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Web.Resolvers;
namespace ImageSharpCommunity.Providers.Remote;

public class RemoteImageResolver : IImageResolver
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _url;
    private readonly RemoteImageProviderSetting _setting;
    private readonly ILogger<RemoteImageResolver> _logger;

    public RemoteImageResolver(IHttpClientFactory clientFactory, string url, RemoteImageProviderSetting setting, ILogger<RemoteImageResolver> logger)
    {
        _clientFactory = clientFactory;
        _url = url;
        _setting = setting;
        _logger = logger;
    }

    public async Task<ImageMetadata> GetMetaDataAsync()
    {
        _logger.LogDebug("Requesting metadata from {Url}", _url);

        var client = _clientFactory.GetRemoteImageProviderHttpClient(_setting);

        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, _url), HttpCompletionOption.ResponseHeadersRead);

        if (!response.Content.Headers.ContentLength.HasValue)
        {
            _logger.LogDebug("ContentLength header missing from {Url}", _url);
        }

        if (!response.Content.Headers.LastModified.HasValue)
        {
            _logger.LogDebug("LastModified header missing from {Url}", _url);
        }

        if (response.Headers.CacheControl?.MaxAge is null)
        {
            _logger.LogDebug("MaxAge header is null from {Url}", _url);
        }

        return new ImageMetadata(response.Content.Headers.LastModified.GetValueOrDefault().UtcDateTime, (response.Headers.CacheControl?.MaxAge).GetValueOrDefault(), response.Content.Headers.ContentLength.GetValueOrDefault());
    }

    public async Task<Stream> OpenReadAsync()
    {
        _logger.LogDebug("Requesting image from {Url}", _url);

        var client = _clientFactory.GetRemoteImageProviderHttpClient(_setting);

        var response = await client.GetAsync(_url);

        return await response.Content.ReadAsStreamAsync();
    }
}
