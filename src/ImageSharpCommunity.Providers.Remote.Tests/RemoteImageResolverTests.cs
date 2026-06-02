using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace ImageSharpCommunity.Providers.Remote.Tests;

public class RemoteImageResolverTests
{
    private static RemoteImageProviderSetting BuildSetting(bool allowAll = true, string? userAgent = null)
    {
        return new RemoteImageProviderSetting("/remote")
        {
            AllowAllDomains = allowAll,
            UserAgent = userAgent ?? Guid.NewGuid().ToString(),
        };
    }

    private static RemoteImageProviderOptions BuildOptions(TimeSpan? fallbackMaxAge = null)
    {
        return new RemoteImageProviderOptions
        {
            FallbackMaxAge = fallbackMaxAge ?? TimeSpan.FromHours(1),
            Settings = new List<RemoteImageProviderSetting> { BuildSetting() }
        };
    }

    private static IHttpClientFactory BuildFactory(MockHttpMessageHandler handler)
    {
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>()))
               .Returns(new HttpClient(handler));
        return factory.Object;
    }

    // ---------------------------------------------------------------------------
    // GetMetaDataAsync
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task GetMetaDataAsync_ReturnsMetadataFromResponseHeaders()
    {
        var lastModified = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        var maxAge = TimeSpan.FromMinutes(30);
        var setting = BuildSetting();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Head, "https://example.com/image.png")
                .Respond(req =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(Array.Empty<byte>());
                    response.Content.Headers.ContentLength = 12345;
                    response.Content.Headers.LastModified = lastModified;
                    response.Headers.CacheControl = new CacheControlHeaderValue { MaxAge = maxAge };
                    return response;
                });

        var resolver = new RemoteImageResolver(
            BuildFactory(mockHttp),
            "https://example.com/image.png",
            setting,
            NullLogger<RemoteImageResolver>.Instance,
            BuildOptions());

        var metadata = await resolver.GetMetaDataAsync();

        Assert.Equal(maxAge, metadata.CacheControlMaxAge);
        Assert.Equal(12345, metadata.ContentLength);
    }

    [Fact]
    public async Task GetMetaDataAsync_UsesFallbackMaxAgeWhenNoCacheControlHeader()
    {
        var fallback = TimeSpan.FromHours(2);
        var setting = BuildSetting();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Head, "https://example.com/image.png")
                .Respond(req =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(Array.Empty<byte>());
                    return response;
                });

        var resolver = new RemoteImageResolver(
            BuildFactory(mockHttp),
            "https://example.com/image.png",
            setting,
            NullLogger<RemoteImageResolver>.Instance,
            BuildOptions(fallbackMaxAge: fallback));

        var metadata = await resolver.GetMetaDataAsync();

        Assert.Equal(fallback, metadata.CacheControlMaxAge);
    }

    [Fact]
    public async Task GetMetaDataAsync_UsesDefaultValuesWhenHeadersMissing()
    {
        var setting = BuildSetting();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Head, "https://example.com/image.png")
                .Respond(req =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(Array.Empty<byte>());
                    return response;
                });

        var resolver = new RemoteImageResolver(
            BuildFactory(mockHttp),
            "https://example.com/image.png",
            setting,
            NullLogger<RemoteImageResolver>.Instance,
            BuildOptions());

        var metadata = await resolver.GetMetaDataAsync();

        Assert.Equal(default(DateTimeOffset).UtcDateTime, metadata.LastWriteTimeUtc);
        Assert.Equal(0, metadata.ContentLength);
    }

    // ---------------------------------------------------------------------------
    // OpenReadAsync
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task OpenReadAsync_ReturnsStreamWithImageContent()
    {
        var imageBytes = Encoding.UTF8.GetBytes("fake-image-data");
        var setting = BuildSetting();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, "https://example.com/image.png")
                .Respond("image/png", new MemoryStream(imageBytes));

        var resolver = new RemoteImageResolver(
            BuildFactory(mockHttp),
            "https://example.com/image.png",
            setting,
            NullLogger<RemoteImageResolver>.Instance,
            BuildOptions());

        using var stream = await resolver.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        Assert.Equal(imageBytes, ms.ToArray());
    }

    [Fact]
    public async Task OpenReadAsync_ReturnsEmptyStreamForEmptyResponse()
    {
        var setting = BuildSetting();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, "https://example.com/empty.png")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(Array.Empty<byte>())
                });

        var resolver = new RemoteImageResolver(
            BuildFactory(mockHttp),
            "https://example.com/empty.png",
            setting,
            NullLogger<RemoteImageResolver>.Instance,
            BuildOptions());

        using var stream = await resolver.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        Assert.Equal(0, ms.Length);
    }
}
