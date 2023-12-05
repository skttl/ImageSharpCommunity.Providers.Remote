using Umbraco.Cms.Core.Manifest;

namespace Umbraco.Community.ImageSharpRemoteImages;

public class ImageSharpRemoteImagesManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest()
        {
            AllowPackageTelemetry = true,
            PackageName = "ImageSharp Remote Images"
        });
    }
}
