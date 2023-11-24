using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using Microsoft.Extensions.Options;
using ImageSharpCommunity.Providers.Remote.Configuration;
using System.Text.RegularExpressions;

namespace ImageSharpCommunity.Providers.Remote;

public class RemoteImageProvider : IImageProvider
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly RemoteImageProviderOptions _options;

    public RemoteImageProvider(IHttpClientFactory clientFactory, IOptions<RemoteImageProviderOptions> options)
    {
        _clientFactory = clientFactory;
        _options = options.Value;
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
            GetMatchingSetting(context) is RemoteImageProviderSetting options 
            && GetRemoteUrl(context) is string url
            && Uri.IsWellFormedUriString(url, UriKind.Absolute) 
            && IsValidUrl(new Uri(url), options);
    }

    public Task<IImageResolver?> GetAsync(HttpContext context)
    {
        if (GetRemoteUrl(context) is not string url || GetMatchingSetting(context) is not RemoteImageProviderSetting options)
        {
            return Task.FromResult((IImageResolver?)null);
        }
        else
        {
            return Task.FromResult((IImageResolver?)new RemoteImageResolver(_clientFactory, url, options));
        }
    }

    private string? GetRemoteUrl(HttpContext context)
    {
        if (
            !context.Request.Path.HasValue 
            || GetMatchingSetting(context) is not RemoteImageProviderSetting options
            || options.Prefix is not string prefix 
            || context.Request.Path.Value.Length <= prefix.Length
            )
        {
            return null;
        }

        var remoteUrl = options.RemoteUrlPrefix + context.Request.Path.Value?[(prefix.Length + 1)..];
        return remoteUrl?.Replace(" ", "%20");
    }

    private RemoteImageProviderSetting? GetMatchingSetting(HttpContext context)
    {
        return _options.Settings?.FirstOrDefault(x => IsMatch(context, x));
    }

    private bool IsMatch(HttpContext context)
    {
        return GetMatchingSetting(context) != null;
    }

    private bool IsMatch(HttpContext context, RemoteImageProviderSetting options)
    {
        return context.Request.Path.StartsWithSegments(options.Prefix, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidUrl(Uri url, RemoteImageProviderSetting options)
    {
        return 
            options.AllowedDomains.Contains("*") 
            || options.AllowedDomains.Contains(url.Host) 
            || options.AllowedDomains.Any(d =>
            {
                var pattern = Regex.Escape(d).Replace("\\*", "(.*)");
                return Regex.IsMatch(url.Host, $"^{pattern}$");
            });
    }
}
