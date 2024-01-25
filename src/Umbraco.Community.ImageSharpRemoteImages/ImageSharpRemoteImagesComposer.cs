using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Imaging.ImageSharp;

namespace Umbraco.Community.ImageSharpRemoteImages;

[ComposeAfter(typeof(ImageSharpComposer))]
public class ImageSharpRemoteImagesComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddImageSharpRemoteImages();
    }
}
