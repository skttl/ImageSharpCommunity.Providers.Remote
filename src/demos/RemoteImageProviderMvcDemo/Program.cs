using ImageSharpCommunity.Providers.Remote;
using ImageSharpCommunity.Providers.Remote.Configuration;
using SixLabors.ImageSharp.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddHttpClient()
    .Configure<RemoteImageProviderOptions>(options =>
    {
        options.Settings
            .Add(new("/ourumb")
            {
                RemoteUrlPrefix = "https://our.umbraco.com/",
                AllowedDomains = new List<string>() { "our.umbraco.com" }
            });

        options.Settings
            .Add(new("/remote")
            {
                AllowedDomains = new List<string>() { "*" }
            });
    })
    .AddImageSharp()
    // needs to insert it at position 0, because it needs to go before WebRootProvider
    .InsertProvider<RemoteImageProvider>(0);


var app = builder.Build();

app.UseImageSharp();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
