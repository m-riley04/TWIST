import { Container, Table } from "react-bootstrap";
import { useCallback, useState, useEffect } from "react";
import { HubConnection } from "@microsoft/signalr";
import { getAsksBySimAndCountry, getConcessionsBySimAndCountry, getAgreementsBySim } from "../../server/simulation_management";
import JointDocumentItem from "./JointDocumentItem";
import TeamAsksPicker from "./Pickers/TeamAsksPicker";
import TeamConcessionsPicker from "./Pickers/TeamConcessionsPicker";
import AgreementTypeEnum from "../../enums/AgreementTypeEnum";
import CountryEnum from "../../enums/CountryEnum";
import SimulationModel from "../../models/SimulationModel";
import ParticipantModel from "../../models/ParticipantModel";
import AgreementModel from "../../models/AgreementModel";
import AskModel from "../../models/AskModel";
import ConcessionModel from "../../models/ConcessionModel";
import "./style.scss";

interface JointDocumentProps {
    simulation: SimulationModel;
    participant: ParticipantModel;
    connection: HubConnection;
};

const JointDocument: React.FC<JointDocumentProps> = ({
    simulation,
    participant,
    connection
}) => {
    const [agreements, setAgreements] = useState<AgreementModel[]>([]);
    const [teamAsks, setTeamAsks] = useState<AskModel[]>([]);
    const [teamConcessions, setTeamConcessions] = useState<ConcessionModel[]>([]);

    useEffect(() => {
        // Check for nulls
        if (!participant) return;
        if (!simulation) return;

        // Check team
        if (participant.country === CountryEnum.NONE) console.error("Error: Participant has no country, so there probably won't be any asks/concessions");

        // Load asks from the database
        getAsksBySimAndCountry(simulation.simulation_id, participant.country)
            .then((data) => setTeamAsks(data ?? []))

        // Load concessions
        getConcessionsBySimAndCountry(simulation.simulation_id, participant.country)
            .then((data) => setTeamConcessions(data ?? []))

        // Load agreements
        getAgreementsBySim(simulation.simulation_id)
             .then((data) => setAgreements(data ?? []))
    }, [simulation, participant]);

    useEffect(() => {
        if (!connection) return;

        const onAgreementsUpdated = (updatedAgreements: AgreementModel[]) => {
            console.log("Received updated agreements from hub", updatedAgreements);
            setAgreements(updatedAgreements);
        };

        connection.on("JointAgreementsUpdated", onAgreementsUpdated);

        return () => {
            connection.off("JointAgreementsUpdated", onAgreementsUpdated);
        };
    }, [connection]);

    // Function to broadcast the updated agreements list to all clients in the game.
    const broadcastAgreements = useCallback((newAgreements: AgreementModel[]) => {
        console.log("Broadcasting updated agreements to hub");
        connection.invoke("JointAgreementsUpdated", simulation, newAgreements)
            .catch(console.error);
    }, [simulation, connection]);

    const broadcastAgreementChanged = useCallback((newAgreement: AgreementModel) => {
        connection.invoke("JointAgreementUpdated", simulation, newAgreement)
            .catch(console.error);
    }, [simulation, connection]);

    // Handles broadcasting the updated points to the hub and updating the points state
    const handlePointsChange = useCallback((id: number, newPoints: number) => {
        setAgreements(prevAgreements => {
            const updatedAgreements = prevAgreements.map(agreement => {
                if (agreement.agreement_id === id) {
                    const newAgreement = { ...agreement, points: newPoints};
                    broadcastAgreementChanged(newAgreement);
                    return newAgreement;
                }
                return agreement;
            });
            broadcastAgreements(updatedAgreements);
            return updatedAgreements;
        });
    }, [broadcastAgreements, broadcastAgreementChanged]);

    // Handles removing the agreement from the list
    const handleRemove = useCallback((id: number) => {
        console.log(`Attempting to remove agreement ${id} from list...`)

        // Get agreement from list
        const agreement = agreements.find(agreement => agreement.agreement_id === id);

        // Check if the agreement exists
        if (!agreement) {
            console.error(`Agreement not found in list. Unable to remove agreement with id ${id}`);
            return;
        }

        // Invoke the remove agreement method
        connection.invoke("RemoveJointAgreement", simulation, agreement)
            .catch(console.error);
    }, [simulation, connection, agreements]);

    const handleAddAsk = useCallback((id: number) => {
        console.log(`Attempting to add ask ${id} from list...`)

        // Get the ask from the list
        const ask = teamAsks.find(ask => ask.ask_id === id);

        // Check if the ask exists
        if (!ask) {
            console.error(`Ask not found in list. Unable to add ask with id ${id}`);
            return;
        }

        // Create a new agreement
        const newAgreement: AgreementModel = {
            agreement_id: 0,
            simulation_id: simulation.simulation_id,
            type: AgreementTypeEnum.ASK,
            type_id: ask.ask_id,
            description: ask.description,
            points: ask.points,
            country: participant.country,
        };

        // Invoke the add ask method
        connection.invoke("AddJointAgreement", simulation, newAgreement)
            .catch(console.error);
    }, [simulation, participant, connection, teamAsks]);

    const handleAddConcession = useCallback((id: number) => {
        console.log(`Attempting to add concession ${id} from list...`)

        // Get the concession from the list
        const concession = teamConcessions.find(concession => concession.concession_id === id);

        // Check if the concession exists
        if (!concession) {
            console.error(`Concession not found in list. Unable to add concession with id ${id}`);
            return;
        }

        // Create a new agreement
        const newAgreement: AgreementModel = {
            agreement_id: 0,
            simulation_id: simulation.simulation_id,
            type: AgreementTypeEnum.CONCESSION,
            type_id: concession.concession_id,
            description: concession.description,
            points: concession.points,
            country: participant.country,
        };

        // Invoke the add concession method
        connection.invoke("AddJointAgreement", simulation, newAgreement)
            .catch(console.error);
    }, [simulation, participant, connection, teamConcessions]);

    const totalPoints = agreements.reduce((acc, item) => acc + item.points, 0);

    return (
        <>
            <div className="joint-agreements-container">
                <h2>Joint Agreements</h2>
                <p
                    style={totalPoints != 100 ? { color: "red" } : { color: "green" }}
                >Total Points: {totalPoints}</p>
                <Table className="joint-agreements-list">
                    <thead>
                        <tr>
                            <th>Points</th>
                            <th>Description</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {agreements.map((item, index) => (
                            <JointDocumentItem
                                key={item.agreement_id}
                                id={item.agreement_id}
                                index={index}
                                description={item.description}
                                points={item.points}
                                onPointsChanged={handlePointsChange}
                                onRemove={handleRemove}
                            />
                        ))}
                    </tbody>
                </Table>
            </div>

            <div className="picker-container">
                <div className="picker-container">
                    <TeamAsksPicker
                        asks={teamAsks}
                        onAddClicked={handleAddAsk}
                    />
                </div>
                <div className="picker-container">
                    <TeamConcessionsPicker
                        concessions={teamConcessions}
                        onAddClicked={handleAddConcession}
                    />
                </div>
            </div>
        </>
    );
};

export default JointDocument;
