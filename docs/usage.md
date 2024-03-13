# Usage

Given the [example config](setup.md) ([for Umbraco here](setup-umbraco.md)), you can now request remote images using the path `/remote/https://test.com/path/to/img.jpg`.

If using the RemoteUrlPrefix option, you can omit everything stated in the config. Eg. having set RemoteUrlPrefix to `https://test.com`, you can request images at `/remote/path/to/img.jpg`, which would then resolve to `https://test.com/path/to/img.jpg`.

## Resolving "local" URL by remote URL

If you don't like to hardcode URLs to images like the above, there is an extension method in `ImageSharpCommunity.Providers.Remote.Helpers` you can use to resolve the local URL.

You need to inject the RemoteImageProviderOptions in first, and then you call it like `uri.GetRemoteImageProviderUrl(options)`

Heres an example in a Razor view:

```cshtml
@using ImageSharpCommunity.Providers.Remote;
@using ImageSharpCommunity.Providers.Remote.Configuration;
@inject IOptions<RemoteImageProviderOptions> RemoteImageProviderOptions;
@{
    var uri = new Uri("https://test.com/path/to/image.jpg");
    var localUrl = uri.GetRemoteImageProviderUrl(RemoteImageProviderOptions.Value); // "/remote/https://test.com/path/to/img.jpg"
}
```

- [Setup](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/setup.md) ([Umbraco](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/setup-umbraco.md))
- [Configuration](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)