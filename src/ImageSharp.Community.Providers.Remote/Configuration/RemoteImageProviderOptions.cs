namespace ImageSharp.Community.Providers.Remote.Configuration;
public class RemoteImageProviderOptions
{
    /// <summary>
    /// A list of settings for remote image providers. Here you define your url prefixes, and which domains are allowed to fetch images from.
    /// </summary>
    public List<RemoteImageProviderSetting> Settings { get; set; } = new List<RemoteImageProviderSetting>();
}