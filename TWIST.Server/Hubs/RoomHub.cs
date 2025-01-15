using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
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
        Task ParticipantUpdated(ParticipantRecord record);
        Task SimulationStarted(SimulationRecord sim);
        Task SimulationStopped();
        Task SimulationUpdated(SimulationRecord sim);
        Task RoundUpdated(RoundEnum round);
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
                newParticipant = new ParticipantRecord(0, CountryEnum.NONE, ParticipantRoleEnum.NONE, sim.SimulationId, username, email, Context.ConnectionId);
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

            // Get simulation
            var sim = simAccessor.Get(participant.SimulationId).FirstOrDefault();
            if (sim == null)
            {
                return base.OnDisconnectedAsync(exception);
            }

            return Clients.Group(sim.Code).ParticipantDisconnected(participant);
        }

        public async Task LeaveRoom(SimulationRecord sim, ParticipantRecord participant)
        {
            // Remove participant from simulation record
            var newParticipants = sim.Participants.Where(p => p != participant.ParticipantId);
            simAccessor.UpdateParticipants(sim.SimulationId, newParticipants);

            // Remove the participant from main group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sim.Code);

            // Remove the participant from team
            string teamName = $"{sim.Code}_{participant.Country}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);

            // Send signal to all in simulation
            await Clients.Group(sim.Code).ParticipantLeft(participant);
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
            await Clients.Group(sim.Code).ParticipantKicked(participant);
        }

        public async Task UpdateParticipantRole(SimulationRecord sim, ParticipantRecord participant, ParticipantRoleEnum newRole)
        {
            // Update the participant's role
            partAccessor.UpdateParticipantRole(participant.ParticipantId, newRole);

            // Send signal
            await Clients.Group(sim.Code).ParticipantUpdated(participant);
        }

        public async Task UpdateParticipantCountry(SimulationRecord sim, ParticipantRecord participant, CountryEnum newCountry)
        {
            // Update database
            partAccessor.UpdateParticipantCountry(participant.ParticipantId, newCountry);

            // Remove the participant from current team
            string teamName = $"{sim.Code}_{participant.Country}";
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);

            // Add the participant to new team
            teamName = $"{sim.Code}_{newCountry}";
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);

            // Send signal
            await Clients.Group(sim.Code).ParticipantUpdated(participant);
        }

        // TODO: Implement these methods
        public async Task RandomlyAssignCountry(SimulationRecord sim, ParticipantRecord participant)
        {
            // Get all participants
            var participants = partAccessor.GetParticipantFromSimulationAndEmail(sim.SimulationId, participant.Email).ToList();
            
            // Get all countries
            var countries = Enum.GetValues<CountryEnum>().ToList();
            
            // Remove the participant's current country
            countries.Remove(participant.Country ?? 0);
            
            // Remove the participant from their current team
            string teamName = $"{sim.Code}_{participant.Country}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
            
            // Assign a random country
            var random = new Random();
            var newCountry = countries[random.Next(countries.Count)];
            
            // Update the participant's country
            partAccessor.UpdateParticipantCountry(participant.ParticipantId, newCountry);
            
            // Add the participant to new team
            teamName = $"{sim.Code}_{newCountry}";
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
            
            // Send signal
            await Clients.Group(sim.Code).ParticipantUpdated(participant);
        }

        // TODO: Implement these methods
        public async Task RandomlyAssignRole(SimulationRecord sim, ParticipantRecord participant)
        {
            // Get all participants
            var participants = partAccessor.GetParticipantFromSimulationAndEmail(sim.SimulationId, participant.Email).ToList();

            // Get all roles
            var roles = Enum.GetValues<ParticipantRoleEnum>().ToList();

            // Remove the participant's current role
            roles.Remove(participant.Role ?? 0);

            // Assign a random role
            var random = new Random();
            var newRole = roles[random.Next(roles.Count)];

            // Update the participant's role
            partAccessor.UpdateParticipantRole(participant.ParticipantId, newRole);

            // Send signal
            await Clients.Group(sim.Code).ParticipantUpdated(participant);
        }

        // TODO: Implement these methods
        public async Task RandomlyAssignCountryToAll(SimulationRecord sim, ParticipantRecord[] participants)
        {
            foreach (ParticipantRecord p in participants)
            {
                await RandomlyAssignCountry(sim, p);
            }
        }

        // TODO: Implement these methods
        public async Task RandomlyAssignRoleToAll(SimulationRecord sim, ParticipantRecord[] participants)
        {
            foreach (ParticipantRecord p in participants)
            {
                await RandomlyAssignRole(sim, p);
            }
        }

        public async Task StartSimulation(SimulationRecord sim)
        {

            // Signal
            await Clients.Group(sim.Code).SimulationStarted(sim);
        }

        public async Task StopSimulation(SimulationRecord sim)
        {

            // Signal
            await Clients.Group(sim.Code).SimulationStopped();
        }

        public async Task UpdateRound(SimulationRecord sim, RoundEnum round)
        {
            // Update the simulation record
            simAccessor.UpdateRound(sim.SimulationId, round);

            // Signal
            await Clients.Group(sim.Code).RoundUpdated(round);
        }
    }
}