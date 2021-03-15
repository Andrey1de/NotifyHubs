using AndreyCurrenclyShared.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace AndreyCurrenclyShared.Models
{
    public class RatioEventADO
    {
        string pair { get; set; }
        double ratio { get; set; }
        double oldratio { get; set; }
   
        DateTime updated { get; set; }
        bool IsValid => !pair.IsZ() && ratio > 0 ;


        public static RatioEventADO NewRatio(CurrencyRatioADO ratioAdo)
        {
            var ret = new RatioEventADO()
            {
                pair = ratioAdo.pair,
                oldratio = ratioAdo.oldratio,
                ratio = ratioAdo.ratio,
                updated = DateTime.Now

            };
            return ret;

        }

    }
}
   
