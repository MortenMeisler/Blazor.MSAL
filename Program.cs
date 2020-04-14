using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Blazor.MSAL.Services;

namespace Blazor.MSAL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddMsalAuthentication(options =>
            {
                var authentication = options.ProviderOptions.Authentication;

                // Tenant Id
                authentication.Authority = "https://login.microsoftonline.com/{yourtenantid}";

                // Application / Client Id
                authentication.ClientId = "{yourclientid}";
            });

            builder.Services.AddSingleton<AzureDevOpsClient>();


            await builder.Build().RunAsync();
        }
    }
}
