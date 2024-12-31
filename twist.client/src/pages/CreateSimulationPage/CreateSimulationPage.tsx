import { useAuth0 } from "@auth0/auth0-react";
import axios from 'axios';
import SimulationModel from "../../models/SimulationModel";
import { FormEvent } from "react";
import { Button, Form, FormLabel } from "react-bootstrap";

const CreateSimulationPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect, user } = useAuth0();

    async function createSimulation(name: string, code: string) {
        try {
            await axios.put<SimulationModel>("https://localhost:7026/api/simulations", {
                name: name,
                participants: JSON.stringify([]),
                start_date: new Date().toISOString(),
                end_date: new Date().toISOString(),
                active: false,
                responses: JSON.stringify([]),
                asks: JSON.stringify([]),
                concessions: JSON.stringify([]),
                round: 0,
                code: code
            });
        } catch (error) {
            console.log(`Unable to create simulation: ${error}`);
        }
    }

    async function doesSimulationExist(code: string) {
        try {
            const response: SimulationModel[] = await axios
                .get<SimulationModel[]>(`https://localhost:7026/api/simulations/${code}`)
                .then(response => response.data);
            
            return response.length > 0;

        } catch (error) {
            console.error(`Unable to check code: ${error}`);
            return true;
        }
    }

    async function generateCode(timeout: number = 5): Promise<string> {
        for (let i = 0; i < timeout; i++) {
            const code = Math.random().toString(36).substring(7);
            const exists = await doesSimulationExist(code);

            if (!exists) return code;
        }

        throw new Error("Unable to generate unique code.");
    }

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