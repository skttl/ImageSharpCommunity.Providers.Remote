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

    public RemoteImageProvider(
        IHttpClientFactory clientFactory,
        IOptions<RemoteImageProviderOptions> options,
        ILogger<RemoteImageProvider> logger,
        ILogger<RemoteImageResolver> resolverLogger,
        IMemoryCache cache
    )
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
        if (
            context.Request.Path.GetMatchingRemoteImageProviderSetting(_options)
            is not RemoteImageProviderSetting setting
        )
        {
            _logger.LogDebug(
                "No matching remote image provider setting found for path: {path}",
                context.Request.Path
            );
            return false;
        }
        if (
            context.Request.Path.GetSourceUrlForRemoteImageProviderUrl(
                _options,
                context.Request.QueryString
            )
            is not string url
        )
        {
            _logger.LogDebug("Source url for path {path} returned null", context.Request.Path);
            return false;
        }
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) || uri is null)
        {
            _logger.LogDebug(
                "Source url ({url}) for path {path} could not be parsed",
                url,
                context.Request.Path
            );
            return false;
        }
        if (!uri.IsValidForSetting(setting))
        {
            _logger.LogDebug(
                "Source url ({url}) for path {path} is not valid for setting with prefix {setting}",
                url,
                context.Request.Path,
                setting.Prefix
            );
            return false;
        }
        if (!UrlReturnsSuccess(setting, uri))
        {
            _logger.LogDebug(
                "Source url ({url}) for path {path} did not return success",
                url,
                context.Request.Path
            );
            return false;
        }

        return true;
    }

    public Task<IImageResolver?> GetAsync(HttpContext context)
    {
        if (
            context.Request.Path.GetSourceUrlForRemoteImageProviderUrl(
                _options,
                context.Request.QueryString
            )
                is not string url
            || context.Request.Path.GetMatchingRemoteImageProviderSetting(_options)
                is not RemoteImageProviderSetting options
        )
        {
            _logger.LogDebug(
                "No matching remote image provider setting found for path: {path}",
                context.Request.Path
            );
            return Task.FromResult((IImageResolver?)null);
        }
        else
        {
            _logger.LogDebug(
                "Found matching remote image provider setting for path: {path}",
                context.Request.Path
            );
            return Task.FromResult(
                (IImageResolver?)
                    new RemoteImageResolver(_clientFactory, url, options, _resolverLogger, _options)
            );
        }
    }

    private bool IsMatch(HttpContext context)
    {
        if (context.Request.Path.GetMatchingRemoteImageProviderSetting(_options) is null)
        {
            _logger.LogInformation(
                "No matching remote image provider setting found for path: {path}",
                context.Request.Path
            );
            return false;
        }
        return true;
    }

    private bool UrlReturnsSuccess(RemoteImageProviderSetting setting, Uri uri)
    {
        if (setting.VerifyUrl == false)
        {
            _logger.LogDebug(
                "Skipping verification of URL {Url} as VerifyUrl is set to false",
                uri
            );
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
            _logger.LogDebug(
                "Setting cache for URL {Url} with MaxAge {MaxAge}",
                uri,
                response.Headers.CacheControl.MaxAge
            );
            _cache.Set(
                nameof(RemoteImageProvider) + uri,
                response.IsSuccessStatusCode,
                response.Headers.CacheControl.MaxAge ?? _options.FallbackMaxAge
            );
        }

        return response.IsSuccessStatusCode;
    }
}
