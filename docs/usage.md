# Usage

Given the [example config](setup.md) ([for Umbraco here](setup-umbraco.md)), you can now request remote images using the path `/remote/https://test.com/path/to/img.jpg`.

If using the RemoteUrlPrefix option, you can omit everything stated in the config. Eg. having set RemoteUrlPrefix to `https://test.com`, you can request images at `/remote/path/to/img.jpg`, which would then resolve to `https://test.com/path/to/img.jpg`.

## Passing query string parameters to the remote URL

By default, no query string parameters from the incoming request are forwarded to the remote server. Control this per-setting using `PassThroughAllParameters` and `PassThroughParameters`:

- **`PassThroughAllParameters: true`** — forward all eligible parameters to the remote URL
- **`PassThroughParameters: ["id", "format"]`** — forward only the named parameters (ignored when `PassThroughAllParameters` is true)
- Neither set (default) — forward no parameters

```json
{
  "RemoteImageProvider": {
    "Settings": [
      {
        "Prefix": "/remote",
        "AllowAllDomains": true,
        "PassThroughParameters": ["id", "format"]
      }
    ]
  }
}
```

A request to `/remote/image.png?id=123&format=webp&width=400` will fetch `https://yourserver.com/image.png?id=123&format=webp` from the remote — only the listed parameters are forwarded. `width` is ignored by the remote fetch (but remains in the query string for ImageSharp to process locally).

## Separating remote parameters from ImageSharp processing parameters

You can use a marker parameter to clearly split "what goes to the remote server" from "what ImageSharp processes locally". Configure `QueryParameterMarker` on `RemoteImageProviderOptions` (default: `_iscpr`):

```
/remote/image.png?id=123&format=webp&_iscpr&width=400&format=jpg
```

- Parameters **before** `_iscpr` — eligible for pass-through to the remote (subject to `PassThroughParameters`)
- Parameters **after** `_iscpr` — kept for ImageSharp processing only, never forwarded

In the example above (with `PassThroughParameters: ["id", "format"]`):
- Remote fetch: `image.png?id=123&format=webp`
- ImageSharp processes: `width=400&format=jpg` → returns a resized JPG

You can disable the marker entirely by setting `QueryParameterMarker` to `null` or empty string in your options.

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