using AndreyCurrenclyShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndreyCurrenclyShared.Services
{
    public interface ICurrencyConverterService
    {
        string DefaultCurrencyPairs { get; }
        string Url { get; }// TimeSpan.FromMilliseconds()

        Task<CurrencyRatioADO> GetRatioForPair(string from, string to);
        string GetConvertorName();
    }

}
