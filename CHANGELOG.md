# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

## [1.3.1]
- **`GetRemoteImageProviderUrl` updated** — fixed a regression when generating remote urls for images, where the settings doesn't auto prefix the hostname. Also removed the default addition of the `QueryParameterMarker`.

## [1.3.0]

### Added

- **`RemoteImageProviderCacheKey`** — new `ICacheKey` implementation that extends the default ImageSharp.Web cache key with pass-through query parameters, ensuring requests with different remote-bound values (e.g. `?id=1` vs `?id=2`) produce distinct cache entries.
- **`SetRemoteImageProviderCacheKey()`** — extension method on both `IImageSharpBuilder` and `IServiceCollection` for manual registration of the cache key.
- **`GetRemoteImageProviderUrl` updated** — when building a local URL from a remote URI (e.g. in Razor views), query parameters are now filtered through `PassThrough(All)Parameters` and the `QueryParameterMarker` is automatically appended.

### Changed

- `InsertImageProvider<RemoteImageProvider>()` now automatically registers `RemoteImageProviderCacheKey`. No manual setup required when using pass-through parameters.

---

## [1.2.0]

### Added

- **Query string pass-through** — two new properties on `RemoteImageProviderSetting` allow query string parameters from the incoming request to be forwarded to the remote URL:
  - `PassThroughAllParameters` (`bool`, default `false`) — when `true`, all eligible query string parameters are forwarded. `PassThroughParameters` is ignored when this is set.
  - `PassThroughParameters` (`List<string>`, default empty) — an explicit allow-list of parameter names to forward.
- **`QueryParameterMarker`** — new property on `RemoteImageProviderOptions` (default `"_iscpr"`). Acts as a separator in the incoming query string: parameters *before* the marker are eligible for pass-through to the remote URL; parameters *after* it are treated as local ImageSharp processing parameters. Set to `null` or empty to disable.
- **`GetSourceUrlForRemoteImageProviderUrl` extended** — the helper now accepts an optional `QueryString queryString` parameter so pass-through logic is applied when building the source URL.
- **`GetImageSharpRequestUrl` fixed** — the URL builder now uses `AbsolutePath` instead of `ToString()`/`PathAndQuery` when stripping the remote prefix, preventing the remote prefix path from being doubled in the resulting ImageSharp URL. Passed-through query parameters and the marker are appended correctly.

### Changed

- `RemoteImageProvider.IsValidRequest` and `RemoteImageProvider.GetAsync` now pass `context.Request.QueryString` to `GetSourceUrlForRemoteImageProviderUrl` so the full pass-through pipeline is exercised at request time.

### Fixed

- `GetImageSharpRequestUrl` no longer includes the remote URL's path segment twice when the remote prefix contains a path (e.g. `https://example.com/images/`).

## [v1.1.0]

### Added

- `VerifyUrl` option to check that a remote URL returns a success status code before selecting the provider.
- `FallbackMaxAge` option on `RemoteImageProviderOptions` — used when the remote server does not return a `Cache-Control` header.

## [v1.0.1]

### Added

- `MaxAge` added to `ImageMetaData` to avoid unnecessary repeated metadata lookups via HTTP.

## [v1.0.0]

- Initial stable release.

# Umbraco package changelog

## [Unreleased]

## [18.0.0]
- Upgrades Umbraco dependency to Umbraco 18

## [17.3.0]
- Fixes version number in package.manifest
- Upgrades ImageSharpComminty.Providers.Remote dependency

## [17.2.0]

- Upgrade dependencies for Umbraco 17.

### Changed

- `AddImageSharpRemoteImages()` now automatically registers `RemoteImageProviderCacheKey`. No manual setup required when using pass-through parameters.

---

## [17.1.0]

- Upgrade dependencies for Umbraco 17.

## [17.0.0]

- Upgrade dependencies for Umbraco 17.

## [16.0.0]

- Upgrade dependencies for Umbraco 16.

## [15.0.0]

- Upgrade dependencies for Umbraco 15 / .NET 9.