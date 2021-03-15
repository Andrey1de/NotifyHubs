using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyCurrencyBL
{
    /// <summary>
    /// Creates Host with using appsettings.json
    /// </summary>
    public class Program
    {

        /// <summary>
        /// The settings json file name
        /// </summary>
        public const string AppsettingsJson = "appsettings.json";
        /// <summary>
        ///  parameter name in settings file
        /// </summary>
        public const string ApplicationUrl = "ApplicationUrl";
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var applicationUrls = GetAppUrls(ApplicationUrl,
                    AppsettingsJson
                );

            var host = Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();

                        if (applicationUrls.Length > 0)
                        {
                            webBuilder.UseUrls(applicationUrls);

                        }

                    });
            return host;
        }
        /// <summary>
        /// Get Urls from appsettings.json or another config files
        /// </summary>
        /// <param name="applicationUrl"> parameter name in settings file </param>
        /// <param name="appsettingsJson">name file of settings</param>
        /// <returns>array of urls</returns>
        private static string[] GetAppUrls(
            string applicationUrl = "ApplicationUrl",
            string appsettingsJson = "appsettings.json")
        {
            var _confgig0 = new ConfigurationBuilder()
                          .AddJsonFile("appsettings.json")
                         .Build(); ;


            string strUrls = _confgig0.GetValue<string>("ApplicationUrl") ?? "";
            string[] urls = strUrls.Split(",".ToCharArray(),
                    System.StringSplitOptions.RemoveEmptyEntries);
            return urls;
        }
    }
}
