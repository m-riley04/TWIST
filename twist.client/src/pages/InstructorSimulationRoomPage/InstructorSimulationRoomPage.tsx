import { useAuth0 } from "@auth0/auth0-react";
import { Button } from "react-bootstrap";
import { useEffect } from "react";
import { useParams } from "react-router";
import { useState } from "react";
import axios from "axios";
import SimulationModel from "../../models/SimulationModel";

const InstructorSimulationRoomPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect } = useAuth0();
    const params = useParams();
    const [simulation, setSimulation] = useState<SimulationModel>();


    useEffect(() => {
        // TODO: Check if the simulation exists

        // TODO: Load simulation data
        axios.get<SimulationModel[]>(`https://localhost:7026/api/simulations/${params?.code}`)
            .then(response => response?.data)
            .then(data => {
                // Check if simulation exists
                if (data?.length <= 0) {
                    throw new Error("Simulation not found.");
                }

                // Get first simulation
                return data[0];
            })
            .then(data => setSimulation(data))
            .catch(error => console.error(error));

        // TODO: Be able to close the room

        // TODO: Be able to change room settings

        // TODO: Add QR code for participants to join
    }, []);

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Simulation Room</h1>
            <h2>{simulation?.name}</h2>
            <h2>Room Code: {params?.code}</h2>
            <Button>Close Room</Button>
            <a href="/instructor">Instructor Home</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorSimulationRoomPage;