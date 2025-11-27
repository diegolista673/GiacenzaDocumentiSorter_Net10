using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using GiacenzaSorterRm.Models.Database;
using GiacenzaSorterRm.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GiacenzaSorterRm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            NLog.LogManager.Setup().LoadConfigurationFromAppSettings();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                
                var host = CreateHostBuilder(args).Build();

                await host.RunAsync();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((context, config) =>
              {
                  var env = context.HostingEnvironment.EnvironmentName;
                  if (env == "LocalDev" || env == "TestDev")
                  {
                      config.AddUserSecrets<Program>();
                  }
              })
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>();
              })
              .ConfigureLogging(logging =>
              {
                  logging.ClearProviders();
                  logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
              })
              .UseNLog();
    }
}
