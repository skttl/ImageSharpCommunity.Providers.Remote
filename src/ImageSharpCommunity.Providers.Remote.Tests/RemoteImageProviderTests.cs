using System.Net;
using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using SixLabors.ImageSharp.Web.Resolvers;
using Xunit;

namespace ImageSharpCommunity.Providers.Remote.Tests;

public class RemoteImageProviderTests
{
    private static RemoteImageProviderOptions BuildOptions(string prefix, bool allowAll = true, bool verifyUrl = false, string? userAgent = null)
    {
        return new RemoteImageProviderOptions
        {
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting(prefix)
                {
                    AllowAllDomains = allowAll,
                    VerifyUrl = verifyUrl,
                    UserAgent = userAgent ?? Guid.NewGuid().ToString(),
                }
            }
        };
    }

    private static RemoteImageProvider BuildProvider(
        IHttpClientFactory factory,
        RemoteImageProviderOptions options)
    {
        return new RemoteImageProvider(
            factory,
            new OptionsWrapper<RemoteImageProviderOptions>(options),
            NullLogger<RemoteImageProvider>.Instance,
            NullLogger<RemoteImageResolver>.Instance,
            new MemoryCache(new MemoryCacheOptions()));
    }

    private static DefaultHttpContext BuildContext(string path)
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Path = path;
        return ctx;
    }

    // ---------------------------------------------------------------------------
    // Match / IsMatch
    // ---------------------------------------------------------------------------

    [Fact]
    public void Match_ReturnsTrueForPathMatchingPrefix()
    {
        var options = BuildOptions("/remote");
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);
        var ctx = BuildContext("/remote/https://example.com/image.png");

        Assert.True(provider.Match(ctx));
    }

    [Fact]
    public void Match_ReturnsFalseForNonMatchingPath()
    {
        var options = BuildOptions("/remote");
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);
        var ctx = BuildContext("/images/foo.png");

        Assert.False(provider.Match(ctx));
    }

    // ---------------------------------------------------------------------------
    // IsValidRequest
    // ---------------------------------------------------------------------------

    [Fact]
    public void IsValidRequest_ReturnsTrueWhenVerifyUrlFalseAndDomainAllowed()
    {
        var options = BuildOptions("/remote", allowAll: true, verifyUrl: false);
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);
        var ctx = BuildContext("/remote/https://example.com/image.png");

        Assert.True(provider.IsValidRequest(ctx));
    }

    [Fact]
    public void IsValidRequest_ReturnsFalseWhenDomainNotAllowed()
    {
        var options = new RemoteImageProviderOptions
        {
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = false,
                    AllowedDomains = new List<string> { "allowed.com" },
                    VerifyUrl = false,
                }
            }
        };
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);
        var ctx = BuildContext("/remote/https://notallowed.com/image.png");

        Assert.False(provider.IsValidRequest(ctx));
    }

    [Fact]
    public void IsValidRequest_ReturnsFalseForNonMatchingPath()
    {
        var options = BuildOptions("/remote");
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);
        var ctx = BuildContext("/images/foo.png");

        Assert.False(provider.IsValidRequest(ctx));
    }

    [Fact]
    public void IsValidRequest_VerifyUrl_ReturnsTrueWhenRemoteReturns200()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Head, "https://example.com/image.png")
                .Respond(HttpStatusCode.OK);

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>()))
               .Returns(new HttpClient(mockHttp));

        var options = BuildOptions("/remote", allowAll: true, verifyUrl: true, userAgent: Guid.NewGuid().ToString());
        var provider = BuildProvider(factory.Object, options);
        var ctx = BuildContext("/remote/https://example.com/image.png");

        Assert.True(provider.IsValidRequest(ctx));
    }

    [Fact]
    public void IsValidRequest_VerifyUrl_ReturnsFalseWhenRemoteReturns404()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Head, "https://example.com/image.png")
                .Respond(HttpStatusCode.NotFound);

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>()))
               .Returns(new HttpClient(mockHttp));

        var options = BuildOptions("/remote", allowAll: true, verifyUrl: true, userAgent: Guid.NewGuid().ToString());
        var provider = BuildProvider(factory.Object, options);
        var ctx = BuildContext("/remote/https://example.com/image.png");

        Assert.False(provider.IsValidRequest(ctx));
    }

    // ---------------------------------------------------------------------------
    // GetAsync
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task GetAsync_ReturnsResolverForMatchingPath()
    {
        var options = BuildOptions("/remote");
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);
        var ctx = BuildContext("/remote/https://example.com/image.png");

        var result = await provider.GetAsync(ctx);

        Assert.NotNull(result);
        Assert.IsType<RemoteImageResolver>(result);
    }

    [Fact]
    public async Task GetAsync_ReturnsNullForNonMatchingPath()
    {
        var options = BuildOptions("/remote");
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);
        var ctx = BuildContext("/images/foo.png");

        var result = await provider.GetAsync(ctx);

        Assert.Null(result);
    }

    // ---------------------------------------------------------------------------
    // ProcessingBehavior
    // ---------------------------------------------------------------------------

    [Fact]
    public void ProcessingBehavior_IsAll()
    {
        var options = BuildOptions("/remote");
        var provider = BuildProvider(Mock.Of<IHttpClientFactory>(), options);

        Assert.Equal(SixLabors.ImageSharp.Web.Providers.ProcessingBehavior.All, provider.ProcessingBehavior);
    }
}
