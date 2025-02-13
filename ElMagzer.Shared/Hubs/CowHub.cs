

using Microsoft.AspNetCore.SignalR;

namespace ElMagzer.Shared.Hubs
{
    public class CowHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        //public async Task SendCowId(string cowsId)
        //{
        //    await Clients.All.SendAsync("ReceiveCowId", cowsId);
        //}
    }
}
