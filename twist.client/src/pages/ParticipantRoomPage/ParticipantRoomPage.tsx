import { Button, Container, Form } from "react-bootstrap";
import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { getSimulationFromCode } from "../../server/simulation_management";
import { HubConnection } from "@microsoft/signalr";
import { useRoomHub } from "../../signalr/useRoomHub";
import ParticipantPageLoadingStatus from "../../components/ParticipantPageLoadingStatus/ParticipantPageLoadingStatus";
import SimulationModel from "../../models/SimulationModel";
import RoundEnum from "../../enums/RoundEnum";
import AsksDocument from "../../components/AsksDocument/AsksDocument";
import SimulationStateEnum from "../../enums/SimulationStateEnum";
import ParticipantModel from "../../models/ParticipantModel";
import { getParticipantByEmailAndSimulation } from "../../server/participant_management";
import ConcessionsDocument from "../../components/ConcessionsDocument/ConcessionsDocument";

const ParticipantRoomPage = () => {
    const [isSimulationLoaded, setIsSimulationLoaded] = useState(false);
    const [isConnected, setIsConnected] = useState(false);
    const [error, setError] = useState<string>("");
    const [signedIn, setSignedIn] = useState(false);

    // Simulation data 
    const [simulation, setSimulation] = useState<SimulationModel>();

    // Instance data
    const [currentParticipant, setCurrentParticipant] = useState<ParticipantModel>();

    const params = useParams();
    const simCode = params.code ?? "";
    const connection: HubConnection | undefined = useRoomHub(simCode);

    useEffect(() => {
        // Check if the code is valid
        if (!simCode) {
            const e = "No code provided";
            console.error(e);
            setError(e);
            return;
        }

        // Get the simulation from the code
        getSimulationFromCode(simCode)
            .then((data) => {
                setSimulation(data);
                setIsSimulationLoaded(true);
                console.log("Simulation loaded.");
            })
            .catch((error) => {
                console.error(error);
                setError(error);
            });
    }, [simCode]);

    // Initialize connection
    useEffect(() => {
        // Check if connection is defined
        if (!connection) return;

        // Set connection
        setIsConnected(true);

        // Connect signals
        connection.on("ParticipantUpdated", (participant) => {
            console.log(`Participant ${participant.name} joined.`);
        });

        connection.on("SimulationStarted", (sim: SimulationModel) => {
            setSimulation(sim);
            console.log("Simulation started.");
        })

        connection.on("SimulationStopped", (sim: SimulationModel) => {
            setSimulation(sim);
            console.log("Simulation stopped.");
        })

        connection.on("RoundUpdated", (round: RoundEnum) => {
            setSimulation((prev) => {
                if (!prev) return; // Null check

                return ({ ...prev, round: round });
            });
            console.log(`Round updated to ${round}`);
        });

        //connection.on("ParticipantKicked", (participant: ParticipantModel) => {
        //    if (participant.connection_id === connection.connectionId) {
        //        console.log("You have been kicked.");
        //        navigate("/");
        //    }
        //})

        // Cleanup
        return () => {
            connection.stop().catch(console.error);
        }
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
                getParticipantByEmailAndSimulation(email, simulation?.simulation_id ?? 0).
                    then((participant) => {
                        setCurrentParticipant(participant[0]); // TODO: Make this safer
                    })
                    .catch((error) => console.error(`Failed to get participant: ${error}`));
                console.log(`Joined simulation ${simulation?.code} as ${name}`);
            })
            .catch((error) => console.error(`Failed to join simulation: ${error}`));
    }

    // Error screen
    if (error) return <p>Error: {error}</p>;

    // Loading screen
    if (!isConnected || !isSimulationLoaded) return (
        <ParticipantPageLoadingStatus
            connection={isConnected ? "connected" : "connecting"}
            simulation={isSimulationLoaded ? "loaded" : "loading"}
        />
    );

    // Round screens
    if (simulation?.state == SimulationStateEnum.IN_PROGRESS && signedIn) {
        switch (simulation?.round) {
            case RoundEnum.DOMESTIC:
                return (
                    <>
                        <h1>Round 1 - Domestic</h1>
                        <p>Id: {currentParticipant?.participant_id}</p>
                        <p>Email: {currentParticipant?.email}</p>
                        <p>Connection ID: {connection?.connectionId}</p>
                        <p>Country: {currentParticipant?.country}</p>
                        
                        {simulation && currentParticipant && connection && (
                            <AsksDocument
                                simulation={simulation}
                                participant={currentParticipant}
                                connection={connection}
                            />
                        )}

                        {simulation && currentParticipant && connection && (
                            <ConcessionsDocument
                                simulation={simulation}
                                participant={currentParticipant}
                                connection={connection}
                            />
                        )}
                    </>
                );
            case RoundEnum.INTERNATIONAL:
                return (
                    <>
                        <h1>Round 2 - International</h1>
                    </>
                );
            case RoundEnum.FINAL_TALLY:
                return (
                    <>
                        <h1>Final Tally</h1>
                        <h2>China</h2>
                        <p>{ } points</p>

                        <h2>USA</h2>
                        <p>{ } points</p>

                        <h2>{ } wins!</h2>
                    </>
                );
            default:
                return <>Waiting for simulation to start...</>
        }
    } else if (signedIn) { // Waiting room
        return (
            <>
                <p>You are signed in! Please wait for the instructor to start the simulation.</p>
                <a href="/">Back</a>
            </>
        );
    }

    // Default joining screen
    return (
        <>
            <p>You are now joining...</p>
            <h1>{simulation?.name}</h1>
            <p>Enter your details to be logged in.</p>
            <Container>
                <Form onSubmit={handleSubmit}>
                    <Form.Group>
                        <Form.Label htmlFor="email">Email:</Form.Label>
                        <Form.Control id="email" title="Email" type="email" placeholder="Enter your email here..." />

                        <Form.Label htmlFor="name">Name:</Form.Label>
                        <Form.Control id="name" title="Name" type="text" placeholder="Enter your name here..." />

                        <Button type="submit">Join</Button>
                    </Form.Group>
                </Form>
            </Container>

            <a href="/">Back</a>
        </>
    );
}

export default ParticipantRoomPage;