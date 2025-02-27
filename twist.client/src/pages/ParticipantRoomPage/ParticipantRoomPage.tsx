import { Button, Col, Container, Form, Row } from "react-bootstrap";
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
import JointDocument from "../../components/JointDocument/JointDocument";
import "./styles.scss";
import CountryEnum from "../../enums/CountryEnum";

const ParticipantRoomPage = () => {
    const [isSimulationLoaded, setIsSimulationLoaded] = useState(false);
    const [isConnected, setIsConnected] = useState(false);
    const [error, setError] = useState<string>("");
    const [signedIn, setSignedIn] = useState(false);

    // Simulation data 
    const [simulation, setSimulation] = useState<SimulationModel>();

    // Instance data
    const [currentParticipant, setCurrentParticipant] = useState<ParticipantModel>();

    // Score calculation
    const [finalTallyVisible, setFinalTallyVisible] = useState(false);
    const [finalUsaScore, setFinalUsaScore] = useState(0);
    const [finalPrcScore, setFinalPrcScore] = useState(0);
    const [winner, setWinner] = useState(CountryEnum.NONE);

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

        connection.on("FinalTallyRevealed", (usaScore: number, prcScore: number) => {

            console.log(`USA: ${usaScore}\nPRC: ${prcScore}`)
            setFinalUsaScore(usaScore);
            setFinalPrcScore(prcScore);
            setWinner(usaScore > prcScore ? CountryEnum.USA : CountryEnum.PRC);
            setFinalTallyVisible(true);
        });

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

    // Persistent participant info header (shown if signed in)
    const ParticipantInfoHeader = () => (
        <div className="participant-info" style={{ textAlign: "left" }}>
            <p>
                <strong>ID:</strong> {currentParticipant?.participant_id} <br />
                <strong>Name:</strong> {currentParticipant?.username} <br />
                <strong>Email:</strong> {currentParticipant?.email} <br />
                <strong>Country:</strong>{" "} 
                {currentParticipant?.country !== 0
                    ? currentParticipant?.country === 1
                        ? "USA"
                        : "PRC"
                    : "NONE"} <br />
                <strong>Connection ID:</strong> {connection?.connectionId}
            </p>
        </div>
    );

    // Error screen
    if (error) return <Container><p>Error: {error}</p></Container>;

    // Loading screen
    if (!isConnected || !isSimulationLoaded) return (
        <ParticipantPageLoadingStatus
            connection={isConnected ? "connected" : "connecting"}
            simulation={isSimulationLoaded ? "loaded" : "loading"}
        />
    );

    // If signed in, render the persistent participant header and main content
    if (signedIn) {
        return (
            <Container className="participant-room">
                <Row className="header">
                    <Col>
                        <h1>{simulation?.name}</h1>
                        <p>
                            <strong>Current Round:</strong> {simulation?.round}
                        </p>
                    </Col>
                    <Col md={4}>
                        {currentParticipant && <ParticipantInfoHeader />}
                    </Col>
                </Row>
                <hr />

                {/* Main content based on simulation state/round */}
                {simulation?.state === SimulationStateEnum.IN_PROGRESS ? (
                    <>
                        {simulation.round === RoundEnum.DOMESTIC && (
                            <>
                                <h2>Round 1 - Domestic</h2>
                                <div className="documents-container">
                                    <div className="document-container">
                                        {!currentParticipant || !connection ? <>Loading...</> : (<AsksDocument
                                            simulation={simulation}
                                            participant={currentParticipant}
                                            connection={connection}
                                        />)}
                                    </div>
                                    <div className="document-container">
                                        {!currentParticipant || !connection ? <>Loading...</> : (<ConcessionsDocument
                                            simulation={simulation}
                                            participant={currentParticipant}
                                            connection={connection!}
                                        />)}
                                    </div>
                                </div>
                            </>
                        )}
                        {simulation.round === RoundEnum.INTERNATIONAL && (
                            <>
                                <h2>Round 2 - International</h2>
                                {!currentParticipant || !connection ? <>Loading...</> : (<JointDocument
                                    simulation={simulation}
                                    participant={currentParticipant}
                                    connection={connection!}
                                />)}
                            </>
                        )}
                        {simulation.round === RoundEnum.FINAL_TALLY && (
                            <>
                                <h2>Final Tally</h2>
                                {!finalTallyVisible ? <p>Waiting for final tally reveal...</p> : (
                                    <>
                                        <Row>
                                            <Col>
                                                <h3>PRC</h3>
                                                <p>{finalPrcScore} points</p>
                                            </Col>
                                            <Col>
                                                <h3>USA</h3>
                                                <p>{finalUsaScore} points</p>
                                            </Col>
                                        </Row>
                                        <h3>{CountryEnum[winner]} wins!</h3>
                                    </>)
                                }
                            </>
                        )}
                    </>
                ) : (
                    // Waiting room content (simulation not yet started)
                    <div className="waiting-room">
                        <h2>Waiting for simulation to start...</h2>
                        <p>
                            Please wait for the instructor to start the simulation. Your info
                            remains visible.
                        </p>
                    </div>
                )}

                <Row>
                    <Col>
                        <a href="/">Back</a>
                    </Col>
                </Row>
            </Container>
        );
    }

    // Default joining screen (if not signed in yet)
    return (
        <Container className="join-room">
            <h1>{simulation?.name}</h1>
            <p>Enter your details to join the simulation.</p>
            <Form onSubmit={handleSubmit}>
                <Form.Group controlId="email">
                    <Form.Label>Email:</Form.Label>
                    <Form.Control type="email" placeholder="Enter your email here..." />
                </Form.Group>
                <Form.Group controlId="name">
                    <Form.Label>Name:</Form.Label>
                    <Form.Control type="text" placeholder="Enter your name here..." />
                </Form.Group>
                <Button type="submit" className="mt-3">
                    Join
                </Button>
            </Form>
            <a href="/">Back</a>
        </Container>
    );
}

export default ParticipantRoomPage;