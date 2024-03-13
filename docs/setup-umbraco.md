# Setup

`Umbraco.Community.ImageSharpRemoteImages` is registered automatically when Umbraco starts, you just need to configure it.

You can configure using appsettings.json, and a schema file is provided for easier setup.

A configuration can look like this:

```json
{
  "Umbraco": {
    "Community": {
      "ImageSharpRemoteImages": {
        "Settings": [
          {
            "Prefix": "/ourumb",
            "RemoteUrlPrefix": "https://our.umbraco.com",
            "AllowedDomains": [ "our.umbraco.com" ]
          },
          {
            "Prefix": "/remote",
            "AllowedDomains": ["*"]
          }
        ]
      }
    }
}
```

By default, no domains is allowed, so you have to configure your desired `RemoteImageProviderOptions` in order to use this provider.

- [Configuration](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)
- [Usage](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/usage.md)