using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PortableManager.Web.Client.Infrastructure;
using PortableManager.Web.Client.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortableManager.Web.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();

            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:8081") });

            await builder.Build().RunAsync();
        }
    }
}
