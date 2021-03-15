using AndreyCurrenclyShared.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyCurrencyBL.HubConfig
{
    public interface IRatioCallback
    {
        [HubMethodName("changeRatios")]
        Task ChangeRatios(List<CurrencyRatioADO> listCallb);

        [HubMethodName("send")]
        Task SendAsyncNew(string str);

    }

    public class RartioHub : Hub<IRatioCallback>
    {
        // readonly ILogger Logger;
        ConsoleColor defaultColor;
       
        public RartioHub()//ILogger _logger)
        {
            defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("HUB:RartioHub()");
            Console.ForegroundColor = defaultColor;


        }
           
       
        public override Task OnConnectedAsync()
        {
            var name = Context.GetHttpContext().Request.Query["name"];
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"HUB:OnConnectedAsync({name})");
            Console.ForegroundColor = defaultColor;

         
            return Task.Delay(0);// Clients.All.SendAsyncNew( $"{name} joined the chat");
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var name = Context.GetHttpContext().Request.Query["name"];
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"HUB:OnDisconnectedAsync({name})");
            Console.ForegroundColor = defaultColor;

            //Logger.LogTrace($"HUB:OnDisconnectedAsync({name})");

            // return Clients.All.SendAsyncNew( $"{name} left the chat");
            return Task.Delay(0);
        }
    }
}
