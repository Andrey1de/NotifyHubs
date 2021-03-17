using FeedsBL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FeedsBL
{
    /// <summary>
    /// Singleton to store data
    /// </summary>
    public interface IDataService
    {

        public NotificationADO[] List();
        public NotificationADO Get(Guid guid);
        public string TryInsert(string type, object JMessage, out NotificationADO outNote);
       public bool Remove(Guid guid);

        public int Count { get;  }
        int NumAllWords { get; }
    }


    public class DataService : IDataService
    {
        double? maxTimeToSec = null;
        readonly ILogger<DataService> Log;
        readonly string BaseStoreDirectory; 


        readonly static  ConcurrentDictionary<Guid, NotificationADO> dictionary = 
                            new ConcurrentDictionary<Guid, NotificationADO>();
        public DataService(IConfiguration config,
            ILogger<DataService> log)
        {
            BaseStoreDirectory = config.GetValue<string>("BaseStoreDirectory");
            BaseStoreDirectory = Path.GetFullPath(BaseStoreDirectory);
            if (!Directory.Exists(BaseStoreDirectory))
            {
                Directory.CreateDirectory(BaseStoreDirectory);
            }
            Log = log;
            if (!maxTimeToSec.HasValue)
            {
                maxTimeToSec = config.GetValue<int>("MaxTimeToSec");
            }
        }

        public NotificationADO[] List()
        {
            var ret = dictionary.Values.ToArray() ;
            return ret;
        }
        public int Count => 
                    dictionary.Count;

        public int NumAllWords =>
                dictionary.Sum(p => p.Value.NumberOfWords);

        public NotificationADO Get(Guid guid)
        {
            NotificationADO value = null;

            if (dictionary.TryGetValue(guid, out value))
            {
                return value;
            }
            return null;
        }
       
        /// <summary>
        /// Retuens error description or empty string;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <param name="outNote"></param>
        /// <param name=""></param>
        /// <returns></returns>

        public  string TryInsert(string type, object body, out NotificationADO outNote )
        {
            string ret = "";
            type = ("" + type).Trim().ToLower();
            outNote = null;
            if (!type.StartsWith("twi") && !type.StartsWith("face"))
            {
                 return $"Type {type} is not suitable";
            }


            Notification note = new Notification(type, body);
            NotificationADO newNote = new NotificationADO( note);
            //Get All with creation time bigger than Now - 60 sec
            NotificationADO[] lastValues = dictionary.Values.ToArray();
   
            for (int i = 0; i < lastValues.Length; i++)
            {
                var p = lastValues[i];
                var dat0 = DateTime.Now;
                double delDSec = (dat0 - p.Created).TotalMilliseconds;
                if (newNote.Type == p.Type &&
                     delDSec < maxTimeToSec * 1000 &&
                    newNote.JMessage == p.JMessage  )
                {
                    outNote = p;
                    var secs =(int)(0.5 + delDSec / 1000.0);
                    string err = $"Notify Retrieved:{outNote.FileName}: \nafter creation spawns {secs} sec < {maxTimeToSec}  :";
                    Log.LogWarning(err);
                    Log.LogInformation($"{outNote.JMessage}");

                    return err;
                }
            }
      
            dictionary.TryAdd(newNote.Uid, newNote);
            outNote = newNote;
            if (StoreNotification(newNote)== null) {
                return String.Empty;
            }

            return "Error to store";
        }

        static object _lockTryCreateDir = new object();


        private Exception StoreNotification(NotificationADO notify)
        {
            string saveDir = null;
            try
            {
                  lock (_lockTryCreateDir)
                 {
                   
                    if(!TryCreateDir(notify, out saveDir))
                    {
                        throw new ApplicationException($"TryCreateDir({notify.JMessage} failed");
                    }

                 }
                notify.FileName = Path.Combine(saveDir, notify.RandomString + ".notification.json");

                File.WriteAllText(notify.FileName, notify. JMessage);
                Log.LogInformation($"Notify Stored:{notify.FileName} stored :");
                Log.LogInformation($"{notify.JMessage}");
       
                var notificationSummary = new NotificationSummary(notify);
                var  fileName = notify.FileName.Replace(".json", ".summary.json");
                File.WriteAllText(fileName, notificationSummary.ToString());

            }
            catch (Exception ex)
            {
                Log.LogError($"Error{ex.StackTrace}");
                //Rewind Directory
                if (!string.IsNullOrWhiteSpace(saveDir) &&   Directory.Exists(saveDir))
                {
                    try
                    {
                        Directory.Delete(saveDir, true);
                    }
                    catch (Exception)
                    {

                       //throw;
                    }
                }
                return ex;
            }
            return null;
        }

        static int _numDirTwitter = 0;
        static int _numDirFacebook = 0;
     
        private bool TryCreateDir(NotificationADO notify, out string saveDir)
        {
            string base0 = Path.Combine(BaseStoreDirectory, notify.Type);
            if (!Directory.Exists(base0))
            {
                Directory.CreateDirectory(base0);
            }
            bool isTwitter = notify.IsTwitter;


            ref int idDir = ref _numDirFacebook;
            if (notify.IsTwitter)
            {
                idDir = ref _numDirTwitter;
            }

            if(idDir <= 0)
            {
                string[] dirs = Directory.GetDirectories(base0);
                if (dirs != null && dirs.Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        string[] arrr = dir.Split("\\/".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        idDir = Math.Max(int.Parse(arrr[arrr.Length - 1]), idDir);
                    }
                }

            }
            idDir++;
            saveDir =  Path.Combine(BaseStoreDirectory,notify.Type , idDir.ToString("D4"));
            if (!Directory.Exists(notify.FileName))
            {
                Directory.CreateDirectory(saveDir);// notify.FileName);
                return true;
    
            }
            //Directory just was created;
            return false;
        }

        public bool Remove(Guid guid)
        {
            NotificationADO note;
            return dictionary.TryRemove(guid, out note);
        }

        NotificationADO IDataService.Get(Guid guid)
        {
            throw new NotImplementedException();
        }

     
    }


}
