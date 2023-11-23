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
using GiacenzaSorterRm.AppCode;
using GiacenzaSorterRm.Models.Database;
using System;

namespace GiacenzaSorterRm
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //debug
            //services.AddDbContext<GiacenzaSorterRmTestContext>(options => options.UseSqlServer(MyConnections.GiacenzaSorterRmContext).EnableSensitiveDataLogging());
            services.AddDbContext<GiacenzaSorterRmTestContext>(options => options.UseSqlServer(MyConnections.GiacenzaSorterRmContext));
           

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
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
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
                    //await context.HttpContext.Response.WriteAsync(
                    //    "Status code page, status code: " +
                    //    context.HttpContext.Response.StatusCode);

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
                       context.User.Identity.Name != null && context.User.FindFirst("Azienda").Value == "POSTEL" ));


            });

            //services.AddMvc().AddSessionStateTempDataProvider();
            //services.AddSession();
            

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
            //app.UseSession();
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
