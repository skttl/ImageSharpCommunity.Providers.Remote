using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace RemoteImageProviderForImageSharp
{
    public class RemoteImageProvider : IImageProvider
    {
        public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.All;
        public Func<HttpContext, bool> Match { get; set; } = PathStartsWithRemote;

        public const string RemotePath = "/remote";

        private readonly IHttpClientFactory _clientFactory;
        private readonly IRemoteImageUrlValidator _urlValidator;

        public RemoteImageProvider(IHttpClientFactory clientFactory, IRemoteImageUrlValidator urlValidator)
        {
            _clientFactory = clientFactory;
            _urlValidator = urlValidator;
        }

        public bool IsValidRequest(HttpContext context)
        {
            var url = GetRemoteUrl(context);
            return Uri.IsWellFormedUriString(url, UriKind.Absolute) && _urlValidator.IsValidUrl(new Uri(url));
        }

        public Task<IImageResolver?> GetAsync(HttpContext context)
        {
            var url = GetRemoteUrl(context);
            return Task.FromResult(url is null ? null : new RemoteImageResolver(_clientFactory, url) as IImageResolver);
        }

        private static bool PathStartsWithRemote(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(RemotePath, StringComparison.OrdinalIgnoreCase);
        }

        private static string? GetRemoteUrl(HttpContext context)
        {
            if (!context.Request.Path.HasValue || context.Request.Path.Value.Length <= RemotePath.Length)
            {
                return null;
            }
            var remoteUrl = context.Request.Path.Value?.Substring(RemotePath.Length + 1);
            return remoteUrl?.Replace(" ", "%20");
        }
    }
}
