import { useAuth0 } from "@auth0/auth0-react";
import LogoutButton from "../../components/LogoutButton";
import SimulationsList from "../../components/SimulationList/SimulationList";
import { useState } from "react";
import { useEffect } from "react";
import axios from 'axios';
import { Button } from "react-bootstrap";
import { getSimulations } from "../../server/simulation_management";
import SimulationModel from "../../models/SimulationModel";

const InstructorPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect, user } = useAuth0();
    const [simulations, setSimulations] = useState<SimulationModel[]>([]);
    
    useEffect(() => {
        // Load simulations data from API
        getSimulations()
            .then(data => {
                if (data !== undefined) {
                    setSimulations(data);
                }
            });
    }, []);

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Instructor Home</h1>
            <p><i>Account: {user?.name}</i></p>
            <p>You can control, view, and create simulations here.</p>
            <h2>Simulations</h2>
            <SimulationsList simulations={simulations} />
            <Button onClick={() => { window.location.assign("/instructor/create") }}>Create New Simulation</Button>
            <LogoutButton />
            <a href="/">Participant?</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorPage;