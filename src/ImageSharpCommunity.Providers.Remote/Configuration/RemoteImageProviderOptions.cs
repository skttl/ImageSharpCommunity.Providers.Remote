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

    /// <summary>
    /// Query string parameter name used as a marker to separate remote pass-through parameters (before the marker)
    /// from local ImageSharp processing parameters (after the marker). Set to null or empty to disable.
    /// Default is "_iscpr".
    /// </summary>
    public string? QueryParameterMarker { get; set; } = "_iscpr";
}
