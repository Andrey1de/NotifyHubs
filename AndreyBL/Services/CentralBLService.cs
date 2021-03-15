using AndreyCurrecyBL.Services;
using AndreyCurrenclyShared.Models;
using AndreyCurrenclyShared.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyCurrencyBL.Services
{
    public class PairsGetTime
    {
        public CurrencyRatioADO Ratio { get; set; } = null;
        public DateTime? Touched { get; set; } = null;

    }

    public interface ICentralBLService
    {
        string DefaultCurrencyPairs { get; init; }
        TimeSpan MaxReadDelay { get; init; }
        string Url { get; init; }

        Task<string> GetConvertorName();
        Task<CurrencyRatioADO> GetRatioForPair(string from, string to);
        Task<List<CurrencyRatioADO>> GetRatioForPairs(FromTo[] pairs, string delimiter = ",");
    }

    public class CentralBLService : ICentralBLService
    {
        public string DefaultCurrencyPairs { get; init; }
    
        public TimeSpan MaxReadDelay { get; init; }// TimeSpan.FromMilliseconds()
        public readonly int MaxReadDelayMsec;//{ get => (int)MaxReadDelay.TotalMilliseconds }// TimeSpan.FromMilliseconds()
        public string Url { get; init; }// TimeSpan.FromMilliseconds()

        private readonly ILogger<CentralBLService> Log;

        private static readonly ConcurrentDictionary<string, PairsGetTime> DictPairsGet ;
        private readonly ICurrencyRatiosHTTPConsumer Consumer;

        public static List<PairsGetTime> AllData { get => DictPairsGet.Values.ToList(); }

        static CentralBLService()
        {
            DictPairsGet =
                new ConcurrentDictionary<string, PairsGetTime>();
        }

        public CentralBLService(ILogger<CentralBLService> logger
                               , IConfiguration config
                               , ICurrencyRatiosHTTPConsumer consumer
            )
        {
            Log = logger;
            Consumer = consumer;
            MaxReadDelayMsec = config.GetValue<int>("MaxReadDelaySec") * 1000;
            MaxReadDelay = TimeSpan.FromMilliseconds(MaxReadDelayMsec);
            // TimeSpan.FromSeconds(1200),//TBD get this value from config 
            DefaultCurrencyPairs = (config.GetValue<string>("DefaultCurrencyPairs")
                                    ?? "USD/ILS,GBP/EUR,EUR/JPY,EUR/USD")
                                    .ClearWhiteSpaces().Replace('/', '-');


        }
        public async Task<string> GetConvertorName()
        {
            return await Consumer.GetConvertorName();
        }

        
        readonly string Provider = "Yahoo";
        Func<PairsGetTime, int> getSpan = (PairsGetTime pgt) =>
         (int)(DateTime.Now - pgt.Touched.Value).TotalMilliseconds;


        public async Task<CurrencyRatioADO> GetRatioForPair(string from, string to)

        {

            from = from.Trim();
            to = to.Trim();
            int spanMsec;
            var key =   (from + "-" + to).ToUpper();

            if (!DictPairsGet.TryGetValue(key, out PairsGetTime pgt)
               || pgt.Ratio != null || pgt.Ratio.IsValid()
               || (spanMsec = getSpan(pgt)) >= MaxReadDelayMsec)
            {
                var pairGet = await Consumer.ConvertPair(from, to);
                if (!pairGet.IsValid())
                {
                    Log.LogWarning($"Impossible currency conversion  pair {key}  ");
                    return null;
                }

                pgt = new PairsGetTime() { Ratio = pairGet, Touched = DateTime.Now };
                DictPairsGet.TryAdd(key, pgt);
                Log.LogInformation($"Provider{Provider}.ConvertPair({key},ratio{pgt.Ratio.Ratio}) ");

            }


            return pgt.Ratio;
        }

   

        public async Task<List<CurrencyRatioADO>> GetRatioForPairs(FromTo[] pairs, string delimiter = ",")
        {


            List<CurrencyRatioADO> ret = new List<CurrencyRatioADO>();
            if (pairs == null || pairs.Length == 0)
                return ret;
            
            int spanMsec;
            StringBuilder sb = new StringBuilder();
            foreach (FromTo pair0 in pairs)
            {
                PairsGetTime pgt = null;
                string key = pair0.Pair.ToUpper();
                bool b;

                if ((b = DictPairsGet.TryGetValue(key, out pgt))
                   && pgt.Ratio != null && pgt.Ratio.IsValid()
                   && (spanMsec = getSpan(pgt)) < MaxReadDelayMsec)
                {
                    //Build delimited string
                    var ratio = pgt.Ratio;
                    ret.Add(ratio);
                    Log.LogInformation($"Provider{Provider}.FromHash({ratio.Pair},ratio={ratio.Ratio}) ");
     
                }
                else
                {
                    if (sb.Length > 1) sb.Append(',');
                    sb.Append(pair0.Pair);
                }

            }
            

            if (sb.Length > 1)
            {
                List<CurrencyRatioADO> listGet = await Consumer.GetDelimited(sb.ToString());
                Log.LogInformation($"Provider{Provider}.GetDelimited({sb}) ");

                listGet.ForEach(ratio => {

                    string key = ratio.Pair.ToUpper().Replace("/", "-"); ;

                    if (ratio.IsValid())
                    {
                        DictPairsGet.TryAdd(key, new PairsGetTime() { Ratio = ratio, Touched = DateTime.Now });
                        Log.LogInformation($"Provider{Provider}.GetDelimited returns({ratio.Pair},ratio={ratio.Ratio}) ");
                        ret.Add(ratio);
                    }
                    else
                    {
                        Log.LogWarning($"Impossible currency conversion  pair {key}  ");

                    }

                });

            }

            return ret;
        }
    }
}
