import { useAuth0 } from "@auth0/auth0-react";
import LogoutButton from "../../components/LogoutButton";
import SimulationsList from "../../components/SimulationList/SimulationList";
import { useState } from "react";
import { useEffect } from "react";
import axios from 'axios';
import SimulationModel from "../../models/SimulationModel";

const InstructorPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect, user } = useAuth0();
    const [simulations, setSimulations] = useState([]);

    function createSimulation() {
        axios.put<SimulationModel>('https://localhost:7026/api/simulations',
            {
                name: "Name",
                participants: JSON.stringify([]),
                start_date: new Date().toISOString(),
                end_date: new Date().toISOString(),
                active: false,
                responses: JSON.stringify([]),
                asks: JSON.stringify([]),
                concessions: JSON.stringify([]),
                round: 0
            })
            .then((response) => { })
            .catch((error) => console.log(error));
    }

    // Load data from API
    useEffect(() => {
        /// TODO: Make this an environment variable for API URL
        axios.get('https://localhost:7026/api/simulations')
            .then((response) => setSimulations(response.data))
            .catch((error) => console.log(error));
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
            <button onClick={createSimulation}>Create New Simulation</button>
            <LogoutButton />
            <a href="/">Participant?</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorPage;