using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using TWISTServer.Controllers;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

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
        public async Task JoinRoom(SimulationRecord sim, string username, string email) // FYI: Methods like this will SILENTLY FAIL if you do not pass the correct param types
        {
            // Check if the email address does not already exist in sim
            var _ = partAccessor.GetParticipantFromSimulationAndEmail(sim.SimulationId, email);
            ParticipantRecord newParticipant;
            if (_.Count() <= 0)
            {
                // Add the participant to participants table
                newParticipant = new ParticipantRecord(0, CountryEnum.None, ParticipantRoleEnum.None, sim.SimulationId, username, email);
                int participantId = partAccessor.InsertAndReturnId(newParticipant);

                // Add participant to simulation record
                var newParticipants = sim.Participants.Append(participantId);
                simAccessor.UpdateParticipants(sim.SimulationId, newParticipants);
            }

            newParticipant = partAccessor.GetParticipantFromSimulationAndEmail(sim.SimulationId, email).First();

            // Add the participant to the main group
            await Groups.AddToGroupAsync(Context.ConnectionId, sim.Code);

            // Add the participant to default country
            string teamName = $"{sim.Code}_{newParticipant.Country}";
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);

            // Send signal to all clients
            await Clients.All.ParticipantJoined(newParticipant);
        }

        public async Task LeaveRoom(SimulationRecord sim, ParticipantRecord participant)
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
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);

            // Send signal to all in simulation
            await Clients.Group(sim.Code).ParticipantLeft(participant.ParticipantId, participant.Email, participant.Username);
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

        public async Task ChangeParticipantRole(string code, ParticipantRecord request)
        {
            throw new NotImplementedException();
        }

        public async Task ChangeParticipantCountry(string code, ParticipantRecord request)
        {

            throw new NotImplementedException();
        }
    }
}