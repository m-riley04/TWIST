import { useAuth0 } from "@auth0/auth0-react";
import { Button, Container } from "react-bootstrap";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import SimulationModel from "../../models/SimulationModel";
import { closeSimulation, getSimulationFromCode } from "../../server/simulation_management";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import ParticipantModel from "../../models/ParticipantModel";
import ParticipantList from "../../components/ParticipantList/ParticipantList";
import { getParticipants } from "../../server/participant_management";

const InstructorSimulationRoomPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect } = useAuth0();
    const [simulation, setSimulation] = useState<SimulationModel>();
    const [connection, setConnection] = useState<HubConnection>();
    const [participants, setParticipants] = useState<ParticipantModel[]>([]);
    const navigate = useNavigate();
    const params = useParams();

    useEffect(() => {
        // Check if code is valid
        if (params.code === undefined) {
            console.error("No code provided.");
            return;
        }

        // Load simulation data
        getSimulationFromCode(params.code)
            .then(data => setSimulation(data))
            .catch(error => console.error(`Unable to load simulation: ${error}`));

        // TODO: Be able to change room settings

        // TODO: Add QR code for participants to join

        // Attempt to connect to the SignalR hub
        const con = new HubConnectionBuilder()
            .withUrl("https://localhost:7026/roomHub")
            .withAutomaticReconnect()
            .build();
        setConnection(con);
    }, []);

    useEffect(() => {
        if (simulation === undefined) return;

        // Get the participants
        getParticipants(simulation.simulation_id)
            .then(data => setParticipants(data))
            .catch(error => console.error(`Unable to load participants: ${error}`));

    }, [simulation])

    useEffect(() => {
        // Check if connection is defined
        if (connection === undefined) return;

        // Start connection
        connection.start()
            .then(() => console.log('Connected to SignalR hub'))
            .catch(err => console.error('Error connecting to hub:', err));

        // Connection signals/slots
        connection.on("ParticipantJoined", (participant: ParticipantModel) => {
            // Add to participants list
            setParticipants((prev) => [...prev, participant]);
            console.log(`New participant joined:`, participant.username);
        });

        connection.on("ParticipantLeft", (participant: ParticipantModel) => {
            // Remove from participants list
            setParticipants((prev) => prev.filter((p) => p.participant_id !== participant.participant_id));
            console.log(`Participant left:`, participant.username);
        });

        connection.on("ParticipantDisconnected", (participant: ParticipantModel) => {
            // Update the participant list
            setParticipants((prev) => prev.map((p) => {
                if (p.participant_id === participant.participant_id) {
                    p.connection_id = participant.connection_id;
                }
                return p;
            }));
        });

    }, [connection]);

    const handleCloseRoom = () => {
        if (params.code === undefined) {
            console.error("Unable to close room: No code provided.")
            return;
        }
        closeSimulation(params.code, new Date())
            .then((response) => {
                // Check if the response failed
                if (response === undefined) {
                    return;
                }

                // Navigate back to instructor home
                navigate("/instructor");
            })
            .catch((error) => {
                console.error(`Unable to close room: ${error}`);
            });
    }

    const handleKickParticipant = (participant: ParticipantModel) => {
        if (connection === undefined) {
            console.error("Unable to kick participant: No connection to hub.");
            return;
        }

        connection.invoke("KickParticipant", simulation, participant)
            .then(() => {
                // Remove participant from list
                setParticipants((prev) => prev.filter((p) => p.email !== participant.email));
                console.log(`Kicked participant '${participant.email}'`);
            })
            .catch((error) => console.error(`Unable to kick participant: ${error}`));
    }

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Simulation Room</h1>
            <h2>{simulation?.name}</h2>
            <h2>Room Code: {params?.code}</h2>
            <Container>
                <ParticipantList participants={participants} onKickClicked={handleKickParticipant} />
            </Container>
            <Button onClick={handleCloseRoom}>Close Room</Button>
            <a href="/instructor">Instructor Home</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorSimulationRoomPage;