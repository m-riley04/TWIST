import { useAuth0 } from "@auth0/auth0-react";
import { Button, Container, Dropdown, DropdownButton } from "react-bootstrap";
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
const WEB_DOMAIN = "localhost:5173";

const InstructorSimulationRoomPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect } = useAuth0();

    const [simulation, setSimulation] = useState<SimulationModel>();
    const [participants, setParticipants] = useState<ParticipantModel[]>([]);
    const [isStarted, setIsStarted] = useState<boolean>(false);

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

        // TODO: Add QR code for participants to join
    }, [simCode]);

    // Initialize participants
    useEffect(() => {
        if (!simulation) return;

        // Get the participants
        getParticipants(simulation.simulation_id)
            .then(data => setParticipants(data))
            .catch(error => console.error(`Unable to load participants: ${error}`));

    }, [simulation])

    // Initialize connection
    useEffect(() => {
        // Check if connection is defined
        if (!connection) return;

        if (connection.state === "Connected") {
            console.log("Connected to SignalR hub.");
        }

        // Connection signals/slots
        connection.on("ParticipantJoined", (participant: ParticipantModel) => {
            // Check if participant is already in list
            if (participants.find((p) => p.participant_id === participant.participant_id)) {
                console.error(`Participant '${participant.username}' already in list`);
                return;
            }

            // Add to participants list
            setParticipants((prev) => [...prev, participant]);
            console.log(`New participant joined:`, participant.username);
        });

        connection.on("ParticipantLeft", (participant: ParticipantModel) => {
            // Remove from participants list
            setParticipants((prev) => prev.filter((p) => p.participant_id !== participant.participant_id));
            console.log(`Participant left:`, participant.username);
        });

        connection.on("ParticipantDisconnected", (participant: ParticipantModel) => {
            // Update the participant list
            setParticipants((prev) => prev.map((p) => {
                if (p.participant_id === participant.participant_id) {
                    p.connection_id = participant.connection_id;
                }
                return p;
            }));
        });

        connection.on("SimulationStarted", (simulation: SimulationModel ) => {
            // TODO
        })

        // Cleanup
        return () => {
            connection.stop().catch(console.error);
        }

    }, [connection]);

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
            .then(() => {
                setIsStarted(true);
                console.log("Simulation started.")
            })
            .catch((error) => console.error(`Unable to start simulation: ${error}`));
    }

    const handleStopSimulation = () => {
    const handleKickParticipant = (participant: ParticipantModel) => {
        if (connection === undefined) {
            console.error("Unable to kick participant: No connection to hub.");
            return;
        }

        connection.invoke("KickParticipant", simulation, participant)
            .then(() => {
                // Remove participant from list
                setParticipants((prev) => prev.filter((p) => p.email !== participant.email));
                console.log(`Kicked participant '${participant.email}'`);
            })
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
        connection?.invoke("RandomlyAssignCountries", simulation)
            .catch((error) => console.error(`Unable to randomly assign countries: ${error}`));
    }

    const handleRandomlyAssignRole = () => {
        connection?.invoke("RandomlyAssignRoles", simulation)
            .catch((error) => console.error(`Unable to randomly assign roles: ${error}`));
    }

    const handleUpdateRound = (newRound: RoundEnum) => {
        connection?.invoke("UpdateRound", simulation, newRound)
            .then(() => {
                setSimulation((prev) => {
                    if (!prev) return; // Null check

                    return ({ ...prev, round: newRound });
                });
                console.log(`Updated to round ${newRound}`)
            })
            .catch((error) => console.error(`Unable to update round: ${error}`));
    }

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

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
            
            <Button onClick={handleStartSimulation}>Start Simulation</Button>
            <Button onClick={handleStopSimulation} variant="warning">Stop Simulation</Button>
            <Button onClick={handleCloseRoom} variant="danger">Close Room</Button>
            <br/>
            <a href="/instructor">Instructor Home</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorSimulationRoomPage;