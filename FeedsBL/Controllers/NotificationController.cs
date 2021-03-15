
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
            var ret = Dal.List().Select(p=>p.Body).ToArray();
            return Ok(ret);
         }

        // GET api/<NotificationController>/5
        [HttpGet()]
        [Route("{id}")]
        public ActionResult<object> Get (int id)
        {
            Notification notify = Dal.Get(id);

            if (notify != null)
            {
                return Ok(notify.Body);
            }
            return NotFound(id);
        }

        // POST api/<NotificationController>
        [HttpPost]
        [Route("insert/{type}")]
        public ActionResult<object> Insert(string type, [FromBody] object body)
        {
            Notification notify = null;
            try
            {
                Dal.TryInsert(type, body, out notify);
                return Ok(notify.Body);
           }
            catch (Exception ex)
            {
                Log.LogError(ex.StackTrace);
                throw ex;
            }
        }



        // DELETE api/<NotificationController>/5
        [HttpDelete()]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            if (Dal.Remove(id))
            {
                Log.LogInformation($"Delete {id} OK");

                return Ok();
            }
            Log.LogInformation($"Delete {id} FAILED");

            return NotFound();  
        }
    }
}
