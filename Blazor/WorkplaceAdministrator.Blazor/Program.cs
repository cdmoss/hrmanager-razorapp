using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Syncfusion.Blazor;
using WorkplaceAdministrator.Blazor.Repositories;

namespace WorkplaceAdministrator.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjc4MzcxQDMxMzgyZTMxMmUzMFMzU1R1Q2NRT1pRM1prL25XSGI5MmR1UUpMMlc1a1Z2UUZjdDJLRUxwTDQ9;Mjc4MzcyQDMxMzgyZTMxMmUzMFRYdUZFNEEwZ3QyVnBqZFd3cGlzcXJtWTZsclZ1UUxmWDcxcU9acklJSDg9;Mjc4MzczQDMxMzgyZTMxMmUzMFRqNnMxb3FmaC8xdXdKUlFOQ01GRWJwSXhyTHlGc1Q2SEVDcDliTllvbzA9");
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<AuthenticationStateProvider, WorkplaceAuthStateProvider>();
            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IShiftRepo, ShiftRepo>();
            builder.Services.AddScoped<IPositionRepo, PositionRepo>();
            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddSyncfusionBlazor();

            await builder.Build().RunAsync();
        }
    }
}