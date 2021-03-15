using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AndreyCurrenclyShared.Models;
using AndreyCurrenclyShared.Services;
using AndreyCurrenclyShared.Text;
using System.Reflection;
using System;

namespace AndreyYahooService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YahooCurrencyRatiosController :
        ControllerBase
    {

        private readonly ILogger<YahooCurrencyRatiosController> Logger;
        private readonly ICurrencyConverterService ConvSvc;

        public YahooCurrencyRatiosController(
            ILogger<YahooCurrencyRatiosController> logger,
            ICurrencyConverterService _converter
            )
        {
            Logger = logger;
            ConvSvc = _converter;
        }

        [Route("name")]
        [HttpGet]
        public ActionResult<string> GetName()
        {
   
            return Ok(ConvSvc.GetConvertorName());
        }

        [Route("pair/{from}/{to}")]
        [HttpGet]
        public async Task<ActionResult<CurrencyRatioADO>> ConvertPair(string from, string to)
        {
            return await ConvSvc.GetRatioForPair(from, to);
        }

        [Route("delimited/{delim}")]
        [HttpGet]
        public async Task<ActionResult<List<CurrencyRatioADO>>> GetDelimited(
            string delim)
        {
            List<CurrencyRatioADO> _listOut = new List<CurrencyRatioADO>();
            List<FromTo> listFromTo = delim.SplitDelimFromTo("-/").Where(p => p.IsValid).ToList();
            try
            {
                if (listFromTo.Count == 0)
                {
                    return _listOut;
                }

                List<Task<CurrencyRatioADO>> _listTasks = listFromTo
                    .Select(pair => ConvSvc.GetRatioForPair(pair.From, pair.To))
                    .ToList();



                if (_listTasks.Count > 0)
                {

                    _ = await Task.WhenAll<CurrencyRatioADO>(_listTasks.ToArray());
                    _listTasks.ForEach(res =>
                    {
                        if (res.IsCompleted && res.Result.IsValid())
                            _listOut.Add(res.Result);
                    });


                }


            }
            catch (Exception ex)
            {

                Logger.LogError(ex.Message);
                return this.NotFound(_listOut);
            }           
            
            return Ok(_listOut);
        }


        [Route("convertorName")]
        [HttpGet]
        public  ActionResult<string> ServiceName(
               string delim)
        {
            var name =  ConvSvc.GetConvertorName();
  
            return Ok(name);
        }




        //[HttpGet]
        //[Route("default")]
        //public async Task<ActionResult<string>> GetDefault()
        //{
        //    return Ok(ConvSvc.DefaultCurrencyPairs);
        //}
    }
}
