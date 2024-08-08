using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Umbraco.Community.ImageSharpRemoteImages;

public class ImageSharpRemoteImagesManifestReader : IPackageManifestReader
{
    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        return await Task.Run(() =>
        {
            return new List<PackageManifest>() {
                new PackageManifest()
            {
                AllowTelemetry = true,
                Name = "ImageSharp Remote Images",
                Extensions = Array.Empty<object>(),
                Version = "14.0.0"
                }
        };
        });
    }
}
