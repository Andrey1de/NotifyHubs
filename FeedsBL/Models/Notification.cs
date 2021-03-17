//using AJP;
//using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace FeedsBL.Models
{

    public class Notification
    {
        public const string FaceBook = "FaceBook";
        public const string Twitter = "Twitter";
        public readonly Guid Uid = Guid.NewGuid();
           public static readonly JsonSerializerOptions JSO =
                new JsonSerializerOptions() {
                         Encoder = JavaScriptEncoder.Create(
                             UnicodeRanges.BasicLatin, 
                             UnicodeRanges.Cyrillic
                        ),
                    WriteIndented = true
                };
        public bool IsFacebook => Type == FaceBook;
        public bool IsTwitter => Type == Twitter;
        public string FileName { get; set; }
        public string Type { get; set; } = "";
         public object Body { get; private set; }
        public string JMessage { get; private set; } = "{}";
        public DateTime Created { get; set; } = DateTime.Now;


        public Notification()
        {
         }

     
        public Notification(string type, object body)
            : this()
        {
           Type = ("" + type).Trim().ToLower();
            if (Type.StartsWith("twi"))
            {
                Type = Twitter;
            }
            else if (Type.StartsWith("face")) 
            {
                Type = FaceBook; 
            }
            else
            {
                throw new ApplicationException($"Type {type} isn't appropriate");
            }
        
            Body = body;
            JMessage = JsonSerializer.Serialize(body, JSO);
           Created = DateTime.Now;
        }

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

     }
   
}