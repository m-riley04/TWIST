using Microsoft.AspNetCore.SignalR;

namespace TWISTServer.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(long username, string message);
    }

    public class ChatHub : Hub<IChatClient>
    {
        public async Task NewMessage(long username, string message) =>
            await Clients.All.ReceiveMessage(username, message);
    }
}