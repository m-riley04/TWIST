import { Table } from "react-bootstrap";
import JointDocumentItem from "./JointDocumentItem";
import update from 'immutability-helper';
import { useCallback, useState } from "react";
import CountryEnum from "../../enums/CountryEnum";
import SimulationModel from "../../models/SimulationModel";
import ParticipantModel from "../../models/ParticipantModel";
import { HubConnection } from "@microsoft/signalr";
import { useEffect } from "react";
import { getAsksBySimAndCountry, getConcessionsBySimAndCountry } from "../../server/simulation_management";
import AgreementModel from "../../models/AgreementModel";
import AskModel from "../../models/AskModel";
import ConcessionModel from "../../models/ConcessionModel";

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
    }, [simulation, participant]);

    useEffect(() => {
        if (!connection) return;

        const onAgreementsUpdated = (updatedAgreements: AgreementModel[]) => {
            console.log("Received updated agreements from hub", updatedAgreements);
            setAgreements(updatedAgreements);
        };

        connection.on("AgreementsUpdated", onAgreementsUpdated);

        return () => {
            connection.off("AgreementsUpdated", onAgreementsUpdated);
        };
    }, [connection]);

    // Function to broadcast the updated agreements list to all clients in the game.
    const broadcastAgreements = useCallback((newAgreements: AgreementModel[]) => {
        console.log("Broadcasting updated agreements to hub");
        connection.invoke("AgreementsUpdated", simulation, participant, newAgreements)
            .catch(console.error);
    }, [simulation, participant, connection]);
    const broadcastAgreementChanged = useCallback((newAgreement: AgreementModel) => {
        connection.invoke("AgreementUpdated", simulation, participant, newAgreement)
            .catch(console.error);
    }, [simulation, participant, connection]);

    // Reorder the agreements array when an item is dragged
    const moveItem = useCallback(
        (dragIndex: number, hoverIndex: number) => {
            setAgreements(prevItems => {
                const newAgreements = update(prevItems, {
                    $splice: [
                        [dragIndex, 1],
                        [hoverIndex, 0, prevItems[dragIndex]],
                    ],
                })
                broadcastAgreements(newAgreements);
                return newAgreements;
            });
        },
        [setAgreements, broadcastAgreements]
    );

    const handlePointsChange = useCallback((id: number, newPoints: number) => {
        setAgreements((prev) =>
            prev.map((agreement) =>
                agreement.agreement_id === id ? { ...agreement, points: newPoints } : agreement
            )
        );
        // Get the updated list (you could also use the functional update from above)
        const updatedAgreements = agreements.map(agreement => {
            if (agreement.agreement_id === id) {
                const newAgreement: AgreementModel = { ...agreement, points: newPoints, modified_date: new Date() }
                broadcastAgreementChanged(agreement);
                return newAgreement
            } else return agreement;
            }
        );
        broadcastAgreements(updatedAgreements);
    }, [agreements, setAgreements, broadcastAgreements, broadcastAgreementChanged]);

    const totalPoints = agreements.reduce((acc, item) => acc + item.points, 0);

    return (
        <>
            <h2>Joint Agreements</h2>
            <p>Total Points: {totalPoints}</p>
            <Table className="participant-list">
                <thead>
                    <tr>
                        <th>Points</th>
                        <th>Description</th>
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
                            moveItem={moveItem}
                            onPointsChanged={handlePointsChange}
                        />
                    ))}
                </tbody>
            </Table>
        </>
    );
};

export default JointDocument;
