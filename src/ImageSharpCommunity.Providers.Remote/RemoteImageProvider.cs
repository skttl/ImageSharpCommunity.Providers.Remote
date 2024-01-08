using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

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
            context.Request.Path.GetMatchingRemoteImageProviderSetting(_options) is RemoteImageProviderSetting setting
            && context.Request.Path.GetSourceUrlForRemoteImageProviderUrl(_options) is string url
            && Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) && uri != null
            && uri.IsValidForSetting(setting);
    }

    public Task<IImageResolver?> GetAsync(HttpContext context)
    {
        if (
            context.Request.Path.GetSourceUrlForRemoteImageProviderUrl(_options) is not string url
            || context.Request.Path.GetMatchingRemoteImageProviderSetting(_options) is not RemoteImageProviderSetting options
        )
        {
            return Task.FromResult((IImageResolver?)null);
        }
        else
        {
            return Task.FromResult((IImageResolver?)new RemoteImageResolver(_clientFactory, url, options));
        }
    }
    private bool IsMatch(HttpContext context)
    {
        return context.Request.Path.GetMatchingRemoteImageProviderSetting(_options) != null;
    }
}
