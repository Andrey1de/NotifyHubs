//using AJP;
//using Newtonsoft.Json;
using System;
using System.Text.Json;
namespace FeedsBL.Models
{
    public class  NotificationADO
    {
         const string Facebook = "Facebook";
         const string Twitter = "Twitter";

        private static readonly JsonSerializerOptions JSO =
             new JsonSerializerOptions() { WriteIndented = true };

        public Guid Uid { get; private set; }
        public string Type { get; private set; }
        public string JMessage { get; private set; }
        public string Text { get; private set; }
        public string FileName { get;  set; }

        public readonly string RandomString;
        public DateTime Created { get; private set; }
        public int NumberOfWords { get; private set; }
        public bool IsFacebook => Type == Facebook;
        public bool IsTwitter => Type == Twitter;

        private object _body;
        
        public object GetBody()
        {
            return _body;
        }

        public NotificationADO(Notification note)
        {
            Uid = note.Uid;
            JMessage = note.JMessage;
            Type = note.Type;
            //_body = note.Body;
            JsonDocument doc = JsonDocument.Parse(JMessage);
            JsonElement root = doc.RootElement;
              if (note.IsTwitter)
            {
                root = root.GetProperty("data");
                Text =  root.GetProperty("text").ToString();
            }
            else
            {
                Text =  root.GetProperty("message").ToString();

            }


            var arr = Text.Split(" \t\r\n".ToCharArray(),
              StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            NumberOfWords = arr.Length;

            Created = note.Created;
        }

        public override string ToString( )
        {
            return JsonSerializer.Serialize<NotificationADO>(this, JSO);
        }
        public  bool Equals(NotificationADO obj)
        {
            bool ret = Type == obj.Type &&  JMessage.Equals(obj.JMessage);
            return ret;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as NotificationADO);
        }

    }
   
}