using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using GiacenzaSorterRm.Models.Database;
using System;

namespace GiacenzaSorterRm
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Database Configuration - SQL Server only
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("PLACEHOLDER"))
            {
                throw new InvalidOperationException(
                    "Connection string non configurata. " +
                    "Configura 'ConnectionStrings:DefaultConnection' in User Secrets (dev) o Environment Variables (prod).");
            }
            
            // Registra il context SQL Server
            services.AddDbContext<GiacenzaSorterContext>(options =>
                options.UseSqlServer(connectionString)
                       .EnableSensitiveDataLogging(Environment.IsDevelopment()));

            // Memory Cache per rate limiting e lockout
            services.AddMemoryCache();

            // Configurazione Active Directory Settings
            services.Configure<GiacenzaSorterRm.Models.ActiveDirectorySettings>(
                Configuration.GetSection("ActiveDirectory"));

            // Configurazione Authentication Settings
            services.Configure<GiacenzaSorterRm.Models.AuthenticationSettings>(
                Configuration.GetSection("Authentication"));

            // Registrazione servizi di autenticazione
            services.AddScoped<GiacenzaSorterRm.Services.IAuthenticationService, GiacenzaSorterRm.Services.AuthenticationService>();
            
            // Registrazione condizionale del servizio Active Directory
            if (Environment.EnvironmentName == "LocalDev")
            {
                // In ambiente LocalDev usa implementazione mock senza AD
                services.AddScoped<GiacenzaSorterRm.Services.IActiveDirectoryService, GiacenzaSorterRm.Services.MockActiveDirectoryService>();
            }
            else
            {
                // In altri ambienti usa implementazione reale con AD
                services.AddScoped<GiacenzaSorterRm.Services.IActiveDirectoryService, GiacenzaSorterRm.Services.ActiveDirectoryService>();
            }

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddControllers().AddNewtonsoftJson();

            CultureInfo[] supportedCultures = new[]
            {
                new CultureInfo("it-IT"),
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("it-IT");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new RouteDataRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddBrowserDetection();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = "/";
                cookieOptions.LogoutPath = "/Logout/Index";

                cookieOptions.Events.OnRedirectToAccessDenied = async context =>
                {
                    context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MaximunRequirements", policy =>
                   policy.RequireAssertion(context =>
                       context.User.Identity.Name != null && (context.User.IsInRole("ADMIN") || context.User.IsInRole("SUPERVISOR"))));

                options.AddPolicy("NormalizzazioneRequirements", policy =>
                   policy.RequireAssertion(context =>
                       context.User.Identity.Name != null && (context.User.FindFirst("Azienda").Value == "ESTERNO" || (context.User.IsInRole("ADMIN") || context.User.IsInRole("SUPERVISOR")))));

                options.AddPolicy("SorterRequirements", policy =>
                   policy.RequireAssertion(context =>
                       context.User.Identity.Name != null && context.User.FindFirst("Azienda").Value == "POSTEL"));
            });

            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/PagesAssociazione");
                options.Conventions.AuthorizeFolder("/PagesNormalizzato");
                options.Conventions.AuthorizeFolder("/PagesNormalizzazione");
                options.Conventions.AuthorizeFolder("/PagesRiepilogo");
                options.Conventions.AuthorizeFolder("/PagesSorter");
                options.Conventions.AuthorizeFolder("/PagesSorterizzato");
                options.Conventions.AuthorizeFolder("/PagesVolumi");
                options.Conventions.AuthorizeFolder("/TipiContenitori");
                options.Conventions.AuthorizeFolder("/TipiDocumenti");
                options.Conventions.AuthorizeFolder("/TipiLavorazioni");
                options.Conventions.AuthorizeFolder("/TipologiaNormalizzazione");
                options.Conventions.AuthorizeFolder("/PagesSpostaGiacenza");
                options.Conventions.AuthorizeFolder("/PagesMacero");
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict
            };

            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseRequestLocalization();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
