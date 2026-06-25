using System.Diagnostics;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Umbraco.Community.ImageSharpRemoteImages;

public class ImageSharpRemoteImagesManifestReader : IPackageManifestReader
{
    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        return await Task.Run(() =>
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location
            );
            var version =
                $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";
            return new List<PackageManifest>()
            {
                new PackageManifest()
                {
                    AllowTelemetry = true,
                    Name = "Umbraco.Community.ImageSharpRemoteImages",
                    Extensions = Array.Empty<object>(),
                    Version = version,
                },
            };
        });
    }
}
