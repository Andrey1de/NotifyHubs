using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace FeedsShared.Controllers
{
    public class Notification
    {
        public Notification()
        {

        }

        public Notification(string type, object body)
        {
            ID = 0;
            JBody = JsonConvert.SerializeObject(body);

            Body = JsonConvert.DeserializeObject(JBody);
            Type = type;
            Created = DateTime.Now;
        }


        public bool Compare(Notification that)
        {
            if (that.Type != Type) return false;
            if (that.JBody == JBody) return true;
            return true;
        }



        public int ID { get; set; } = 0;
        public string Type { get; set; } = "";

        public object Body { get; private set; } = new object();
        public string JBody { get; private set; } = "{}";
        public DateTime Created { get; set; } = DateTime.Now;
    }


}