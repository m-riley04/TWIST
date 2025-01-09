using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using TWISTServer.Controllers;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.Hubs
{
    public interface IRoomClient
    {
        Task ParticipantJoined(int id, string username, string email);
    }

    public class RoomHub : Hub<IRoomClient>
    {
        SimulationsDataAccessor simAccessor = new();
        ParticipantsDataAccessor partAccessor = new();
        public async Task JoinRoom(string code, string username, string email) // FYI: Methods like this will SILENTLY FAIL if you do not pass the correct param types
        {
            /// TODO: Check if the email address already exists
            
            // Get simulation
            List<SimulationRecord> sims = simAccessor.GetByCode(code).ToList();

            if (sims.Count == 0)
            {
                throw new HubException("Simulation not found");
            }

            SimulationRecord sim = sims.First();

            /// Add the participant to participants table
            ParticipantRecord participant = new ParticipantRecord(0, null, null, sim.SimulationId, username, email);
            int participantId = partAccessor.InsertAndReturnId(participant);

            // Add participant to simulation record
            sim.Participants.Append(participantId);
            simAccessor.UpdateParticipants(sim.SimulationId, sim.Participants);

            // Send signal to all clients
            await Clients.All.ParticipantJoined(participantId, email, username);
        }
    }
}