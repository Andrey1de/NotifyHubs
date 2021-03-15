using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using FeedsBL.Models;
namespace FeedsBL.Models
{
    public class Notification
    {
        const string Facebook = "Facebook";
        const string Twitter = "Twitter";
        public readonly Guid Uid;
        public readonly string RandomString;
        public static readonly JsonSerializerOptions JSO;

        static Notification()
        {
            JSO = new JsonSerializerOptions() { WriteIndented = true };
        }
        public Notification()
        {
            Uid = Guid.NewGuid();
            RandomString = Convert.ToBase64String(Uid.ToByteArray()).Substring(0, 8);
        }
        public Notification(string type, object body)
        {
             type = Twitter;
            if (type.StartsWith("twi",StringComparison.InvariantCultureIgnoreCase))
            {
            }
            else if (type.StartsWith("face", StringComparison.InvariantCultureIgnoreCase))
            {
                type = Facebook;
            }
          
            ID = 0;
            JBody = JsonSerializer.Serialize(body,JSO);

            Body = JsonSerializer.Deserialize<object>(JBody);

            Type = type;
            Created = DateTime.Now;
        }

        public bool IsFacebook => Type == Facebook;
        public bool IsTwitter => Type == Twitter;

        public string FileName { get; set; }
        public bool Compare(Notification that)
        {
            if (that.Type != Type) return false;
            if (that.JBody == JBody) return true;
            return true;
        }

        public override string ToString()
        {
            return JBody;
        }

        public int ID { get; set; } = 0;
        public string Type { get; set; } = "";

        public object Body { get; private set; } = new object();
        public string JBody { get; private set; } = "{}";
        public DateTime Created { get; set; } = DateTime.Now;
    }


}