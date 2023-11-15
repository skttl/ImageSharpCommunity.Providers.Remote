using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using Microsoft.Extensions.Options;

namespace ImageSharp.Community.Providers.Remote;

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

    private Func<HttpContext, bool>? match;
    public Func<HttpContext, bool> Match
    {
        get => this.match ?? this.IsMatch;
        set => this.match = value;
    }

    public bool IsValidRequest(HttpContext context)
    {
        return 
            GetMatchingProviderOption(context) is RemoteImageProviderOptions options 
            && GetRemoteUrl(context) is string url
            && Uri.IsWellFormedUriString(url, UriKind.Absolute) 
            && IsValidUrl(new Uri(url), options);
    }

    public Task<IImageResolver?> GetAsync(HttpContext context)
    {
        if (GetRemoteUrl(context) is not string url || GetMatchingProviderOption(context) is not RemoteImageProviderOptions options)
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
            || GetMatchingProviderOption(context) is not RemoteImageProviderOptions options
            || options.Prefix is not string prefix 
            || context.Request.Path.Value.Length <= prefix.Length
            )
        {
            return null;
        }

        var remoteUrl = options.RemoteUrlPrefix + context.Request.Path.Value?.Substring(prefix.Length + 1);
        return remoteUrl?.Replace(" ", "%20");
    }

    private RemoteImageProviderOptions? GetMatchingProviderOption(HttpContext context)
    {
        return IsMatch(context, _options) 
            ? _options 
            : _options.AdditionalOptions?.FirstOrDefault(x => IsMatch(context, x));
    }

    private bool IsMatch(HttpContext context)
    {
        return GetMatchingProviderOption(context) != null;
    }

    private bool IsMatch(HttpContext context, RemoteImageProviderOptions options)
    {
        return context.Request.Path.StartsWithSegments(options.Prefix, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidUrl(Uri url, RemoteImageProviderOptions options)
    {
        return options.AllowedDomains.Contains("*") || options.AllowedDomains.Contains(url.Host);
    }
}
