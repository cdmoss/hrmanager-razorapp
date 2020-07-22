using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Hangfire;
using Hangfire.MySql.Core;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Org.BouncyCastle.Asn1.Cms;
using Hangfire.Common;
using Google.Protobuf.WellKnownTypes;
using AutoMapper;
using MHFoodBank.Web.Repositories;
using MHFoodBank.Common;

namespace MHFoodBank.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FoodBankContext>(options =>
            {
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, IdentityRole<int>>(options =>
                    options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<FoodBankContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddHangfire(options => options
                .UseSerializerSettings(new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore})
                .UseStorage(
                    new MySqlStorage(
                        Configuration.GetConnectionString("HangfireConnection"),
                        new MySqlStorageOptions
                        {
                            TransactionIsolationLevel = System.Data.IsolationLevel.ReadCommitted,
                            QueuePollInterval = TimeSpan.FromSeconds(15),
                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
                            PrepareSchemaIfNecessary = true,
                            DashboardJobListLimit = 50000,
                            TransactionTimeout = TimeSpan.FromMinutes(1)
                        }
                    )));

            services.AddHangfireServer();
            services.AddScoped<IReminderManager, ReminderManager>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IClockedTimeRepo, ClockedTimeRepo>();

            services
                .AddRazorPages()
                .AddRazorPagesOptions(options =>
                    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", ""))
                .AddNewtonsoftJson();

            services
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, FoodBankContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // seed the database with everything
            if (DbSeeder.SeedRoles(roleManager))
            {
                DbSeeder.SeedAdmin(userManager);
                DbSeeder.SeedTestVolunteer(userManager, context);
                DbSeeder.SeedPositions(context);
                DbSeeder.SeedStaff(userManager);
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
