//using AJP;
//using Newtonsoft.Json;
using System;
using System.Text.Json;
namespace FeedsBL.Models
{
    public class NotificationSummary
    {
        private static readonly JsonSerializerOptions JSO =
                 Notification.JSO;
        public Guid Uid { get; private set; }
        public int NumberOfWords { get; private set; }
        public DateTime Created { get; private set; }

        public NotificationSummary(NotificationADO note)
        {
            Uid = note.Uid;
          NumberOfWords = note.NumberOfWords;
            Created = note.Created;

        }
        public override string ToString()
        {
            return JsonSerializer.Serialize<NotificationSummary>(this, JSO);
        }

    }
   
}