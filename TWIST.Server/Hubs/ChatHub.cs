using Microsoft.AspNetCore.SignalR;

namespace TWISTServer.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string username, string message);
    }

    public class ChatHub : Hub<IChatClient>
    {
        public async Task NewMessage(string username, string message) => // FYI: Methods like this will SILENTLY FAIL if you do not pass the correct param types
            await Clients.All.ReceiveMessage(username, message);
    }
}