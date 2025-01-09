import { Button, Form } from "react-bootstrap";
import SimulationModel from "../../models/SimulationModel";
import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { getSimulationFromCode } from "../../server/simulation_management";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { SERVER_URL } from "../../server/server_consts";

const ParticipantRoomPage = () => {
    const [connection, setConnection] = useState<HubConnection>();
    const [simulation, setSimulation] = useState<SimulationModel>();
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(true);
    const params = useParams();
    const [signedIn, setSignedIn] = useState(false);

    useEffect(() => {
        // Check if the code is valid
        if (params.code === undefined) {
            setError("No code provided.");
            console.error("No code provided");
            return;
        }

        // Get the simulation from the code
        getSimulationFromCode(params.code)
            .then((data) => setSimulation(data))
            .then(() => setLoading(false))
            .catch((error) => {
                setError(`Unable to load simulation from code: ${error}`);
                console.error(error);
            });

        // Attempt to connect to the SignalR hub
        const con = new HubConnectionBuilder()
            .withUrl(`${SERVER_URL}/roomHub`)
            .withAutomaticReconnect()
            .build();
        setConnection(con);

    }, []);

    useEffect(() => {
        // Check if connection is defined
        if (connection === undefined) return;

        // Start connection
        connection.start()
            .then(() => console.log('Connected to SignalR hub'))
            .catch(err => console.error('Error connecting to hub:', err));

    }, [connection]);

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        // Get email and name from form
        const email = (document.getElementById("email") as HTMLInputElement).value;
        const name = (document.getElementById("name") as HTMLInputElement).value;

        // Check if email and name are valid
        if (email === "") {
            console.error("Email must be provided.");
            return;
        }

        if (name === "") {
            console.error("Name must be provided.");
            return;
        }

        // Attempt to join
        connection?.invoke("JoinRoom", simulation, name, email)
            .then(() => {
                setSignedIn(true);
                console.log(`Joined simulation ${simulation?.code} as ${name}`);
            })
            .catch((error) => console.error(`Failed to join simulation: ${error}`));
    }

    if (error !== "") return <div>{error}</div>;

    if (loading) return <div>Loading...</div>;

    if (connection?.state === "Connecting") return <div>Connecting sockets...</div>;

    return (
        <>
            <p>You are now joining...</p>
            <h1>{simulation?.name}</h1>
            <p>Enter your details to be logged in.</p>
            {
                signedIn ? 
                <p>You are signed in! Please wait for the instructor to start the simulation.</p>
                : <Form onSubmit={handleSubmit}>
                    <Form.Group>
                        <Form.Label htmlFor="email">Email:</Form.Label><br />
                        <Form.Control id="email" title="Email" type="email" placeholder="Enter your email here..." />

                        <Form.Label htmlFor="name">Name:</Form.Label><br />
                        <Form.Control id="name" title="Name" type="text" placeholder="Enter your name here..." />

                        <Button type="submit">Join</Button>
                    </Form.Group>
                </Form>
            }

            <a href="/">Back</a>
        </>
    );
}

export default ParticipantRoomPage;