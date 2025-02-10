using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using TWISTServer.Controllers;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

namespace TWISTServer.Hubs
{
    public class ParticipantConnection(int ParticipantId, string ConnectionId)
    {
        public int ParticipantId { get; set; } = ParticipantId;
        public string ConnectionId { get; set; } = ConnectionId;

        public static bool operator <(ParticipantConnection a, ParticipantConnection b)
        {
            return a.ConnectionId.First() < b.ConnectionId.First();
        }

        public static bool operator >(ParticipantConnection a, ParticipantConnection b)
        {
            return a.ConnectionId.First() > b.ConnectionId.First();
        }

        public static bool operator <=(ParticipantConnection a, ParticipantConnection b)
        {
            return a.ConnectionId.First() <= b.ConnectionId.First();
        }

        public static bool operator >=(ParticipantConnection a, ParticipantConnection b)
        {
            return a.ConnectionId.First() >= b.ConnectionId.First();
        }

        public static bool operator ==(ParticipantConnection a, ParticipantConnection b)
        {
            return a.ConnectionId == b.ConnectionId;
        }
        public static bool operator !=(ParticipantConnection a, ParticipantConnection b)
        {
            return a.ConnectionId != b.ConnectionId;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public interface IRoomClient
    {
        Task InstructorInitialized();
        Task ParticipantJoined(ParticipantRecord record);
        Task ParticipantKicked(ParticipantRecord record);
        Task ParticipantLeft(ParticipantRecord record);
        Task ParticipantDisconnected(ParticipantRecord record);
        Task ParticipantUpdated(ParticipantRecord record);
        Task SimulationStarted(SimulationRecord sim);
        Task SimulationStopped(SimulationRecord sim);
        Task SimulationUpdated(SimulationRecord sim);
        Task RolesAssigned(ParticipantRecord[] participants);
        Task CountriesAssigned(ParticipantRecord[] participants);
        Task RoundUpdated(RoundEnum round);
        Task AskUpdated(AskRecord asks);
        Task AsksUpdated(AskRecord[] asks);
        Task ConnectionsPolled(ParticipantConnection[] connections);
        Task GroupsPolled(Dictionary<string, List<ParticipantConnection>> groups);
    }

    public class RoomHub : Hub<IRoomClient>
    {
        SimulationsDataAccessor simAccessor = new();
        ParticipantsDataAccessor partAccessor = new();

        public static ConcurrentDictionary<string, List<ParticipantConnection>> AllGroups = new();
        public static List<ParticipantConnection> AllConnections = []; // TODO: Need to make this not global for ALL simulations (dictionary with simulation code as key)

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Get the participant
            var id = _FindParticipantId(Context.ConnectionId);
            var participant = partAccessor.Get(id).SingleOrDefault();
            if (participant == null)
            {
                return base.OnDisconnectedAsync(exception);
            }

            // Get simulation
            var sim = simAccessor.Get(participant.SimulationId).FirstOrDefault();
            if (sim == null)
            {
                return base.OnDisconnectedAsync(exception);
            }

            _RemoveFromRoom(Context.ConnectionId, sim.Code, participant.Country ?? CountryEnum.NONE);

            Clients.Group(sim.Code).ParticipantDisconnected(participant); // TODO: check if not awaiting this is fine
            return base.OnDisconnectedAsync(exception);
        }

        public async Task InstructorInitialize(SimulationRecord sim)
        {
            // Add the instructor to the main simulation group
            await Groups.AddToGroupAsync(Context.ConnectionId, sim.Code);

            // Send signal
            await Clients.Group(sim.Code).InstructorInitialized();
        }
        public async Task JoinRoom(SimulationRecord sim, string username, string email) // FYI: Methods like this will SILENTLY FAIL if you do not pass the correct param types
        {
            // Check if the email address does not already exist in sim
            var _ = partAccessor.GetParticipantFromSimulationAndEmail(sim.SimulationId, email).SingleOrDefault();
            ParticipantRecord newParticipant;
            if (_ == null)
            {
                // Add the participant to participants table
                newParticipant = new ParticipantRecord(0, CountryEnum.NONE, ParticipantRoleEnum.NONE, sim.SimulationId, username, email);
                int participantId = partAccessor.InsertAndReturnId(newParticipant);
                newParticipant = newParticipant with { ParticipantId = participantId };
            }
            else newParticipant = _;

            // Add the connection to the room
            _AddToRoom(Context.ConnectionId, newParticipant.ParticipantId, sim.Code, newParticipant.Country ?? CountryEnum.NONE);

            // Send signal to all clients
            await Clients.All.ParticipantJoined(newParticipant);
        }

        public async Task LeaveRoom(SimulationRecord sim, ParticipantRecord participant)
        {
            _RemoveFromRoom(Context.ConnectionId, sim.Code, participant.Country ?? CountryEnum.NONE);

            // Send signal to all in simulation
            await Clients.Group(sim.Code).ParticipantLeft(participant);
        }

        public async Task KickParticipant(SimulationRecord sim, ParticipantRecord participant)
        {
            // Remove the participant from the participants table
            //partAccessor.Delete(participant.ParticipantId);

            _RemoveFromRoom(Context.ConnectionId, sim.Code, participant.Country ?? CountryEnum.NONE);

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

            // Check participant connection id
            string connectionId = _FindConnectionId(participant.ParticipantId);
            if (connectionId == null) throw new Exception($"Participant {participant.ParticipantId} does not have a connection id.");
            ParticipantConnection participantConnection = AllConnections.Find(x => x.ParticipantId == participant.ParticipantId);

            // Remove the participant from current team
            string teamName = $"{sim.Code}_{participant.Country}";
            await Groups.RemoveFromGroupAsync(connectionId, teamName);
            AllGroups.AddOrUpdate(teamName, new List<ParticipantConnection> { }, (key, value) => { value.RemoveAll(value => value.ConnectionId == connectionId); return value; });

            // Add the participant to new team
            teamName = $"{sim.Code}_{newCountry}";
            await Groups.AddToGroupAsync(connectionId, teamName);
            AllGroups.AddOrUpdate(teamName, new List<ParticipantConnection> { participantConnection }, (key, value) => { value.Add(participantConnection); return value; });

            // Send signal
            await Clients.Group(sim.Code).ParticipantUpdated(participant);
        }

        // TODO: Implement these methods
        public async Task RandomlyAssignCountries(SimulationRecord sim, ParticipantRecord[] participants)
        {
            List<ParticipantRecord> newParticipants = [];
            foreach (ParticipantRecord participant in participants)
            {
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

                // Update participant record
                ParticipantRecord newParticipant = participant with { Country = newCountry };

                // Add the participant to new team
                teamName = $"{sim.Code}_{newCountry}";
                await Groups.AddToGroupAsync(Context.ConnectionId, teamName);

                // Send signal
                await Clients.Group(sim.Code).ParticipantUpdated(participant);

                // Add participant to group
                newParticipants.Add(newParticipant);
            }

            // Send finished signal
            await Clients.Group(sim.Code).CountriesAssigned(newParticipants.ToArray());
        }

        // TODO: Implement these methods
        public async Task RandomlyAssignRoles(SimulationRecord sim, ParticipantRecord[] participants)
        {
            List<ParticipantRecord> newParticipants = [];
            foreach (ParticipantRecord participant in participants)
            {
                // Get all roles
                var roles = Enum.GetValues<ParticipantRoleEnum>().ToList();

                // Remove the participant's current role
                roles.Remove(participant.Role ?? 0);

                // Assign a random role
                var random = new Random();
                var newRole = roles[random.Next(roles.Count)];

                // Update the participant's role
                partAccessor.UpdateParticipantRole(participant.ParticipantId, newRole);

                // Update participant record
                ParticipantRecord newParticipant = participant with { Role = newRole };

                // Send signal
                await Clients.Group(sim.Code).ParticipantUpdated(participant);

                // Add participant to list
                newParticipants.Add(newParticipant);
            }

            // Send finished signal
            await Clients.Group(sim.Code).RolesAssigned(newParticipants.ToArray());
        }

        public async Task StartSimulation(SimulationRecord sim)
        {
            // Update the simulation record
            simAccessor.UpdateState(sim.SimulationId, SimulationStateEnum.IN_PROGRESS);
            sim = sim with { State = SimulationStateEnum.IN_PROGRESS };

            // Signal
            await Clients.Group(sim.Code).SimulationStarted(sim);
        }

        public async Task StopSimulation(SimulationRecord sim)
        {
            simAccessor.UpdateState(sim.SimulationId, SimulationStateEnum.NONE);
            sim = sim with { State = SimulationStateEnum.NONE };

            // Signal
            await Clients.Group(sim.Code).SimulationStopped(sim);
        }

        public async Task UpdateRound(SimulationRecord sim, RoundEnum round)
        {
            // Update the simulation record
            simAccessor.UpdateRound(sim.SimulationId, round);

            // Signal
            await Clients.Group(sim.Code).RoundUpdated(round);
        }

        public async Task AskUpdated(SimulationRecord sim, ParticipantRecord participant, AskRecord ask)
        {
            // Update the ask


            // Signal
            await Clients.Group($"{sim.Code}_{participant.Country}").AskUpdated(ask);
        }

        public async Task AsksUpdated(SimulationRecord sim, ParticipantRecord participant, AskRecord[] asks)
        {
            // Update the asks


            // Signal
            await Clients.Group($"{sim.Code}_{participant.Country}").AsksUpdated(asks);
        }

        public async Task PollConnections(SimulationRecord sim)
        {
            // Signal
            await Clients.Group(sim.Code).ConnectionsPolled(AllConnections.ToArray());
        }

        public async Task PollGroups(SimulationRecord sim)
        {
            // Signal
            await Clients.Group(sim.Code).GroupsPolled(AllGroups.ToDictionary());
        }

        private async void _RemoveFromRoom(string connectionId, string simulationCode, CountryEnum country)
        {
            string teamName = $"{simulationCode}_{country}";

            // Get the connection
            ParticipantConnection connection = AllConnections.Find(x => x.ConnectionId == connectionId);

            // Remove from SignalR groups
            await Groups.RemoveFromGroupAsync(connectionId, simulationCode);
            await Groups.RemoveFromGroupAsync(connectionId, teamName);

            // Remove from in-memory 
            AllConnections.RemoveAll(value => value.ConnectionId == connectionId);
            foreach (KeyValuePair<string, List<ParticipantConnection>> entry in AllGroups)
            {
                AllGroups.AddOrUpdate(teamName, new List<ParticipantConnection> { }, (key, value) => { value.RemoveAll(value => value.ConnectionId == connectionId); return value; });
            }
        }

        private async void _AddToRoom(string connectionId, int participantId, string simulationCode, CountryEnum country)
        {
            string teamName = $"{simulationCode}_{country}";
            ParticipantConnection newConnection = new(participantId, connectionId);

            // Add to SignalR groups
            await Groups.AddToGroupAsync(connectionId, simulationCode);
            await Groups.AddToGroupAsync(connectionId, teamName);

            // Add to in-memory
            AllConnections.Add(newConnection);
            AllGroups.AddOrUpdate(teamName, new List<ParticipantConnection> { newConnection }, (key, value) => { value.Add(newConnection); return value; });
        }

        private string _FindConnectionId(int participantId)
        {
            return AllConnections.Find(x => x.ParticipantId == participantId)?.ConnectionId ?? "";
        }

        private int _FindParticipantId(string connectionId)
        {
            return AllConnections.Find(x => x.ConnectionId == connectionId)?.ParticipantId ?? 0;
        }
    }
}