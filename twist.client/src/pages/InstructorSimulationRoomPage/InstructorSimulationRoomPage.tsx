import { useAuth0 } from "@auth0/auth0-react";
import { Button } from "react-bootstrap";
import { useEffect } from "react";
import { useParams } from "react-router";

const InstructorSimulationRoomPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect } = useAuth0();
    const params = useParams();

    useEffect(() => {
        // TODO: Check if the simulation exists

        // TODO: Load simulation data

        // TODO: Be able to close the room

        // TODO: Be able to change room settings

        // TODO: Add QR code for participants to join
    });

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Simulation Room</h1>
            <h2>Room Code: {params.code}</h2>
            <Button>Close Room</Button>
            <a href="/instructor">Instructor Home</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorSimulationRoomPage;