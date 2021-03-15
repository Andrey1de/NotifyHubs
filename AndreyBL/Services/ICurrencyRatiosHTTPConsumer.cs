using System.Threading.Tasks;
using AndreyCurrenclyShared.Models;
using System.Collections.Generic;

namespace AndreyCurrecyBL.Services
{
    public interface ICurrencyRatiosHTTPConsumer
    {
        Task<CurrencyRatioADO> ConvertPair(string from, string to);
        Task<List<CurrencyRatioADO>> GetDelimited(string delim);
        Task<string> GetConvertorName();
    }
}
