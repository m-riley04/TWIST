import { Button, Form } from "react-bootstrap";
import { doesSimulationExist } from "../../server/simulation_management";
import { useNavigate } from "react-router";
import React from "react";

const ParticipantLoginPage = () => {
    const navigate = useNavigate();

    const handleSubmitCode = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        // Get code from form
        const code = (document.getElementById("code") as HTMLInputElement).value;
        
        // Check if the simulation exists
        doesSimulationExist(code)
            .then((exists) => {
                if (!exists) {
                    console.error(`Simulation does not exist with code '${code}'.`);
                    return;
                }
                navigate(`/room/${code}`);
            })
            .catch((error) => console.error(error));
    }

    return (
        <>
            <h1>KU Trade War Simulation</h1>
            <p>Enter the room code to begin.</p>
            <Form onSubmit={handleSubmitCode}>
                <Form.Group>
                    <Form.Label htmlFor="text">Room Code:</Form.Label><br />
                    <Form.Control id="code" title="Code" type="text" placeholder="Enter your code here..." />
                    <Button type="submit">Join</Button>
                </Form.Group>
            </Form>

            <a href="/instructor">Instructor Login</a>
        </>
    );
}

export default ParticipantLoginPage;