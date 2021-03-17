using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FeedsBL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatController : ControllerBase
    {
        readonly IDataService Dal;

        readonly ILogger<StatController> Log;

        public StatController(IConfiguration config, IDataService dataService,
             ILogger<StatController> log)
        {
            Log = log;
            Dal = dataService;

        }

        // GET: api/<StatController>
        [HttpGet]
        [Route("count")]
       public int GetCount()
        {
            return Dal.Count;
        }


        [HttpGet]
        [Route("numAllWords")]
        public int NumAllWords()
        {
            return Dal.NumAllWords;
        }

        //// GET api/<StatController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<StatController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<StatController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<StatController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
