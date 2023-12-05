using ImageSharpCommunity.Providers.Remote;
using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Web.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco.Community.ImageSharpRemoteImages;

public static class UmbracoBuilderExtensions
{
    public static IUmbracoBuilder AddImageSharpRemoteImages(this IUmbracoBuilder builder, Action<RemoteImageProviderOptions>? defaultOptions = default)
    {
        // if the Manifest Filter is registred then we assume this has been added before so we don't do it again. 
        if (builder.ManifestFilters().Has<ImageSharpRemoteImagesManifestFilter>())
        {
            return builder;
        }

        // load up the settings. 
        var options = builder.Services.AddOptions<RemoteImageProviderOptions>()
            .Bind(builder.Config.GetSection("Umbraco:Community:ImageSharpRemoteImages"));

        if (defaultOptions != default)
        {
            options.Configure(defaultOptions);
        }
        options.ValidateDataAnnotations();

        builder.ManifestFilters().Append<ImageSharpRemoteImagesManifestFilter>();

        builder.Services
                .AddImageSharp()
                // needs to insert it at position 0, because it needs to go before WebRootProvider
                .InsertProvider<RemoteImageProvider>(0);

        return builder;
    }
}
