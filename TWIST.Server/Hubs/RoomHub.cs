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
        Task ParticipantKicked(ParticipantRecord record);
        Task ParticipantLeft(ParticipantRecord record);
        Task ParticipantDisconnected(ParticipantRecord record);
    }

    public class RoomHub : Hub<IRoomClient>
    {
        SimulationsDataAccessor simAccessor = new();
        ParticipantsDataAccessor partAccessor = new();
        public async Task JoinRoom(SimulationRecord sim, string username, string email) // FYI: Methods like this will SILENTLY FAIL if you do not pass the correct param types
        {
            // Check if the email address does not already exist in sim
            var _ = partAccessor.GetParticipantFromSimulationAndEmail(sim.SimulationId, email).SingleOrDefault();
            ParticipantRecord newParticipant;
            if (_ == null)
            {
                // Add the participant to participants table
                newParticipant = new ParticipantRecord(0, CountryEnum.None, ParticipantRoleEnum.None, sim.SimulationId, username, email, Context.ConnectionId);
                int participantId = partAccessor.InsertAndReturnId(newParticipant);
                newParticipant = newParticipant with { ParticipantId = participantId };

                // Add participant to simulation record
                var newParticipants = sim.Participants.Append(participantId);
                simAccessor.UpdateParticipants(sim.SimulationId, newParticipants);
            } 
            else
            {
                // Otherwise, update the participant's connection id
                newParticipant = _ with { ConnectionId = Context.ConnectionId };
                partAccessor.UpdateParticipantConnectionId(_.ParticipantId, Context.ConnectionId);
                newParticipant = _;
            }

            // Add the participant to the main simulation group
            await Groups.AddToGroupAsync(Context.ConnectionId, sim.Code);

            // Add the participant to default country
            string teamName = $"{sim.Code}_{newParticipant.Country}";
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);

            // Send signal to all clients
            await Clients.All.ParticipantJoined(newParticipant);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                return base.OnDisconnectedAsync(exception);
            }

            // Get the participant
            var participant = partAccessor.GetParticipantsByConnectionId(Context.ConnectionId).SingleOrDefault();
            if (participant == null)
            {
                return base.OnDisconnectedAsync(exception);
            }

            // Update the participant's connection id
            partAccessor.UpdateParticipantConnectionId(participant.ParticipantId, null);

            return Clients.Group(participant.SimulationId.ToString()).ParticipantDisconnected(participant);
        }

        public async Task LeaveRoom(SimulationRecord sim, ParticipantRecord participant)
        {
            // Remove participant from simulation record
            var newParticipants = sim.Participants.Where(p => p != participant.ParticipantId);
            simAccessor.UpdateParticipants(sim.SimulationId, newParticipants);

            // Remove the participant from main group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sim.SimulationId.ToString());

            // Remove the participant from team
            string teamName = $"{sim.SimulationId}_{participant.Country}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);

            // Send signal to all in simulation
            await Clients.Group(sim.SimulationId.ToString()).ParticipantLeft(participant);
        }

        public async Task KickParticipant(SimulationRecord sim, ParticipantRecord participant)
        {
            // Remove the participant from the participants table
            partAccessor.Delete(participant.ParticipantId);

            // Remove participant from simulation record
            var newParticipants = sim.Participants.Where(p => p != participant.ParticipantId);
            simAccessor.UpdateParticipants(sim.SimulationId, newParticipants);

            // Remove the participant from main group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sim.SimulationId.ToString());

            // Remove the participant from team
            string teamName = $"{sim.SimulationId}_{participant.Country}";
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);

            // Send signal to all in simulation
            await Clients.Group(sim.SimulationId.ToString()).ParticipantKicked(participant);
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