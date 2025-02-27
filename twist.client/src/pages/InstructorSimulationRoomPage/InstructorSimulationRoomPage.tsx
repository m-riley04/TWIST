import { useAuth0 } from "@auth0/auth0-react";
import { Button, Container, Dropdown, DropdownButton, Modal } from "react-bootstrap";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import SimulationModel from "../../models/SimulationModel";
import { closeSimulation, getSimulationFromCode } from "../../server/simulation_management";
import { HubConnection } from "@microsoft/signalr";
import ParticipantModel from "../../models/ParticipantModel";
import ParticipantList from "../../components/ParticipantList/ParticipantList";
import { getParticipants } from "../../server/participant_management";
import CountryEnum from "../../enums/CountryEnum";
import RoleEnum from "../../enums/RoleEnum";
import { useRoomHub } from "../../signalr/useRoomHub";
import RoundEnum from "../../enums/RoundEnum";
import QRCode from "react-qr-code";

// TODO: make this into an env variable
const WEB_DOMAIN = "twist-server.azurewebsites.net";

const InstructorSimulationRoomPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect } = useAuth0();

    const [simulation, setSimulation] = useState<SimulationModel>();
    const [participants, setParticipants] = useState<ParticipantModel[]>([]);

    // Modal state for reset confirmations
    const [showResetModal, setShowResetModal] = useState(false);
    const [resetModalTitle, setResetModalTitle] = useState("");
    const [resetModalMessage, setResetModalMessage] = useState("");
    const [pendingResetAction, setPendingResetAction] = useState<(() => void) | null>(null);

    const navigate = useNavigate();
    const params = useParams();
    const simCode = params.code ?? "";
    const connection: HubConnection | undefined = useRoomHub(simCode);

    // Initialize simulation data
    useEffect(() => {
        // Check if code is valid
        if (!simCode) {
            console.error("No code provided.");
            return;
        }

        // Load simulation data
        getSimulationFromCode(simCode)
            .then(data => setSimulation(data))
            .catch(error => console.error(`Unable to load simulation: ${error}`));

        // TODO: Be able to change room settings
    }, [simCode]);

    // Initialize connection
    useEffect(() => {
        // Check if connection is defined
        if (!connection) return;

        if (connection.state === "Connected") {
            console.log("Connected to SignalR hub.");
        }

        connection.on("SimulationStarted", (sim: SimulationModel) => {
            setSimulation(sim);
            console.log("Simulation started.");
        })

        connection.on("SimulationStopped", (sim: SimulationModel) => {
            setSimulation(sim);
            console.log("Simulation stopped.");
        })

        connection.on("CountriesAssigned", (participants: ParticipantModel[]) => {
            setParticipants(participants);
            console.log("Countries have been randomly assigned.");
        });

        connection.on("RolesAssigned", (participants: ParticipantModel[]) => {
            setParticipants(participants);
            console.log("Roles have been randomly assigned.");
        });

        connection.on("InstructorInitialized", () => {
            console.log("Instructor has been initialized.");
        });

        connection.on("ConnectionsPolled", (connections: string[]) => {
            console.log(connections);
        });

        connection.on("GroupsPolled", (groups: object) => {
            console.log(groups);
        });

        // Cleanup
        return () => {
            connection.stop().catch(console.error);
        }

    }, [connection]);

    /// INSTRUCTOR AND SIMULATION-DEPENDENT SIGNALS
    useEffect(() => {
        if (!connection || !simulation) return;

        // Initialize instructor
        connection.invoke("InstructorInitialize", simulation)
            .catch((error) => console.error(`Unable to initialize instructor: ${error}`));

        connection.on("RoundUpdated", (round: RoundEnum) => {
            setSimulation((prev) => {
                if (!prev) return; // Null check

                return ({ ...prev, round: round });
            });
            console.log(`Updated to round ${round}`);
        });

    }, [connection, simulation])

    // Initialize participants
    useEffect(() => {
        if (!simulation) return;

        // Get the participants
        getParticipants(simulation.simulation_id)
            .then(data => setParticipants(data))
            .catch(error => console.error(`Unable to load participants: ${error}`));

    }, [simulation])

    useEffect(() => {
        if (!connection) return;

        const handleParticipantJoined = (participant: ParticipantModel) => {
            setParticipants((prev) => {
                // Only add if not already present
                if (prev.find((p) => p.participant_id === participant.participant_id)) {
                    console.error(`Participant '${participant.username}' already in list`);
                    return prev;
                }
                console.log(`New participant joined:`, participant.username);
                return [...prev, participant];
            });
        };

        // Connection signals/slots
        connection.on("ParticipantJoined", handleParticipantJoined);

        connection.on("ParticipantLeft", (participant: ParticipantModel) => {
            // Remove from participants list
            setParticipants((prev) => prev.filter((p) => p.participant_id !== participant.participant_id));
            console.log(`Participant left:`, participant.username);
        });

        connection.on("ParticipantKicked", (participant: ParticipantModel) => {
            setParticipants((prev) => prev.filter((p) => p.email !== participant.email));
            console.log(`Kicked participant '${participant.email}'`);
        });

        connection.on("ParticipantDisconnected", () => { //participant: ParticipantModel) => {
            // Update the participant list
            setParticipants((prev) => prev.map((p) => {
                /// TODO: more here
                //if (p.participant_id === participant.participant_id) {
                //    p.connected = false;
                //}
                return p;
            }));
        });

        // Cleanup to remove the event listener
        return () => {
            connection.off("ParticipantJoined", handleParticipantJoined);
        };
    }, [connection])

    const handleCloseRoom = () => {
        if (params.code === undefined) {
            console.error("Unable to close room: No code provided.")
            return;
        }
        closeSimulation(params.code, new Date())
            .then((response) => {
                // Check if the response failed
                if (response === undefined) {
                    return;
                }

                // Navigate back to instructor home
                navigate("/instructor");
            })
            .catch((error) => {
                console.error(`Unable to close room: ${error}`);
            });
    }

    const handleStartSimulation = () => {
        // Check if simulation is loaded
        if (!simulation) {
            console.error("Unable to start simulation: No simulation loaded.");
            return;
        }

        // Check current round
        if (simulation?.round === RoundEnum.NONE) {
            console.error("Unable to start simulation: No starting round selected.");
            return;
        }

        connection?.invoke("StartSimulation", simulation)
            .catch((error) => console.error(`Unable to start simulation: ${error}`));
    }

    const handleStopSimulation = () => {
        // Check if simulation is loaded
        if (!simulation) {
            console.error("Unable to start simulation: No simulation loaded.");
            return;
        }

        // Check current round
        if (simulation?.round === RoundEnum.NONE) {
            console.error("Unable to start simulation: No starting round selected.");
            return;
        }

        connection?.invoke("StopSimulation", simulation)
            .catch((error) => console.error(`Unable to stop simulation: ${error}`));
    }

    const handleKickParticipant = (participant: ParticipantModel) => {
        if (connection === undefined) {
            console.error("Unable to kick participant: No connection to hub.");
            return;
        }

        connection.invoke("KickParticipant", simulation, participant)
            .catch((error) => console.error(`Unable to kick participant: ${error}`));
    }

    const handleCountryChanged = (participant: ParticipantModel, country: CountryEnum) => {
        if (connection === undefined) {
            console.error("Unable to update participant: No connection to hub.");
            return;
        }

        connection.invoke("UpdateParticipantCountry", simulation, participant, country)
            .then(() => {
                // Update participant in list
                setParticipants((prev) => prev.map((p) => {
                    if (p.participant_id === participant.participant_id) {
                        p.country = country;
                    }
                    return p;
                }));
                console.log(`Updated participant '${participant.email}' country to ${country}`);
            })
            .catch((error) => console.error(`Unable to update participant: ${error}`));
    }

    const handleRoleChanged = (participant: ParticipantModel, role: RoleEnum) => {
        if (connection === undefined) {
            console.error("Unable to update participant: No connection to hub.");
            return;
        }

        connection.invoke("UpdateParticipantRole", simulation, participant, role)
            .then(() => {
                // Update participant in list
                setParticipants((prev) => prev.map((p) => {
                    if (p.participant_id === participant.participant_id) {
                        p.role = role;
                    }
                    return p;
                }));
                console.log(`Updated participant '${participant.email}' role to ${role}`);
            })
            .catch((error) => console.error(`Unable to update participant: ${error}`));
    }

    const handleRandomlyAssignCountry = () => {
        connection?.invoke("RandomlyAssignCountries", simulation, participants)
            .catch((error) => console.error(`Unable to randomly assign countries: ${error}`));
    }

    const handleRandomlyAssignRole = () => {
        connection?.invoke("RandomlyAssignRoles", simulation, participants)
            .catch((error) => console.error(`Unable to randomly assign roles: ${error}`));
    }

    const handleUpdateRound = (newRound: RoundEnum) => {
        connection?.invoke("UpdateRound", simulation, newRound)
            .catch((error) => console.error(`Unable to update round: ${error}`));
    }

    const handleNextRound = () => {
        if (!simulation) return;

        if (simulation?.round === RoundEnum.FINAL_TALLY) {
            console.error("Cannot go to next round: Already at final tally.");
            return;
        }

        handleUpdateRound(simulation?.round + 1);
    }

    const handlePreviousRound = () => {
        if (!simulation) return;

        if (simulation?.round === RoundEnum.NONE) {
            console.error("Cannot go to previous round: No round selected.");
            return;
        }

        handleUpdateRound(simulation?.round - 1);
    }

    const handleReveal = () => {
        connection?.invoke("RevealFinalTally", simulation)
            .catch((error) => console.error(`Unable to reveal final score: ${error}`));
    }

    /// RESETTING HANDLERS
    const handleResetUSAAsks = () => {
        connection?.invoke("ResetCountryAsks", simulation?.simulation_id, CountryEnum.USA, simulation?.code)
            .catch((error) => console.error(`Unable to reset USA asks: ${error}`));
    }

    const handleResetUSAConcessions = () => {
        connection?.invoke("ResetCountryConcessions", simulation?.simulation_id, CountryEnum.USA, simulation?.code)
            .catch((error) => console.error(`Unable to reset USA concessions: ${error}`));
    }

    const handleResetPRCAsks = () => {
        connection?.invoke("ResetCountryAsks", simulation?.simulation_id, CountryEnum.PRC, simulation?.code)
            .catch((error) => console.error(`Unable to reset PRC asks: ${error}`));
    }

    const handleResetPRCConcessions = () => {
        connection?.invoke("ResetCountryConcessions", simulation?.simulation_id, CountryEnum.PRC, simulation?.code)
            .catch((error) => console.error(`Unable to reset PRC concessions: ${error}`));
    }

    const handleResetJointAgreements = () => {
        connection?.invoke("ResetJointAgreements", simulation?.simulation_id, simulation?.code)
            .catch((error) => console.error(`Unable to reset PRC concessions: ${error}`));
    }

    /// DEBUGGING HANDLERS
    const handlePollConnections = () => {
        connection?.invoke("PollConnections", simulation)
            .catch((error) => console.error(`Unable to poll connections: ${error}`));
    }

    const handlePollGroups = () => {
        connection?.invoke("PollGroups", simulation)
            .catch((error) => console.error(`Unable to poll groups: ${error}`));
    }

    // --- Modal handlers for reset confirmations ---
    const openResetModal = (action: () => void, title: string, message: string) => {
        setPendingResetAction(() => action);
        setResetModalTitle(title);
        setResetModalMessage(message);
        setShowResetModal(true);
    };

    const closeResetModal = () => {
        setShowResetModal(false);
        setPendingResetAction(null);
    };

    const confirmResetAction = () => {
        if (pendingResetAction) {
            pendingResetAction();
        }
        closeResetModal();
    };

    if (error) return <div>Oops... {error.message}</div>;
    if (isLoading) return <div>Loading...</div>;
    if (!simulation) return <div>Loading simulation...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Simulation Room</h1>
            <h2>{simulation?.name}</h2>
            <p>Current Round: {simulation?.round}</p>
            <DropdownButton title={`Round ${simulation?.round}`}>
                {(Object.values(RoundEnum).filter(n => !isNaN(Number(n))) as RoundEnum[]).map((val, i) => <Dropdown.Item key={i} eventKey={val} onClick={() => handleUpdateRound(val)}>{val}</Dropdown.Item>)}
            </DropdownButton>
            <h2>Room Code: {params?.code}</h2>
            <QRCode value={`https://${WEB_DOMAIN}/room/${params.code}`}/>
            <Container>
                <Button onClick={handleRandomlyAssignCountry}>Randomly Assign Countries</Button>
                <Button onClick={handleRandomlyAssignRole}>Randomly Assign Roles</Button>
                <ParticipantList participants={participants} onKickClicked={handleKickParticipant} onCountryChanged={handleCountryChanged} onRoleChanged={handleRoleChanged} />
            </Container>

            <h3>Basic Simulation Controls</h3>
            <Container>
                <Button onClick={handleStartSimulation}>Start Simulation</Button>
                <Button onClick={handleStopSimulation} variant="warning">Stop Simulation</Button>
                <Button onClick={handleCloseRoom} variant="danger">Close Room</Button>
                <Button onClick={handlePreviousRound}>Previous Round</Button>
                <Button onClick={handleNextRound}>Next Round</Button>
                <Button onClick={handleReveal} variant="warning">Reveal Final Score</Button>
            </Container>

            <h3>Resetting</h3>
            <Container>
                <Button
                    onClick={() =>
                        openResetModal(handleResetUSAAsks, "Reset USA Asks", "Are you sure you want to reset USA Asks?")
                    }
                >
                    Reset USA Asks
                </Button>
                <Button
                    onClick={() =>
                        openResetModal(
                            handleResetUSAConcessions,
                            "Reset USA Concessions",
                            "Are you sure you want to reset USA Concessions?"
                        )
                    }
                >
                    Reset USA Concessions
                </Button>
                <Button
                    onClick={() =>
                        openResetModal(handleResetPRCAsks, "Reset PRC Asks", "Are you sure you want to reset PRC Asks?")
                    }
                >
                    Reset PRC Asks
                </Button>
                <Button
                    onClick={() =>
                        openResetModal(
                            handleResetPRCConcessions,
                            "Reset PRC Concessions",
                            "Are you sure you want to reset PRC Concessions?"
                        )
                    }
                >
                    Reset PRC Concessions
                </Button>
                <Button
                    onClick={() =>
                        openResetModal(
                            handleResetJointAgreements,
                            "Reset Joint Agreements",
                            "Are you sure you want to reset Joint Agreements?"
                        )
                    }
                >
                    Reset Joint Agreements
                </Button>
            </Container>

            <h3>Debugging</h3>
            <Container>
                <Button onClick={handlePollConnections}>Get Connections</Button>
                <Button onClick={handlePollGroups}>Get Groups</Button>
            </Container>

            <br/>
            <a href="/instructor">Instructor Home</a>

            {/* Confirmation Modal for Reset Actions */}
            <Modal show={showResetModal} onHide={closeResetModal}>
                <Modal.Header closeButton>
                    <Modal.Title>{resetModalTitle}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>{resetModalMessage}</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={closeResetModal}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={confirmResetAction}>
                        Confirm
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorSimulationRoomPage;