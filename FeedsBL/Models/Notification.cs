//using AJP;
//using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Text.Json;
namespace FeedsBL.Models
{

    public class Notification
    {
        const string Facebook = "Facebook";
        const string Twitter = "Twitter";
        public readonly Guid Uid = Guid.NewGuid();
        public readonly string RandomString;
        private static readonly JsonSerializerOptions JSO =
                new JsonSerializerOptions() { WriteIndented = true };

        public Notification()
        {
              RandomString = Convert.ToBase64String(Uid.ToByteArray()).Substring(0, 10).ToLower();
        }

     
        public Notification(string type, object body)
            : this()
        {
            Body = body;
            JMessage = JsonSerializer.Serialize(body, JSO);

    
            Type = type;
            Created = DateTime.Now;
        }

        public bool IsFacebook => Type == Facebook;
        public bool IsTwitter => Type == Twitter;

        public string FileName { get; set; }
        public bool Compare(Notification that)
        {
            if (that.Type != Type) return false;
            if (that.JMessage == JMessage) 
                return true;
            return true;
        }

        public override string ToString()
        {
            return JMessage;
        }

        public string Type { get; set; } = "";

      public object Body { get; private set; } 
        public string JMessage { get; private set; } = "{}";
        public DateTime Created { get; set; } = DateTime.Now;
    }
   
}