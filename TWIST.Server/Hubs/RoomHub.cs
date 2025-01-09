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
        Task ParticipantJoined(ParticipantRecord record);
        Task ParticipantKicked(string username, string email);
        Task ParticipantLeft(int id, string username, string email);
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

        public async Task KickParticipant(SimulationRecord sim, ParticipantRecord participant)
        {
            // Remove the participant from the participants table
            partAccessor.Delete(participant.ParticipantId);

            // Remove participant from simulation record
            var newParticipants = sim.Participants.Where(p => p != participant.ParticipantId);
            simAccessor.UpdateParticipants(sim.SimulationId, newParticipants);

            // Remove the participant from main group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sim.Code);

            // Remove the participant from team
            string teamName = $"{sim.Code}_{participant.Country}";
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);

            // Send signal to all in simulation
            await Clients.Group(sim.Code).ParticipantKicked(participant.Email, participant.Username);
        }

            /// Add the participant to participants table
            ParticipantRecord participant = new ParticipantRecord(0, null, null, sim.SimulationId, username, email);
            int participantId = partAccessor.InsertAndReturnId(participant);

            // Add participant to simulation record
            var newParticipants = sim.Participants.Append(participantId);
            simAccessor.UpdateParticipants(sim.SimulationId, newParticipants);

            // Send signal to all clients
            await Clients.All.ParticipantJoined(participantId, email, username);
        }
    }
}