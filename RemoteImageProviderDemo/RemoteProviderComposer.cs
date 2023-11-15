using ImageSharp.Community.Providers.Remote;
using SixLabors.ImageSharp.Web.DependencyInjection;
using Umbraco.Cms.Core.Composing;

namespace RemoteImageProviderUmbracoDemo
{
    public class RemoteProviderComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {

            builder.Services
                .Configure<RemoteImageProviderOptions>(options =>
                {
                    options.AllowedDomains = new() { "umbraco.com" };

                    options.AdditionalOptions = new List<RemoteImageProviderOptions>()
                    {
                        new ()
                        {
                            Prefix = "/ourumb",
                            RemoteUrlPrefix = "https://our.umbraco.com/",
                            AllowedDomains = new List<string>() { "our.umbraco.com" }
                        },
                        new ()
                        {
                            Prefix = "/allremote",
                            AllowedDomains = new List<string>() { "*" }
                        }
                    };
                })
                .AddImageSharp()
                // needs to insert it at position 0, because it needs to go before WebRootProvider
                .InsertProvider<RemoteImageProvider>(0);

        }
    }
}
