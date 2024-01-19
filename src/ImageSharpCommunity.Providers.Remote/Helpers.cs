using System.Text;
using System.Text.RegularExpressions;
using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.AspNetCore.Http;

namespace ImageSharpCommunity.Providers.Remote;

public static class Helpers
{
    private static Dictionary<string, HttpClient> HttpClients { get; } = new Dictionary<string, HttpClient>();

    /// <summary>
    /// Gets the remote URL for a given path, based on the specified options.
    /// </summary>
    /// <param name="path">The path for which to retrieve the remote URL.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>The remote URL for the given path, or null if no matching remote image provider setting is found.</returns>
    public static HttpClient GetRemoteImageProviderHttpClient(this IHttpClientFactory factory, RemoteImageProviderSetting setting)
    {
        if (HttpClients.TryGetValue(setting.ClientDictionaryKey, out HttpClient? HttpClient) && HttpClient is not null)
        {
            return HttpClient;
        }

        var httpClient = factory.CreateClient(setting.ClientDictionaryKey);

        if (!string.IsNullOrWhiteSpace(setting.UserAgent))
        {
            // set useragent string of client:
            httpClient.DefaultRequestHeaders.Add("User-Agent", setting.UserAgent);
        }
        httpClient.Timeout = TimeSpan.FromMilliseconds(setting.Timeout);
        httpClient.MaxResponseContentBufferSize = setting.MaxBytes;

        HttpClients.TryAdd(setting.ClientDictionaryKey, httpClient);

        return httpClient;
    }

    /// <summary>
    /// Checks if a given path is valid for a specific setting, based on the provided options.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="setting">The remote image provider setting to validate against.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>True if the path is valid for the specified setting, false otherwise.</returns>
    public static bool IsValidForSetting(this Uri uri, RemoteImageProviderSetting setting)
    {
        return setting.AllowedDomains.Contains("*")
            || setting.AllowedDomains.Contains(uri.Host)
            || setting.AllowedDomains.Any(d =>
            {
                var pattern = Regex.Escape(d).Replace("\\*", "(.*)");
                return Regex.IsMatch(uri.Host, $"^{pattern}$");
            });
    }

    /// <summary>
    /// Gets the remote URL for a given path, based on the specified options.
    /// </summary>
    /// <param name="path">The path for which to retrieve the remote URL.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>The remote URL for the given path, or null if no matching remote image provider setting is found.</returns>
    public static string? GetSourceUrlForRemoteImageProviderUrl(this PathString path, RemoteImageProviderOptions options)
    {
        if (
            !path.HasValue
            || path.GetMatchingRemoteImageProviderSetting(options) is not RemoteImageProviderSetting setting
            || setting.Prefix is not string prefix
            || path.Value.Length <= prefix.Length
            )
        {
            return null;
        }

        var remoteUrl = setting.RemoteUrlPrefix + path.Value?[(prefix.Length + 1)..];
        return remoteUrl?.Replace(" ", "%20");
    }

    /// <summary>
    /// Gets the remote image provider setting that matches the given path, based on the specified options.
    /// </summary>
    /// <param name="path">The path for which to find a matching remote image provider setting.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>The remote image provider setting that matches the given path, or null if no matching setting is found.</returns>
    public static RemoteImageProviderSetting? GetMatchingRemoteImageProviderSetting(this PathString path, RemoteImageProviderOptions options)
    {
        return options.Settings?.FirstOrDefault(x => path.IsMatchingRemoteImageProviderSetting(x));
    }

    /// <summary>
    /// Checks if the given path matches any of the remote image provider options.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>True if the given path matches any of the remote image provider options, false otherwise.</returns>
    public static bool IsMatchingRemoteImageProviderOptions(this PathString path, RemoteImageProviderOptions options)
    {
        return path.GetMatchingRemoteImageProviderSetting(options) != null;
    }

    /// <summary>
    /// Checks if the given path matches the specified remote image provider setting.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="setting">The remote image provider setting to compare against.</param>
    /// <returns>True if the given path matches the specified remote image provider setting, false otherwise.</returns>
    public static bool IsMatchingRemoteImageProviderSetting(this PathString path, RemoteImageProviderSetting setting)
    {
        return path.StartsWithSegments(setting.Prefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the remote image provider setting that matches the given URI, based on the specified options.
    /// </summary>
    /// <param name="uri">The URI for which to find a matching remote image provider setting.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>The remote image provider setting that matches the given URI, or null if no matching setting is found.</returns>
    public static RemoteImageProviderSetting? GetMatchingRemoteImageProviderSetting(this Uri uri, RemoteImageProviderOptions options)
    {
        return options.Settings?.FirstOrDefault(x => uri.IsMatchingRemoteImageProviderSetting(x));
    }

    /// <summary>
    /// Checks if the given URI matches any of the remote image provider options.
    /// </summary>
    /// <param name="uri">The URI to check.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>True if the given URI matches any of the remote image provider options, false otherwise.</returns>
    public static bool IsMatchingRemoteImageProviderOptions(this Uri uri, RemoteImageProviderOptions options)
    {
        return uri.GetMatchingRemoteImageProviderSetting(options) != null;
    }

    /// <summary>
    /// Checks if the given URI matches the specified remote image provider setting.
    /// </summary>
    /// <param name="uri">The URI to check.</param>
    /// <param name="setting">The remote image provider setting to compare against.</param>
    /// <returns>True if the given URI matches the specified remote image provider setting, false otherwise.</returns>
    public static bool IsMatchingRemoteImageProviderSetting(this Uri uri, RemoteImageProviderSetting setting)
    {
        return setting.AllowedDomains.Any(x => x == "*" || x.Equals(uri.Host, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the remote URL for the given URI, based on the specified options.
    /// </summary>
    /// <param name="uri">The URI for which to retrieve the remote URL.</param>
    /// <param name="options">The options containing the remote image provider settings.</param>
    /// <returns>The remote URL for the given URI, or null if no matching remote image provider setting is found.</returns>
    public static string? GetRemoteImageProviderUrl(this Uri uri, RemoteImageProviderOptions options)
    {
        if (
            uri.GetMatchingRemoteImageProviderSetting(options) is not RemoteImageProviderSetting setting
            )
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append(setting.Prefix);

        var path = uri.ToString();

        if (
            string.IsNullOrWhiteSpace(setting.RemoteUrlPrefix) == false
            && Uri.TryCreate(setting.RemoteUrlPrefix, UriKind.Absolute, out Uri? remoteUri)
            && remoteUri != null
        )
        {
            path = uri.PathAndQuery;
            if (uri.PathAndQuery.StartsWith(remoteUri.PathAndQuery, StringComparison.OrdinalIgnoreCase))
            {
                path = uri.PathAndQuery[remoteUri.PathAndQuery.Length..];
            }


        }

        if (setting.Prefix.EndsWith("/") == false && path.StartsWith("/") == false)
        {
            sb.Append("/");
        }

        sb.Append(path);

        return sb.ToString();
    }
}
