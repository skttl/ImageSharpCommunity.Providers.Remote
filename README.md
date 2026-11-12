# ImageSharpCommunity.Providers.Remote

[![Downloads](https://img.shields.io/nuget/dt/ImageSharpCommunity.Providers.Remote?color=cc9900)](https://www.nuget.org/packages/ImageSharpCommunity.Providers.Remote/)
[![NuGet](https://img.shields.io/nuget/vpre/ImageSharpCommunity.Providers.Remote?color=0273B3)](https://www.nuget.org/packages/ImageSharpCommunity.Providers.Remote)
[![GitHub license](https://img.shields.io/github/license/skttl/ImageSharp.Community.Providers.Remote?color=8AB803)](https://github.com/skttl/ImageSharp.Community.Providers.Remote/blob/main/LICENSE)

ImageSharpCommunity.Providers.Remote is a library that provides remote image loading functionality for the ImageSharp.Web library. It allows you to load images from remote URLs and integrate them seamlessly into your ImageSharp-based applications.

[A wrapper package for Umbraco, simplyfying the setup is also available!](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/umbraco-marketplace-readme.md)

## Installation

Version 4 targets .NET 8 or later and uses ImageSharp.Web 4. The package major version follows ImageSharp. Use version 1.x for applications that still use ImageSharp.Web 3.

ImageSharp.Web 4 requires a [Six Labors license](https://docs.sixlabors.com/articles/imagesharp.web/) for Release builds. This repository includes an assembly-scoped Community license in `sixlabors.lic`, configured through `Directory.Build.props`, for building the named open-source assemblies locally and in CI. It expires on 2028-02-10. Six Labors permits publishing assembly-scoped Community licenses; unrestricted and commercial license keys must remain private.

Publish `ImageSharpCommunity.Providers.Remote` 4.0.0 before publishing the Umbraco 19 package. The build workflow uses a local package to verify both packages before publication.

You can install the library via NuGet.

```bash
dotnet add package ImageSharpCommunity.Providers.Remote --version VERSION_NUMBER
```

## Documentation

- [Setup](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/setup.md)
- [Configuration](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)
- [Usage](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/usage.md)

## Contributing

Contributions to the `ImageSharpCommunity.Providers.Remote` library are welcome! If you encounter any issues or have suggestions for improvements, please feel free to create a new issue or submit a pull request.

## License

The `ImageSharpCommunity.Providers.Remote` library is licensed under the [MIT License](https://opensource.org/licenses/MIT). Please see the [LICENSE](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/LICENSE) file for more details.
