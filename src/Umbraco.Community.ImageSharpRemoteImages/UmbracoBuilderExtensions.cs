using ImageSharpCommunity.Providers.Remote;
using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Umbraco.Community.ImageSharpRemoteImages;

public static class UmbracoBuilderExtensions
{
    public static IUmbracoBuilder AddImageSharpRemoteImages(this IUmbracoBuilder builder, Action<RemoteImageProviderOptions>? defaultOptions = default)
    {
        // load up the settings. 
        var options = builder.Services.AddOptions<RemoteImageProviderOptions>()
            .Bind(builder.Config.GetSection("Umbraco:Community:ImageSharpRemoteImages"));

        if (defaultOptions != default)
        {
            options.Configure(defaultOptions);
        }
        options.ValidateDataAnnotations();

        builder.Services.AddSingleton<IPackageManifestReader, ImageSharpRemoteImagesManifestReader>();

        builder.Services.InsertImageProvider<RemoteImageProvider>(0);

        return builder;
    }
}
