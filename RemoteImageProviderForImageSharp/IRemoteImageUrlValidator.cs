namespace RemoteImageProviderForImageSharp
{
    public interface IRemoteImageUrlValidator
    {
        bool IsValidUrl(Uri url);
    }
}
