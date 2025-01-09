import { useAuth0 } from "@auth0/auth0-react";
import { Button } from "react-bootstrap";
import { useEffect } from "react";
import { useParams } from "react-router";
import { useState } from "react";
import SimulationModel from "../../models/SimulationModel";
import { closeSimulation, getSimulationFromCode } from "../../server/simulation_management";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import ParticipantModel from "../../models/ParticipantModel";

const InstructorSimulationRoomPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect } = useAuth0();
    const params = useParams();
    const [simulation, setSimulation] = useState<SimulationModel>();
    const [connection, setConnection] = useState<HubConnection>();
    const [participants, setParticipants] = useState<ParticipantModel[]>([]);

    useEffect(() => {
        // Check if code is valid
        if (params.code === undefined) {
            console.error("No code provided.");
            return;
        }

        // Load simulation data
        getSimulationFromCode(params.code)
            .then(data => setSimulation(data));

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
        // Check if connection is defined
        if (connection === undefined) return;

        // Start connection
        connection.start()
            .then(() => console.log('Connected to SignalR hub'))
            .catch(err => console.error('Error connecting to hub:', err));

        // Connection signals/slots
        connection.on("ParticipantJoined", (id: number, username: string, email: string) => {
            // Add to participants list
            setParticipants([...participants, { participant_id: id, simulation_id: 0, username: username, email: email }]);
            console.log(`New participant joined:`, username);
        });
    }, [connection]);

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Simulation Room</h1>
            <h2>{simulation?.name}</h2>
            <h2>Room Code: {params?.code}</h2>
            <Button onClick={() => {
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
                        window.location.assign("/instructor");
                    });
            }}>Close Room</Button>
            <a href="/instructor">Instructor Home</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorSimulationRoomPage;