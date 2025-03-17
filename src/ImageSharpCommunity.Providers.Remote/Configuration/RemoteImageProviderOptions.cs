namespace ImageSharpCommunity.Providers.Remote.Configuration;
public class RemoteImageProviderOptions
{
    /// <summary>
    /// A list of settings for remote image providers. Here you define your url prefixes, and which domains are allowed to fetch images from.
    /// </summary>
    public List<RemoteImageProviderSetting> Settings { get; set; } = new List<RemoteImageProviderSetting>();

    /// <summary>
    /// Fallback max age for the image. If the server does not return a cache-control header, this value is used.
    /// </summary>
    public TimeSpan FallbackMaxAge { get; set; } = TimeSpan.FromHours(1);
}
