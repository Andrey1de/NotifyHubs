using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedsBL
{
    /// <summary>
    /// Creates Host with using appsettings.json
    /// </summary>
    public class Program
    {

        /// <summary>
        /// The settings json file name
        /// </summary>
        private const string _appsettingsJson = "appsettings.json";
        /// <summary>
        ///  parameter name in settings file
        /// </summary>
        private const string _applicationUrl = "ApplicationUrl";

        public static string[] ApplicationUrls { get; private set; }
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfigurationBuilder config0 = null;
            ApplicationUrls = GetAppUrls(_applicationUrl,
                    _appsettingsJson); 

            var host = Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();

                        if (ApplicationUrls.Length > 0)
                        {
                            webBuilder.UseUrls(ApplicationUrls);

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
            string appsettingsJson = "appsettings.json"
            )
        {
             var config0 = new ConfigurationBuilder()
                          .AddJsonFile("appsettings.json")
                         .Build(); ;


            string strUrls = config0.GetValue<string>("ApplicationUrl") ?? "";
            string[] urls = strUrls.Split(",".ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries);
            return urls;
        }
    }

}
