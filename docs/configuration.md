# Configuration Options

The `RemoteImageProviderOptions` class provides the following configuration options:

- `Settings`: A list of the different allowed sources for images. 

Each setting (`RemoteImageProviderSetting`) provides the following configuration options:

- `Prefix`: Specified in the constructor, and defines the local path to prefix all remote image requests with. For example, setting this to `/remote` allows requests like `/remote/https://test.com/test.png` to pass through this provider.

- `RemoteUrlPrefix` (optional): Prefixes the URL on the server. For example, setting this to `https://test.com/` allows requests like `/remote/test.png` to download `https://test.com/test.png`. Note: You still need to allow the specific domain in the `AllowedDomains` setting.

- `MaxBytes`: Specifies the maximum allowable download size in bytes. By default, it is set to `10485760` bytes (10 MB).

- `Timeout`: Sets the timeout for a request in milliseconds. By default, it is set to `180000` milliseconds (180 seconds - 3 minutes).

- `UserAgent`: Sets a user agent value for the request. This can be useful for interacting with social networks. By default, it is set to `"ImageSharpRemoteProvider/[current version]"`.

- `HttpClientName`: Sets the name of the HttpClient to use when downloading images. By default, it is set to `"ImageSharpRemoteProvider/HttpClient"`.

- `AllowedDomains`: Specifies a list of allowable domains to process images from. Images from domains not listed here will be blocked. This is an empty list by default. You can add `*` to allow all domains. This can be used instead of, or in combination with `AllowedDomainsRegex`.

- `AllowedDomainsRegex`: Specifies a list of regex patterns, matching allowable domains to process images from. Images from domains not matching any of the regexes will be blocked. This is an empty list by default. This can be used instead of, or in combination with `AllowedDomains`.

- `AllowAllDomains`: Boolean value. If set to true, all domains is allowed to process.

- `AdditionalOptions`: Allows specifying additional `RemoteImageProviderOptions` instances. This can be useful when you have multiple configurations with different prefixes or other settings.

Please note that these options provide customization and control over how remote images are loaded and processed. You can adjust these options according to your specific requirements.

Don't forget to configure these options in your application's services configuration as shown in the Usage section of this README.

Please refer to the documentation or the XML comments in the code for more information about each option and its usage.

- [Setup](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/setup.md) ([Umbraco](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/setup-umbraco.md))
- [Usage](https://github.com/skttl/ImageSharpCommunity.Providers.Remote/blob/main/docs/usage.md)