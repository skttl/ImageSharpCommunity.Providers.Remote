namespace RemoteImageProviderForImageSharp
{
    public class AllowedDomainsRemoteImageUrlValidatorOptions
    {
        public ICollection<string> AllowedDomains { get; set; } = new List<string>();
    }
}
