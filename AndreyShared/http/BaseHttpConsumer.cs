using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AndreyCurrecyBL.http
{


    public class BaseHttpConsumer : IBaseHttpConsumer
    {
       // protected readonly ILogger<BaseHttpConsumer> Log;
        public readonly HttpClient Client;

        public string Url
        {
            get => Client.BaseAddress.ToString();
            set => Client.BaseAddress = new Uri(value);

        }


        public BaseHttpConsumer(
        //   ILogger<BaseHttpConsumer> logger,
           HttpClient client
          )
        {
           // Log = logger;
            Client = client;// new HttpClient();

        }
        public async Task<string> HttpGet(string url)
        {
            // "/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc");
            using (var response = await Client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                string retString = await response.Content.ReadAsStringAsync();
                return retString;

            }
        }

        public async Task<T> HttpGet<T>(string url)
        {

            string jsonString = await HttpGet(url);

            T ret = JsonSerializer.Deserialize<T>(jsonString);
            return ret;

        }
        public async Task<string> HttpGetRawString(string url)
        {

            string str = await HttpGet(url);

            return str;

        }

        public async Task<string> HttpPost(string url, string payload)
        {
            using (HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json"))
            {
                using (var response = await Client.PostAsync(url, content))
                {
                    response.EnsureSuccessStatusCode();
                    string retString = await response.Content.ReadAsStringAsync();
                    return retString;

                }
            }
        }
        public async Task<string> HttpPost<TPayload>(string url, TPayload payload)

        {

            string jsonPayload = JsonSerializer.Serialize<TPayload>(payload);
            string jsonString = await HttpPost(url, jsonPayload);
            return jsonString;
        }
        public async Task<TReturn> HttpPost<TPayload, TReturn>(string url, TPayload payload)

        {

            string jsonString = await HttpPost<TPayload>(url, payload);

            string jsonRet = JsonSerializer.Serialize<TPayload>(payload);
            TReturn ret = JsonSerializer.Deserialize<TReturn>(jsonRet);
            return ret;
        }

    }
}
