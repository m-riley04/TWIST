using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace TWISTServer.Hubs
{
    public interface IRoomClient
    {
        Task ParticipantJoined(string username, string email);
    }

    public class RoomHub : Hub<IRoomClient>
    {
        public async Task JoinRoom(string code, string username, string email) // FYI: Methods like this will SILENTLY FAIL if you do not pass the correct param types
        {
            /// TODO:  Check if the email address already exists

            /// TODO: Add the participant

            // Send signal to all clients
            await Clients.All.ParticipantJoined(email, username);
        }
    }
}