using AndreyCurrenclyShared.Models;
using AndreyCurrenclyShared.Text;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AndreyCurrenclyShared.Models

{
    /// <summary>
    /// Class to be used in WebApi requests transport
    /// Cause names  are lower cased
    /// </summary>
    public class CurrencyRatioADO
    {

       // [Newtonsoft.Json.JsonProperty("pair")]
        [JsonPropertyName("pair")]
        public string Pair { get; set; } = "";

        [JsonPropertyName("ratio")]
        public double Ratio { get; set; } = -1;


        [JsonPropertyName("oldRatio")]
        public double OldRatio { get; set; } = -1;

        [JsonPropertyName("percent")]
        public double Percent
        {
            get
            {
                double del = 0.0;
                if (Ratio > 0 && OldRatio > 0)
                {
                    del = 100.0 * (1 - (OldRatio / Ratio));
                }
                return double.Parse(del.ToString("G4"));
            }
        }

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; } = new DateTime(1800, 1, 1);

        [JsonPropertyName("status")]
        public int Status { get; set; } = 0;

        public bool IsValid()
        {
            return !Pair.IsZ() && Ratio > 0 && Status > 0;
        }

        public CurrencyRatioADO Clone()
        {
            return this.MemberwiseClone() as CurrencyRatioADO;
        }

        public static CurrencyRatioADO FromObject(object o)
        {
            CurrencyRatioADO ado = new CurrencyRatioADO();
            
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(o))
            {
                var name = propertyDescriptor.Name.ToLower();
                var strValue = propertyDescriptor.GetValue(o).ToString();
                try
                {
                    switch (name)
                    {

                        case "pair":
                            ado.Pair = strValue;
                            break;
                        case "ratio":
                            ado.Ratio = double.Parse(strValue);
                            break;
                        case "oldratio":
                            ado.OldRatio = double.Parse(strValue);
                            break;
                        case "updated":
                            ado.Updated = DateTime.Parse(strValue);
                            break;
                        case "status":
                            ado.Status = int.Parse(strValue);
                            break;

                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"CurrencyRatioADO FromObject name={name} value={strValue} error \n" +
                        ex.StackTrace);
                }
            }
            return ado;

        }
    }
}
