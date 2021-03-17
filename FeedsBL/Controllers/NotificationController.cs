
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
   /// <summary>
   /// Tramsport class used for serialization
   /// </summary>
    public class RetWithGuid
    {
        public Guid guid { get; set; } = Guid.Empty;
        public object body { get; set; }
        public RetWithGuid()
        {

        }
        public RetWithGuid(NotificationADO note)
        {
            guid = note.Uid;
            body = note.GetBody();
        }
    }

    /// <summary>
    /// Main controller to operate with Notification storage
    /// </summary>
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

        /// <summary>
        /// Retrieves the list of the existing objects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public ActionResult<RetWithGuid[]> List()
        {
            var ret = Dal.List().Select(p => new RetWithGuid(p))
                    .ToArray();
            return Ok(ret);
        }

        /// <summary>
        /// Retrieves the object with attached guidS
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        // GET api/Notification/guid
        [HttpGet()]
        [Route("{id}")]
        public ActionResult<RetWithGuid> Get(Guid guid)
        {
            NotificationADO notify = Dal.Get(guid);

            if (notify != null)
            {
                return Ok(new RetWithGuid(notify));
            }
            return NotFound($"Object {guid.ToString()} not been stored ");
        }

    
        /// <summary>
        /// Returns Guid if Inserded success otherwise error 409 and Empty Guid
        /// </summary>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        // POST api/<NotificationController>
        [HttpPost]
        [Route("insert/{type}")]
        public ActionResult<RetWithGuid> Insert(string type, [FromBody] object body)
        {
         //   lock (_lockInsert)
           // {
                NotificationADO notify = null;
                try
                {
                    string err = Dal.TryInsert(type, body, out notify);
                    if (string.IsNullOrEmpty(err))
                    {
                        return Ok(new RetWithGuid(notify));
                    }
                    return NotFound(err);
                    //new RetWithGuid() { guid = Guid.Empty, body = body });
                 }
                catch (Exception ex)
                {
                    Log.LogError(ex.StackTrace);
                    return this.NotFound($"Error occures {ex.StackTrace}");
                    
               
                }
         //   }
     
        }


        /// <summary>
        /// Deletes the selected object by Guid
        /// If Guid == Guid.Empty - cleares all the store
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        // DELETE api/Notification/guid/
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
