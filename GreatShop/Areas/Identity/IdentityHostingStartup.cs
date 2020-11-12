using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(GreatShop.Areas.Identity.IdentityHostingStartup))]
namespace GreatShop.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}