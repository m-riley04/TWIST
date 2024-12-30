import { useAuth0 } from "@auth0/auth0-react";
import axios from 'axios';
import SimulationModel from "../../models/SimulationModel";
import { FormEventHandler } from "react";
import { FormEvent } from "react";

const CreateSimulationPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect, user } = useAuth0();

    function createSimulation(name: string) {
        axios.put<SimulationModel>('https://localhost:7026/api/simulations',
            {
                name: name,
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

    function onCreateClicked(event: FormEvent) {
        event.preventDefault();
        const form = document.getElementById("simulation-form") as HTMLFormElement;
        const data = new FormData(form);
        const name = data.get("name") as string;
        createSimulation(name);
        window.location.assign("/instructor/room"); /// TODO: Redirect to the specific room
    }

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Create Simulation</h1>
            <form onSubmit={onCreateClicked} id="simulation-form">
                <label htmlFor="name">Simulation Name</label>
                <input id="name" name="name" placeholder="Name" />
                <br/>
                <button type="submit">Create</button>
                <button type="reset">Reset</button>
            </form>

            <button onClick={() => { window.location.assign("/instructor") }}>Cancel</button>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default CreateSimulationPage;