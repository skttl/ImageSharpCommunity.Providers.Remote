using ImageSharpCommunity.Providers.Remote.Configuration;
using SixLabors.ImageSharp.Web.Resolvers;
namespace ImageSharpCommunity.Providers.Remote;

public class RemoteImageResolver : IImageResolver
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _url;
    private readonly RemoteImageProviderSetting _setting;

    public RemoteImageResolver(IHttpClientFactory clientFactory, string url, RemoteImageProviderSetting setting)
    {
        _clientFactory = clientFactory;
        _url = url;
        _setting = setting;
    }

    public async Task<ImageMetadata> GetMetaDataAsync()
    {
        var client = _clientFactory.GetRemoteImageProviderHttpClient(_setting);

        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, _url), HttpCompletionOption.ResponseHeadersRead);

        if (!response.Content.Headers.ContentLength.HasValue)
        {
            throw new Exception("Required header ContentLength is missing.");
        }

        if (!response.Content.Headers.LastModified.HasValue || !response.Content.Headers.ContentLength.HasValue)
        {
            throw new Exception("Required header LastModified is missing.");
        }

        return new ImageMetadata(response.Content.Headers.LastModified.Value.UtcDateTime, response.Content.Headers.ContentLength.Value);
    }

    public async Task<Stream> OpenReadAsync()
    {
        var client = _clientFactory.GetRemoteImageProviderHttpClient(_setting);

        var response = await client.GetAsync(_url);

        return await response.Content.ReadAsStreamAsync();
    }
}
