using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace ImageSharpCommunity.Providers.Remote;

public class RemoteImageProvider : IImageProvider
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly RemoteImageProviderOptions _options;
    private readonly ILogger<RemoteImageProvider> _logger;
    private readonly ILogger<RemoteImageResolver> _resolverLogger;
    private readonly IMemoryCache _cache;

    public RemoteImageProvider(IHttpClientFactory clientFactory, IOptions<RemoteImageProviderOptions> options, ILogger<RemoteImageProvider> logger, ILogger<RemoteImageResolver> resolverLogger, IMemoryCache cache)
    {
        _clientFactory = clientFactory;
        _options = options.Value;
        _logger = logger;
        _resolverLogger = resolverLogger;
        _cache = cache;
    }

    public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.All;

    private Func<HttpContext, bool>? _match;
    public Func<HttpContext, bool> Match
    {
        get => _match ?? IsMatch;
        set => _match = value;
    }

    public bool IsValidRequest(HttpContext context)
    {
        return
            context.Request.Path.GetMatchingRemoteImageProviderSetting(_options) is RemoteImageProviderSetting setting
            && context.Request.Path.GetSourceUrlForRemoteImageProviderUrl(_options) is string url
            && Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) && uri != null
            && uri.IsValidForSetting(setting)
            && UrlReturnsSuccess(setting, uri);
    }

    public Task<IImageResolver?> GetAsync(HttpContext context)
    {
        if (
            context.Request.Path.GetSourceUrlForRemoteImageProviderUrl(_options) is not string url
            || context.Request.Path.GetMatchingRemoteImageProviderSetting(_options) is not RemoteImageProviderSetting options
        )
        {
            _logger.LogDebug("No matching remote image provider setting found for path: {path}", context.Request.Path);
            return Task.FromResult((IImageResolver?)null);
        }
        else
        {
            _logger.LogDebug("Found matching remote image provider setting for path: {path}", context.Request.Path);
            return Task.FromResult((IImageResolver?)new RemoteImageResolver(_clientFactory, url, options, _resolverLogger));
        }
    }
    private bool IsMatch(HttpContext context)
    {
        return context.Request.Path.GetMatchingRemoteImageProviderSetting(_options) != null;
    }

    private bool UrlReturnsSuccess(RemoteImageProviderSetting setting, Uri uri)
    {
        if (setting.VerifyUrl == false)
        {
            _logger.LogDebug("Skipping verification of URL {Url} as VerifyUrl is set to false", uri);
            return true;
        }

        if (_cache.TryGetValue(nameof(RemoteImageProvider) + uri, out bool cachedResult))
        {
            _logger.LogDebug("Using cached result for URL {Url}", uri);
            return cachedResult;
        }

        var client = _clientFactory.GetRemoteImageProviderHttpClient(setting);
        var request = new HttpRequestMessage(HttpMethod.Head, uri);
        var response = client.SendAsync(request).Result;

        if (response.Headers.CacheControl?.MaxAge is not null)
        {
            _cache.Set(nameof(RemoteImageProvider) + uri, response.IsSuccessStatusCode, response.Headers.CacheControl.MaxAge ?? TimeSpan.Zero);
        }

        return response.IsSuccessStatusCode;
    }
}
