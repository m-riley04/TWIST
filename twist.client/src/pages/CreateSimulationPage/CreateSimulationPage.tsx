import { Button, Form } from "react-bootstrap";
import { createSimulation, createSimulationAsksPRC, createSimulationAsksUSA, createSimulationConcessionsPRC, createSimulationConcessionsUSA, generateCode, getSimulationFromCode } from "../../server/simulation_management";
import { FormEvent } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router";

const CreateSimulationPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect } = useAuth0();
    const navigate = useNavigate();

    async function onCreateClicked(event: FormEvent) {
        event.preventDefault();

        // Get data from form
        const form = document.getElementById("simulation-form") as HTMLFormElement;
        const data = new FormData(form);
        const name = data.get("name") as string;
        
        try {
            // Check and generate code
            const code = await generateCode();

            // Create simulation
            await createSimulation(name, code);
            const simId: number = (await getSimulationFromCode(code))?.simulation_id ?? 0;

            // Initialize asks and concessions
            await createSimulationAsksUSA(simId);
            await createSimulationAsksPRC(simId)
            await createSimulationConcessionsUSA(simId);
            await createSimulationConcessionsPRC(simId);

            // Navigate AFTER
            navigate(`/instructor/room/${code}`);

        } catch (error) {
            console.error(`Unable to create new simulation: ${error}`);
            return;
        }
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