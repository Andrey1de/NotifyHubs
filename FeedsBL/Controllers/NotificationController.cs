
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using  FeedsBL;
using Microsoft.Extensions.Logging;
using FeedsBL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FeedsBL.Controllers
{

 
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        readonly IDataService Dal;

        readonly ILogger<NotificationController> Log;

        public NotificationController(IConfiguration config, IDataService dataService,
             ILogger<NotificationController> log)
        {
            Log = log;
            Dal = dataService;
         }

      
        [HttpGet]
        [Route("list")]
        public ActionResult<object[]> List()
        {
            var ret = Dal.List().Select(p=>p.GetBody()).ToArray();
            return Ok(ret);
         }

        // GET api/<NotificationController>/5
        [HttpGet()]
        [Route("{id}")]
        public ActionResult<object> Get (Guid guid)
        {
            NotificationADO notify = Dal.Get(guid);

            if (notify != null)
            {
                return Ok(notify.JMessage);
            }
            return NotFound(guid);
        }

     //   static object _lockInsert = new object();
        // POST api/<NotificationController>
        [HttpPost]
        [Route("insert/{type}")]
        public ActionResult<object> Insert(string type, [FromBody] object body)
        {
         //   lock (_lockInsert)
           // {
                NotificationADO notify = null;
                try
                {
                    Dal.TryInsert(type, body, out notify);
                    return Ok(notify.JMessage);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex.StackTrace);
                return this.NoContent();
                   // throw ex;
                }
         //   }
     
        }



        // DELETE api/<NotificationController>/5
        [HttpDelete()]
        [Route("delete/{id}")]
        public ActionResult Delete(string guid)
        {
            Guid uid = new Guid(guid);

            if (Dal.Remove(uid))
            {
                Log.LogInformation($"Delete {uid.ToString()} OK");

                return Ok();
            }
            Log.LogInformation($"Delete {uid.ToString()} FAILED");

            return NotFound();
        }
    }
}
