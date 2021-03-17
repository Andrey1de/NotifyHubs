//using AJP;
//using Newtonsoft.Json;
using System;
using System.Text.Json;
namespace FeedsBL.Models
{
    public class  NotificationADO
    {

        private static readonly JsonSerializerOptions JSO = Notification.JSO;

        public Guid Uid { get; private set; }
        public string Type { get; private set; }
        public string JMessage { get; private set; }
        public string Text { get; private set; }
        public string FileName { get;  set; }

          public DateTime Created { get; private set; }
        public int NumberOfWords { get; private set; }
        public bool IsFacebook => Type == Notification.FaceBook;
        public bool IsTwitter => Type == Notification.Twitter;
        public readonly string RandomString;

        private object _body;
        
        public object GetBody()
        {
            return _body;
        }

        public NotificationADO(Notification note)
        {
             Uid = note.Uid;
            RandomString = Convert.ToBase64String(Uid.ToByteArray()).Substring(0, 10).ToLower();

            JMessage = note.JMessage;
             Type = note.Type;
             _body = note.Body;

             JsonDocument doc = JsonDocument.Parse(JMessage);
             JsonElement root = doc.RootElement;
              if (note.IsTwitter)
             {
                root = root.GetProperty("data");
                Text =  root.GetProperty("text").ToString();
             }
            else if (note.IsFacebook)
            {
                Text =  root.GetProperty("message").ToString();

             }
            else
            {
                throw new ApplicationException($"Type {Type} isn't appropriate");

            }


            var arr = Text.Split(" \t\r\n".ToCharArray(),
              StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            NumberOfWords = arr.Length;

            Created = note.Created;
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

        public override int GetHashCode()
        {
            return Type.GetHashCode() + JMessage.GetHashCode();
        }
    }
   
}