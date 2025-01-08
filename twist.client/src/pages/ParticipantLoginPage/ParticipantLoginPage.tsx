import { Button, Container, Form } from "react-bootstrap";
import { doesSimulationExist } from "../../server/simulation_management";
import { useNavigate } from "react-router";
import React from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";

const connection = new HubConnectionBuilder()
    .withUrl("https://localhost:7026/chatHub")
    .withAutomaticReconnect()
    .build();

connection.start()
    .then(() => console.log('Connected to SignalR hub'))
    .catch(err => console.error('Error connecting to hub:', err));

connection.on("ReceiveMessage", (username: string, message: string) => {
    console.log(`Received message from ${username}:`, message);
});

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
            <Container>
                <Form onSubmit={handleSubmitCode}>
                    <Form.Group>
                        <Form.Label htmlFor="text">Room Code:</Form.Label><br />
                        <Form.Control id="code" title="Code" type="text" placeholder="Enter your code here..." />
                        <Button type="submit">Join</Button>
                    </Form.Group>
                </Form>
            </Container>

            <Container>
                <Form>
                    <Form.Label>Name: </Form.Label>
                    <Form.Control id="name" />

                    <Form.Label>Message: </Form.Label>
                    <Form.Control id="message" />

                    <Button type="submit" onClick={(event) => {
                        event.preventDefault();
                        const name = (document.getElementById("name") as HTMLInputElement).value;
                        const message = (document.getElementById("message") as HTMLInputElement).value;
                        connection.send('NewMessage', name, message)
                            .then(() => ((document.getElementById("message") as HTMLInputElement).value = ""))
                            .catch((error) => console.error(error));
                        }}>Send</Button>
                </Form>
            </Container>

            <a href="/instructor">Instructor Login</a>
        </>
    );
}

export default ParticipantLoginPage;