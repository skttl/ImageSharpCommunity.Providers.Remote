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

By default, no domains is allowed, so you have to configure your desired `RemoteImageProviderOptions` in order to use this provider.

- [Configuration](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)
- [Usage](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/configuration.md)