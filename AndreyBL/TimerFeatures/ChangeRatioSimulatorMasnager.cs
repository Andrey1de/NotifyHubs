using AndreyCurrenclyShared.Models;
using AndreyCurrencyBL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyCurrencyBL.TimerFeatures
{
    public static class ChangeRatioSimulatorMasnager
    {
        /// <summary>
        /// Generates new ratio in randomal chosen member
        /// </summary>
        /// <returns></returns>
        public static List<CurrencyRatioADO> GetChanges()
        {
            var r = new Random();
            List<PairsGetTime> list = CentralBLService.AllData;
            List<CurrencyRatioADO> ret = new List<CurrencyRatioADO>();
            if (list.Count > 0)
            {
                int num = r.Next(0, list.Count);

                var  adoOrig = list[num].Ratio;

                var ado = adoOrig.Clone();
                // var ado = list[num].Ratio;

                var koefNew = 1 + ((r.NextDouble() - 0.5) / 10.0);// Change +/- 5% randomaly;
                var percent = (koefNew - 1) * 100.0;// Change +/- 5% randomaly;
                ado.OldRatio = ado.Ratio;
                ado.Ratio *= koefNew; // ratio +/- 10% randomaly;
                ado.Status = 2;

                ado.Ratio = double.Parse(ado.Ratio.ToString("G6"));
               
                ret.Add(ado);
            }



            return ret;
        }
    }
}
