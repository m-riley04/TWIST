using Microsoft.AspNetCore.SignalR;

namespace TWISTServer.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string username, string message);
    }

    public class ChatHub : Hub<IChatClient>
    {
        public async Task NewMessage(string username, string message) =>
            await Clients.All.ReceiveMessage(username, message);
    }
}