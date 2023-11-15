# ImageSharp.Community.Providers.Remote

ImageSharp.Community.Providers.Remote is a library that provides remote image loading functionality for the ImageSharp.Web library. It allows you to load images from remote URLs and integrate them seamlessly into your ImageSharp-based applications.

## Installation

You can install the library via NuGet.

# [Package Manager](#tab/tabid-1)

```bash
PM > Install-Package ImageSharp.Community.Providers.Remote -Version VERSION_NUMBER
```

# [.NET CLI](#tab/tabid-2)

```bash
dotnet add package ImageSharp.Community.Providers.Remote --version VERSION_NUMBER
```

# [PackageReference](#tab/tabid-3)

```xml
<PackageReference Include="ImageSharp.Community.Providers.Remote" Version="VERSION_NUMBER" />
```

# [Paket CLI](#tab/tabid-4)

```bash
paket add ImageSharp.Community.Providers.Remote --version VERSION_NUMBER
```

## Usage

To use the `ImageSharp.Community.Providers.Remote` library, register the remote image provider in your application's services configuration. This can typically be done in the ConfigureServices method of your Startup class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Other services configuration...

    services.AddImageSharp()
            .Configure<RemoteImageProviderOptions>(options =>
            {
                options.AllowedDomains = new[] { "example.com" };
                options.Prefix = "/images";
                // Additional configuration options...
            })
            .InsertProvider<RemoteImageProvider>(0);
}
```

If you are using the default `WebRootProvider`, you need to make sure the `RemoteImageProvider` is inserted before that. Hence why `InsertProvider` is used instead of `AddProvider` in the above example.

By default, no domains is allowed, so you have to configure your desired `RemoteImageProviderOptions` in order to use this provider.

## Configuration Options

The `RemoteImageProviderOptions` class provides the following configuration options:

- `Prefix`: Specifies the local path to prefix all remote image requests with. For example, setting this to `/remote` allows requests like `/remote/https://test.com/test.png` to pass through this provider.

- `RemoteUrlPrefix` (optional): Prefixes the URL on the server. For example, setting this to `https://test.com/` allows requests like `/remote/test.png` to download `https://test.com/test.png`. Note: You still need to allow the specific domain in the `AllowedDomains` setting.

- `MaxBytes`: Specifies the maximum allowable download size in bytes. By default, it is set to `4194304` bytes (4 MB).

- `Timeout`: Sets the timeout for a request in milliseconds. By default, it is set to `3000` milliseconds (3 seconds).

- `UserAgent`: Sets a user agent value for the request. This can be useful for interacting with social networks. By default, it is set to `"ImageSharpRemoteProvider/0.1"`.

- `HttpClientName`: Sets the name of the HttpClient to use when downloading images. By default, it is set to `"ImageSharpRemoteProvider/HttpClient"`.

- `AllowedDomains`: Specifies a list of allowable domains to process images from. Images from domains not listed here will be blocked. This is an empty list by default. You can add `*` to allow all domains.

- `AdditionalOptions`: Allows specifying additional `RemoteImageProviderOptions` instances. This can be useful when you have multiple configurations with different prefixes or other settings.

Please note that these options provide customization and control over how remote images are loaded and processed. You can adjust these options according to your specific requirements.

Don't forget to configure these options in your application's services configuration as shown in the Usage section of this README.

Please refer to the documentation or the XML comments in the code for more information about each option and its usage.

## Contributing

Contributions to the `ImageSharp.Community.Providers.Remote` library are welcome! If you encounter any issues or have suggestions for improvements, please feel free to create a new issue or submit a pull request.

## License

The `ImageSharp.Community.Providers.Remote` library is licensed under the [MIT License](https://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for more details.