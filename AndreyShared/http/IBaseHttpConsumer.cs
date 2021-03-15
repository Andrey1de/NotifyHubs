using System.Threading.Tasks;

namespace AndreyCurrecyBL.http
{
    public interface IBaseHttpConsumer
    {
        string Url { get; set; }

        Task<string> HttpGet(string url);
        Task<T> HttpGet<T>(string url);
         Task<string> HttpGetRawString(string url);
  


        Task<string> HttpPost(string url, string payload);
        Task<TReturn> HttpPost<TPayload, TReturn>(string url, TPayload payload);
        Task<string> HttpPost<TPayload>(string url, TPayload payload);
    }
}