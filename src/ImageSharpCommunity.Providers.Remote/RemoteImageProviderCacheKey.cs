using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;

namespace ImageSharpCommunity.Providers.Remote;

/// <summary>
/// Cache key implementation that extends the default ImageSharp.Web cache key with any
/// pass-through query parameters from the incoming request. This ensures that requests
/// with different remote-bound parameters (e.g. ?id=1 vs ?id=2) produce distinct cache entries.
/// </summary>
public class RemoteImageProviderCacheKey : ICacheKey
{
    private readonly RemoteImageProviderOptions _options;

    public RemoteImageProviderCacheKey(IOptions<RemoteImageProviderOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc/>
    public string Create(HttpContext context, CommandCollection commands)
    {
        var baseKey = CaseHandlingUriBuilder.BuildRelative(
            CaseHandlingUriBuilder.CaseHandling.LowerInvariant,
            context.Request.PathBase,
            context.Request.Path,
            QueryString.Create(commands));

        var setting = context.Request.Path.GetMatchingRemoteImageProviderSetting(_options);
        if (setting is null || (!setting.PassThroughAllParameters && setting.PassThroughParameters.Count == 0))
        {
            return baseKey;
        }

        var rawQuery = context.Request.QueryString.Value?.TrimStart('?');
        if (string.IsNullOrEmpty(rawQuery))
        {
            return baseKey;
        }

        var allParams = rawQuery
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(p =>
            {
                var eq = p.IndexOf('=');
                return eq >= 0
                    ? (Key: Uri.UnescapeDataString(p[..eq]), Value: (string?)Uri.UnescapeDataString(p[(eq + 1)..]))
                    : (Key: Uri.UnescapeDataString(p), Value: (string?)null);
            });

        var markerKey = _options.QueryParameterMarker;
        var eligibleParams = !string.IsNullOrEmpty(markerKey)
            ? allParams.TakeWhile(p => !string.Equals(p.Key, markerKey, StringComparison.OrdinalIgnoreCase))
            : allParams;

        var passThroughSet = setting.PassThroughAllParameters
            ? null
            : new HashSet<string>(setting.PassThroughParameters, StringComparer.OrdinalIgnoreCase);

        var passThrough = string.Join("&", eligibleParams
            .Where(p => passThroughSet is null || passThroughSet.Contains(p.Key))
            .Select(p => p.Value is not null
                ? $"{p.Key.ToLowerInvariant()}={p.Value.ToLowerInvariant()}"
                : p.Key.ToLowerInvariant()));

        if (string.IsNullOrEmpty(passThrough))
        {
            return baseKey;
        }

        return baseKey + "|" + passThrough;
    }
}
