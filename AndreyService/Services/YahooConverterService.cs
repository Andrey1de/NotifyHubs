using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AndreyCurrenclyShared.Models;
using AndreyCurrenclyShared.Services;
using AndreyCurrenclyShared.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace AndreyYahooService.Services
{

   
    class PairsGetTime
    {
        public CurrencyRatioADO Ratio { get; set; }
        public DateTime? Touched { get; set; } = null;

    }
    public class YahooConverterService : ICurrencyConverterService
    {
        public string DefaultCurrencyPairs { get; init; }

        public int MaxReadDelayMsec { get; init; }// TimeSpan.FromMilliseconds()
        public string Url { get; init; }// TimeSpan.FromMilliseconds()

        private readonly ILogger<YahooConverterService> Log;
        HttpClient Client;


        static ConcurrentDictionary<string, PairsGetTime> _dict =
                new ConcurrentDictionary<string, PairsGetTime>();
   
        public YahooConverterService(ILogger<YahooConverterService> logger
                                      ,IConfiguration config
                                       //,HttpClient client
                                     )
        {
            //this is singeleton so "static client is appropriate"
            Client = new HttpClient();
             Log = logger;
            MaxReadDelayMsec = config.GetValue<int>("MaxReadDelaySec") * 1000;
            // TimeSpan.FromSeconds(1200),//TBD get this value from config 
            DefaultCurrencyPairs = (config.GetValue<string>("DefaultCurrencyPairs") 
                                    ?? "USD/ILS,GBP/EUR,EUR/JPY,EUR/USD")
                                    .ClearWhiteSpaces().Replace('/','-');
            Url = config.GetValue<string>("URL") ??
                "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&"+
                "from_currency={0}&to_currency={1}&apikey=55Y1508W05UYQN3G";

        }

        /// <summary>
        /// Returns ratio and update time of demanded currency pair
        /// If value has updated less than 2 minutes before
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<CurrencyRatioADO> GetRatioForPair(string from, string to)

        {
            from = from.ToNZ().Trim().ToLower();
            to = to.Trim().ToNZ().ToLower();
            string pair = from + "/" + to;

            DateTime dt0 = DateTime.Now;
            var that = new CurrencyRatioADO();
            that.Pair = pair;
            that.Ratio = -1.0;
            that.Status = -1;
            that.Updated = DateTime.Parse("1800-01-01");
            string jsonBody = "";


            try

            {

                if (string.IsNullOrEmpty(from))
                {
                    Log.LogWarning("from parameteris empty");
                    return that;
                }
                  if (string.IsNullOrEmpty(to))
                {
                    Log.LogWarning("to parameteris empty");
                    return that;
                }

                
  
   

                if (from == to)
                {
                    Log.LogWarning("Input parameteres from:{0} == to:{1} returnad ratio 1", from, to);
                    that.Ratio = 1.0;
                    that.Updated = DateTime.Now;
                    that.Status = 0;
                    return that;
                }
                PairsGetTime pgt = null;

                // If value has updated less than 2 minutes before
                // Return last stored value
                bool b = false;
                if(MaxReadDelayMsec > 0)
                {

                     b = _dict.TryGetValue(pair, out pgt);
                    int del = (b && pgt.Touched.HasValue) ?
                        (int)(DateTime.Now - pgt.Touched.Value).TotalMilliseconds : -1;


                    if (b && del < MaxReadDelayMsec )
                    {
                        //   Log.LogInformation("TouchedTime={0} < maxReadDelay{1}", del, maxReadDelay);

                        return pgt.Ratio;
                    }

                }

               // var client = new HttpClient();
       
                string url = string.Format(Url, from, to);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                using (var response = await Client.SendAsync(request))
                {

                    response.EnsureSuccessStatusCode();
                    jsonBody = await response.Content.ReadAsStringAsync();
                    ;

                    string[] pars1 = new string[] {
                       // "\"4. To_Currency Name\":",
                        "\"5. Exchange Rate\":",
                        "\"6. Last Refreshed\":",
                        "\"7. Time Zone\":" };

                    string[] arrr = jsonBody.Split(pars1, StringSplitOptions.RemoveEmptyEntries);


                    var strRatio = arrr[1].Trim().Split("\"".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                    that.Ratio = Double.Parse(strRatio);
                    var strDate = arrr[2].Trim().Split("\"".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                    that.Updated = DateTime.Parse(strDate);
                    that.Status = 1;

                    _dict.TryAdd(pair, new PairsGetTime() { Ratio = that, Touched = DateTime.Now });
                    Log.LogInformation("{0}\n received in delt = {1} ms\n",
                            jsonBody, (int)(DateTime.Now - dt0).TotalMilliseconds);
                }



            }
            catch (Exception ex)
            {
            
                Log.LogError("jsonBody:\"" + jsonBody + "\"\n" + ex.ToString());
                // throw ex;
            }
            return that;

        }
        static string _name = null;
        public string GetConvertorName()
        {
            return  _name ?? MethodBase.GetCurrentMethod().DeclaringType.Name;

        }
    }
}
