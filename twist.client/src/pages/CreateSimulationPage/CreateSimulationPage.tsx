import { useAuth0 } from "@auth0/auth0-react";
import axios from 'axios';
import SimulationModel from "../../models/SimulationModel";
import { FormEvent } from "react";
import { Button, Form, FormLabel } from "react-bootstrap";
import { createSimulation, generateCode } from "../../server/simulation_management";

const CreateSimulationPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect, user } = useAuth0();

    async function onCreateClicked(event: FormEvent) {
        event.preventDefault();

        // Get data from form
        const form = document.getElementById("simulation-form") as HTMLFormElement;
        const data = new FormData(form);
        const name = data.get("name") as string;

        // Check and generate code
        let code;
        try {
            code = await generateCode();
        } catch (error) {
            console.error(`Unable to create new simulation: ${error}`);
            return;
        }

        // Create simulation
        await createSimulation(name, code);

        // Navigate to the room
        window.location.assign(`/instructor/room/${code}`);
    }

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Create Simulation</h1>
            <Form onSubmit={onCreateClicked} id="simulation-form">
                <Form.Group>
                    <Form.Label htmlFor="name">Simulation Name</Form.Label>
                    <Form.Control id="name" name="name" placeholder="Name" />
                    <Button type="submit">Create</Button>
                    <Button type="reset">Reset</Button>
                </Form.Group>
            </Form>

            <Button onClick={() => { window.location.assign("/instructor") }}>Cancel</Button>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default CreateSimulationPage;