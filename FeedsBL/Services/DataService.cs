﻿using FeedsBL.Models;
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

        public Notification[] List();
        public Notification Get(int id);
        public bool TryInsert(string type, object jbody, out Notification outNote);
        public bool Remove(int id);
    


    }


    public class DataService : IDataService
    {
        TimeSpan? maxTimeTo = null;
        readonly ILogger<DataService> Log;
        readonly string BaseStoreDirectory; 


        readonly ConcurrentDictionary<int, Notification> dictionary = new ConcurrentDictionary<int, Notification>();
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
            if (!maxTimeTo.HasValue)
            {
                maxTimeTo = TimeSpan.FromSeconds(config.GetValue<int>("MaxTimeTo"));
            }
        }

        public Notification[] List()
        {
            var ret = dictionary.Values.ToArray() ;
            return ret;
        }

        public Notification Get(int id)
        {
            Notification value = null;

            if (dictionary.TryGetValue(id, out value))
            {
                return value;
            }
            return null;
        }

        public  bool TryInsert(string type, object body, out  Notification outNote)
        {
            type = ("" + type).Trim().ToLower();
            outNote = null;
            if (!type.StartsWith("twi") && !type.StartsWith("face"))
            {
                 return false;
            }


            Notification newNote = new Notification(type,body);
            //Get All with creation time bigger than Now - 60 sec
            IEnumerable<Notification> lastValues = dictionary.Values
                  .Where(p => p.Created >= DateTime.Now - TimeSpan.FromSeconds(60));
            //If exists same notify
            Notification sameValue = lastValues.FirstOrDefault(p => newNote.Compare(p));
            if (sameValue != default(Notification))
            {
                 outNote = sameValue;
                Log.LogInformation($"Notify Retrieved:{outNote.FileName}  :");
                Log.LogInformation($"{outNote.JBody}");
                return false; ;
            }
            newNote.ID = (dictionary.Count > 0) ? (dictionary.Keys.Max() + 1) : 1 ;
            while(!dictionary.TryAdd(newNote.ID, newNote))
            {
                newNote.ID++;
            }
            outNote = newNote;
            StoreNotification(newNote);
            return true;

        }
      
        private void StoreNotification(Notification notify)
        {
            try
            {
                string base0 = Path.Combine(BaseStoreDirectory, notify.Type);
                if (!Directory.Exists(base0)){
                    Directory.CreateDirectory(base0);
                }

                string[] dirs =  Directory.GetDirectories(base0);
                int idDir = 0;
                if(dirs != null && dirs.Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        string[] arrr = dir.Split("\\/".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        idDir = Math.Max(int.Parse(arrr[arrr.Length - 1]), idDir);
                    }
                }
                idDir++;
                string saveDir = Path.Combine(base0, idDir.ToString("D4"));
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                notify.FileName = Path.Combine(saveDir,notify.RandomString + ".notification.json");
     
                File.WriteAllText(notify.FileName, notify.JBody);
                Log.LogInformation($"Notify Stored:{notify.FileName} stored :");
                Log.LogInformation($"{notify.JBody}");

            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool Remove(int id)
        {
            Notification note;
            return dictionary.TryRemove(id, out note);
        }

    }


}
