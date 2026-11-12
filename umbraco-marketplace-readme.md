# Umbraco.Community.ImageSharpRemoteImages

[![Downloads](https://img.shields.io/nuget/dt/Umbraco.Community.ImageSharpRemoteImages?color=cc9900)](https://www.nuget.org/packages/Umbraco.Community.ImageSharpRemoteImages/)
[![NuGet](https://img.shields.io/nuget/vpre/Umbraco.Community.ImageSharpRemoteImages?color=0273B3)](https://www.nuget.org/packages/Umbraco.Community.ImageSharpRemoteImages)
[![GitHub license](https://img.shields.io/github/license/skttl/ImageSharp.Community.Providers.Remote?color=8AB803)](https://github.com/skttl/ImageSharp.Community.Providers.Remote/blob/main/LICENSE)

Umbraco.Community.ImageSharpRemoteImages is a package wrapping [ImageSharpCommunity.Providers.Remote](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/README.md), which is a library that provides remote image loading functionality for the ImageSharp.Web library. It allows you to load images from remote URLs and integrate them seamlessly into your ImageSharp-based applications.

## Installation

Version 19 targets Umbraco 19 and .NET 11, and uses ImageSharp 4 through ImageSharpCommunity.Providers.Remote 4.0.0. The current prerelease targets Umbraco 19.0.0-rc1.

Before upgrading, review the [Umbraco 19 upgrade notes](https://docs.umbraco.com/umbraco-cms/19.latest/get-started/upgrading-and-migrating/version-specific). In particular, the imaging HMAC key must be canonical Base64 under .NET 11. Correcting the encoding without changing the decoded bytes preserves existing signed image URLs.

You can install the library via NuGet.

```bash
dotnet add package Umbraco.Community.ImageSharpRemoteImages --version VERSION_NUMBER
```

## Documentation

- [Setup](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/setup-umbraco.md)
- [Configuration](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)
- [Usage](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/usage.md)

## Contributing

Contributions to the `Umbraco.Community.ImageSharpRemoteImages` and `ImageSharpCommunity.Providers.Remote` library are welcome! If you encounter any issues or have suggestions for improvements, please feel free to create a new issue or submit a pull request.

## License

The `Umbraco.Community.ImageSharpRemoteImages` and `ImageSharpCommunity.Providers.Remote` library is licensed under the [MIT License](https://opensource.org/licenses/MIT). Please see the [LICENSE](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/LICENSE) file for more details.
