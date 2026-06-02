using ImageSharpCommunity.Providers.Remote.Configuration;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace ImageSharpCommunity.Providers.Remote.Tests;

public class HelpersTests
{
    private static RemoteImageProviderOptions BuildOptions(string prefix, string? remoteUrlPrefix = null, bool allowAll = true, List<string>? allowedDomains = null)
    {
        return new RemoteImageProviderOptions
        {
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting(prefix)
                {
                    RemoteUrlPrefix = remoteUrlPrefix,
                    AllowAllDomains = allowAll,
                    AllowedDomains = allowedDomains ?? new List<string>(),
                }
            }
        };
    }

    // ---------------------------------------------------------------------------
    // IsMatchingRemoteImageProviderSetting
    // ---------------------------------------------------------------------------

    [Fact]
    public void IsMatchingRemoteImageProviderSetting_ReturnsTrueForMatchingPrefix()
    {
        var setting = new RemoteImageProviderSetting("/remote");
        PathString path = "/remote/https://example.com/image.png";

        Assert.True(path.IsMatchingRemoteImageProviderSetting(setting));
    }

    [Fact]
    public void IsMatchingRemoteImageProviderSetting_ReturnsFalseForNonMatchingPrefix()
    {
        var setting = new RemoteImageProviderSetting("/remote");
        PathString path = "/images/foo.png";

        Assert.False(path.IsMatchingRemoteImageProviderSetting(setting));
    }

    [Fact]
    public void IsMatchingRemoteImageProviderSetting_IsCaseInsensitive()
    {
        var setting = new RemoteImageProviderSetting("/remote");
        PathString path = "/REMOTE/image.png";

        Assert.True(path.IsMatchingRemoteImageProviderSetting(setting));
    }

    // ---------------------------------------------------------------------------
    // GetMatchingRemoteImageProviderSetting (PathString overload)
    // ---------------------------------------------------------------------------

    [Fact]
    public void GetMatchingRemoteImageProviderSetting_ReturnsSettingForMatchingPath()
    {
        var options = BuildOptions("/remote");
        PathString path = "/remote/https://example.com/image.png";

        var result = path.GetMatchingRemoteImageProviderSetting(options);

        Assert.NotNull(result);
        Assert.Equal("/remote", result!.Prefix);
    }

    [Fact]
    public void GetMatchingRemoteImageProviderSetting_ReturnsNullForNonMatchingPath()
    {
        var options = BuildOptions("/remote");
        PathString path = "/images/foo.png";

        var result = path.GetMatchingRemoteImageProviderSetting(options);

        Assert.Null(result);
    }

    // ---------------------------------------------------------------------------
    // GetMatchingRemoteImageProviderSetting (Uri overload)
    // ---------------------------------------------------------------------------

    [Fact]
    public void GetMatchingRemoteImageProviderSetting_Uri_ReturnsSettingWhenAllowAllDomains()
    {
        var options = BuildOptions("/remote", allowAll: true);
        var uri = new Uri("https://example.com/image.png");

        var result = uri.GetMatchingRemoteImageProviderSetting(options);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetMatchingRemoteImageProviderSetting_Uri_ReturnsNullWhenDomainNotAllowed()
    {
        var options = BuildOptions("/remote", allowAll: false, allowedDomains: new List<string> { "allowed.com" });
        var uri = new Uri("https://notallowed.com/image.png");

        var result = uri.GetMatchingRemoteImageProviderSetting(options);

        Assert.Null(result);
    }

    // ---------------------------------------------------------------------------
    // GetSourceUrlForRemoteImageProviderUrl
    // ---------------------------------------------------------------------------

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_ReturnsRemoteUrlForFullUrl()
    {
        var options = BuildOptions("/remote");
        PathString path = "/remote/https://example.com/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options);

        Assert.Equal("https://example.com/image.png", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_PrefixesWithRemoteUrlPrefix()
    {
        var options = BuildOptions("/remote", remoteUrlPrefix: "https://cdn.example.com/");
        PathString path = "/remote/images/photo.jpg";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options);

        Assert.Equal("https://cdn.example.com/images/photo.jpg", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_ReturnsNullForNonMatchingPath()
    {
        var options = BuildOptions("/remote");
        PathString path = "/images/photo.jpg";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options);

        Assert.Null(result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_EncodesSpaces()
    {
        var options = BuildOptions("/remote");
        PathString path = "/remote/https://example.com/my image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options);

        Assert.Equal("https://example.com/my%20image.png", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_PassesThroughParameters_WhenMatchingQueryKey()
    {
        var options = new RemoteImageProviderOptions
        {
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "width" },
                }
            }
        };

        PathString path = "/remote/https://example.com/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options, new QueryString("?width=800"));

        Assert.Contains("width=800", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_DoesNotAppendParameters_WhenNoMatchingQueryKey()
    {
        var options = new RemoteImageProviderOptions
        {
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "width" },
                }
            }
        };

        PathString path = "/remote/https://example.com/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options, new QueryString("?format=webp"));

        Assert.Equal("https://example.com/image.png", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_PassesThroughAllParameters_WhenPassThroughAllParametersIsTrue()
    {
        var options = new RemoteImageProviderOptions
        {
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughAllParameters = true,
                }
            }
        };

        PathString path = "/remote/https://example.com/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options, new QueryString("?id=123&format=webp"));

        Assert.Contains("id=123", result);
        Assert.Contains("format=webp", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_PassesThroughNoParameters_WhenEmptyListAndPassThroughAllParametersIsFalse()
    {
        var options = new RemoteImageProviderOptions
        {
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughAllParameters = false,
                    PassThroughParameters = new List<string>(),
                }
            }
        };

        PathString path = "/remote/https://example.com/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options, new QueryString("?id=123&format=webp"));

        Assert.Equal("https://example.com/image.png", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_Marker_OnlyPassesThroughParamsBeforeMarker()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "id", "format" },
                }
            }
        };

        PathString path = "/remote/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options, new QueryString("?id=123&format=webp&_iscpr&width=400&format=jpg"));

        Assert.Contains("id=123", result);
        Assert.Contains("format=webp", result);
        Assert.DoesNotContain("width", result);
        Assert.DoesNotContain("format=jpg", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_Marker_PassesThroughAllEligibleParamsWhenNoMarkerPresent()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "format" },
                }
            }
        };

        PathString path = "/remote/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options, new QueryString("?format=webp&width=400"));

        Assert.Contains("format=webp", result);
    }

    [Fact]
    public void GetSourceUrlForRemoteImageProviderUrl_Marker_DisabledWhenMarkerIsNull()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = null,
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "format" },
                }
            }
        };

        PathString path = "/remote/image.png";

        var result = path.GetSourceUrlForRemoteImageProviderUrl(options, new QueryString("?_iscpr&format=webp"));

        Assert.Contains("format=webp", result);
    }

    // ---------------------------------------------------------------------------
    // IsValidForSetting
    // ---------------------------------------------------------------------------

    [Fact]
    public void IsValidForSetting_ReturnsTrueWhenAllowAllDomains()
    {
        var setting = new RemoteImageProviderSetting("/remote") { AllowAllDomains = true };
        var uri = new Uri("https://anydomain.com/image.png");

        Assert.True(uri.IsValidForSetting(setting));
    }

    [Fact]
    public void IsValidForSetting_ReturnsTrueWhenWildcardInAllowedDomains()
    {
        var setting = new RemoteImageProviderSetting("/remote")
        {
            AllowedDomains = new List<string> { "*" }
        };
        var uri = new Uri("https://anydomain.com/image.png");

        Assert.True(uri.IsValidForSetting(setting));
    }

    [Fact]
    public void IsValidForSetting_ReturnsTrueForExactDomainMatch()
    {
        var setting = new RemoteImageProviderSetting("/remote")
        {
            AllowedDomains = new List<string> { "example.com" }
        };
        var uri = new Uri("https://example.com/image.png");

        Assert.True(uri.IsValidForSetting(setting));
    }

    [Fact]
    public void IsValidForSetting_ReturnsFalseWhenDomainNotInList()
    {
        var setting = new RemoteImageProviderSetting("/remote")
        {
            AllowedDomains = new List<string> { "example.com" }
        };
        var uri = new Uri("https://other.com/image.png");

        Assert.False(uri.IsValidForSetting(setting));
    }

    [Fact]
    public void IsValidForSetting_ReturnsTrueForGlobWildcardSubdomain()
    {
        var setting = new RemoteImageProviderSetting("/remote")
        {
            AllowedDomains = new List<string> { "*.example.com" }
        };
        var uri = new Uri("https://cdn.example.com/image.png");

        Assert.True(uri.IsValidForSetting(setting));
    }

    [Fact]
    public void IsValidForSetting_ReturnsFalseForGlobWildcardWhenRootDomain()
    {
        var setting = new RemoteImageProviderSetting("/remote")
        {
            AllowedDomains = new List<string> { "*.example.com" }
        };
        var uri = new Uri("https://example.com/image.png");

        Assert.False(uri.IsValidForSetting(setting));
    }

    [Fact]
    public void IsValidForSetting_ReturnsTrueForRegexMatch()
    {
        var setting = new RemoteImageProviderSetting("/remote")
        {
            AllowedDomainsRegex = new List<string> { @"^cdn\d+\.example\.com$" }
        };
        var uri = new Uri("https://cdn42.example.com/image.png");

        Assert.True(uri.IsValidForSetting(setting));
    }

    [Fact]
    public void IsValidForSetting_ReturnsFalseForRegexNoMatch()
    {
        var setting = new RemoteImageProviderSetting("/remote")
        {
            AllowedDomainsRegex = new List<string> { @"^cdn\d+\.example\.com$" }
        };
        var uri = new Uri("https://images.example.com/image.png");

        Assert.False(uri.IsValidForSetting(setting));
    }

    // ---------------------------------------------------------------------------
    // GetRemoteImageProviderUrl
    // ---------------------------------------------------------------------------

    [Fact]
    public void GetRemoteImageProviderUrl_ReturnsLocalPathForRemoteUri()
    {
        var options = BuildOptions("/remote", allowAll: true);
        var uri = new Uri("https://example.com/image.png");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.NotNull(result);
        Assert.StartsWith("/remote", result);
        Assert.Contains("image.png", result);
        Assert.Contains("_iscpr", result);
    }

    [Fact]
    public void GetRemoteImageProviderUrl_ReturnsNullWhenNoDomainMatch()
    {
        var options = BuildOptions("/remote", allowAll: false, allowedDomains: new List<string> { "allowed.com" });
        var uri = new Uri("https://notallowed.com/image.png");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.Null(result);
    }

    [Fact]
    public void GetRemoteImageProviderUrl_StripsRemoteUrlPrefixFromPath()
    {
        var options = BuildOptions("/remote", remoteUrlPrefix: "https://cdn.example.com/", allowAll: true);
        var uri = new Uri("https://cdn.example.com/photos/image.png");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.NotNull(result);
        Assert.StartsWith("/remote/photos/image.png", result);
        Assert.Contains("_iscpr", result);
    }

    [Fact]
    public void GetRemoteImageProviderUrl_FiltersQueryParamsByPassThroughParameters()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "id" },
                }
            }
        };
        var uri = new Uri("https://example.com/image.png?id=123&secret=abc");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.Contains("id=123", result);
        Assert.DoesNotContain("secret", result);
        Assert.Contains("_iscpr", result);
        Assert.True(result!.IndexOf("id=123") < result.IndexOf("_iscpr"));
    }

    [Fact]
    public void GetRemoteImageProviderUrl_PassesThroughAllQueryParams_WhenPassThroughAllParametersIsTrue()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughAllParameters = true,
                }
            }
        };
        var uri = new Uri("https://example.com/image.png?id=123&format=webp");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.Contains("id=123", result);
        Assert.Contains("format=webp", result);
        Assert.Contains("_iscpr", result);
        Assert.True(result!.IndexOf("format=webp") < result.IndexOf("_iscpr"));
    }

    [Fact]
    public void GetRemoteImageProviderUrl_OmitsMarker_WhenMarkerIsNull()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = null,
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote") { AllowAllDomains = true }
            }
        };
        var uri = new Uri("https://example.com/image.png");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.Equal("/remote/image.png", result);
    }

    [Fact]
    public void GetRemoteImageProviderUrl_StripsAllQueryParams_WhenNoneAllowed()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "id" },
                }
            }
        };
        var uri = new Uri("https://example.com/image.png?width=400&format=jpg");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.StartsWith("/remote/image.png?_iscpr", result);
        Assert.DoesNotContain("width", result);
        Assert.DoesNotContain("format", result);
    }

    [Fact]
    public void GetRemoteImageProviderUrl_StripsAllQueryParams_WhenPassThroughParametersIsEmpty()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string>(),
                }
            }
        };
        var uri = new Uri("https://example.com/image.png?id=123&format=webp");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.StartsWith("/remote/image.png?_iscpr", result);
        Assert.DoesNotContain("id", result);
        Assert.DoesNotContain("format", result);
    }

    [Fact]
    public void GetRemoteImageProviderUrl_KeepsOnlyAllowedParams_WhenMixedQueryString()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughParameters = new List<string> { "id", "format" },
                }
            }
        };
        var uri = new Uri("https://example.com/image.png?id=123&secret=abc&format=webp&token=xyz");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.Contains("id=123", result);
        Assert.Contains("format=webp", result);
        Assert.DoesNotContain("secret", result);
        Assert.DoesNotContain("token", result);
        Assert.Contains("_iscpr", result);
        Assert.True(result!.IndexOf("format=webp") < result.IndexOf("_iscpr"));
    }

    [Fact]
    public void GetRemoteImageProviderUrl_AppendsMarkerOnly_WhenNoQueryString()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote") { AllowAllDomains = true }
            }
        };
        var uri = new Uri("https://example.com/image.png");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.Equal("/remote/image.png?_iscpr", result);
    }

    [Fact]
    public void GetRemoteImageProviderUrl_AppendsMarkerAfterParams_WhenPassThroughAllParametersAndQueryString()
    {
        var options = new RemoteImageProviderOptions
        {
            QueryParameterMarker = "_iscpr",
            Settings = new List<RemoteImageProviderSetting>
            {
                new RemoteImageProviderSetting("/remote")
                {
                    AllowAllDomains = true,
                    PassThroughAllParameters = true,
                }
            }
        };
        var uri = new Uri("https://example.com/image.png?id=123");

        var result = uri.GetRemoteImageProviderUrl(options);

        Assert.Equal("/remote/image.png?id=123&_iscpr", result);
    }

    // ---------------------------------------------------------------------------
    // RemoteImageProviderSetting constructor normalises prefix
    // ---------------------------------------------------------------------------

    [Fact]
    public void RemoteImageProviderSetting_PrefixWithoutLeadingSlash_GetsSlashPrepended()
    {
        var setting = new RemoteImageProviderSetting("remote");

        Assert.Equal("/remote", setting.Prefix);
    }

    [Fact]
    public void RemoteImageProviderSetting_PrefixWithLeadingSlash_RemainsUnchanged()
    {
        var setting = new RemoteImageProviderSetting("/remote");

        Assert.Equal("/remote", setting.Prefix);
    }
}
