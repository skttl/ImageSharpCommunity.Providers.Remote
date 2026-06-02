# Setup

To use the `ImageSharpCommunity.Providers.Remote` library, register the remote image provider in your application's services configuration. This can typically be done in the ConfigureServices method of your Startup class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Other services configuration...

    services.AddImageSharp()
            .Configure<RemoteImageProviderOptions>(options =>
            {
                options.Settings
                    .Add(new("/remote")
                    {
                        AllowedDomains = new List<string>() { "*" }
                    });
            })
            .InsertProvider<RemoteImageProvider>(0);
}
```

If you are using the default `WebRootProvider`, you need to make sure the `RemoteImageProvider` is inserted before that. Hence why `InsertProvider` is used instead of `AddProvider` in the above example.

## Cache key when using pass-through parameters

When using `PassThroughParameters` or `PassThroughAllParameters`, the custom `RemoteImageProviderCacheKey` must be active so that requests with different pass-through values (e.g. `?id=1` vs `?id=2`) produce distinct cache entries. Without it, ImageSharp.Web's default cache key only considers ImageSharp processing commands (`width`, `format`, etc.) and the two requests would incorrectly share a cache entry.

**This is registered automatically** when calling `InsertProvider<RemoteImageProvider>` or `AddImageSharpRemoteImages` (Umbraco). No additional setup is required.

By default, no domains is allowed, so you have to configure your desired `RemoteImageProviderOptions` in order to use this provider.

- [Configuration](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)
- [Usage](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)