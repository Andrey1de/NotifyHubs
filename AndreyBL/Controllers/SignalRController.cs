using AndreyCurrenclyShared.Models;
using AndreyCurrencyBL.HubConfig;
using AndreyCurrencyBL.TimerFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AndreyCurrencyBL.Controllers
{
    [ApiController]
    // [Route("signalr/[controller]")]
    [Route("api/signalr")]
    
    public class SignalRController : ControllerBase
    {
        public IHubContext<RartioHub, IRatioCallback> HubCont;

        public SignalRController(IHubContext<RartioHub, IRatioCallback>  _hubCont)
        {
            HubCont = _hubCont;
        }
        
        [HttpGet]
        [Route("testchange")]
        public async Task<ActionResult> GetTestChange()
        {
            List<CurrencyRatioADO> data = ChangeRatioSimulatorMasnager.GetChanges();


            await HubCont.Clients.All.ChangeRatios(data);

            var res = new JsonResult(data);
            
            var json = JsonConvert.SerializeObject(data,Formatting.Indented) ;
            Console.WriteLine("GetTestChange()");
            Console.WriteLine(json);

            return Ok(new { Message = "Request Completed" });
        }
    }
}
