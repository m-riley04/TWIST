import { Table } from "react-bootstrap";
import ConcessionsDocumentItem from "./ConcessionsDocumentItem";
import update from 'immutability-helper';
import { useCallback, useState } from "react";
import CountryEnum from "../../enums/CountryEnum";
import SimulationModel from "../../models/SimulationModel";
import { HubConnection } from "@microsoft/signalr";
import ParticipantModel from "../../models/ParticipantModel";
import { useEffect } from "react";
import { getConcessionsBySimAndCountry } from "../../server/simulation_management";
import ConcessionModel from "../../models/ConcessionModel";

interface ConcessionsDocumentProps {
    simulation: SimulationModel;
    participant: ParticipantModel;
    connection: HubConnection;
};

const ConcessionsDocument: React.FC<ConcessionsDocumentProps> = ({
    simulation,
    participant,
    connection
}) => {
    const [concessions, setConcessions] = useState<ConcessionModel[]>([]);

    // Load concessions from the database
    useEffect(() => {
        // Check for nulls
        if (!participant) return;
        if (!simulation) return;

        getConcessionsBySimAndCountry(simulation.simulation_id, participant.country)
            .then((data) => setConcessions(data ?? []))
    }, [simulation, participant]);

    useEffect(() => {
        if (!connection) return;

        const onConcessionsUpdated = (updatedConcessions: ConcessionModel[]) => {
            console.log("Received updated concessions from hub", updatedConcessions);
            setConcessions(updatedConcessions);
        };

        connection.on("ConcessionsUpdated", onConcessionsUpdated);

        return () => {
            connection.off("ConcessionsUpdated", onConcessionsUpdated);
        };
    }, [connection]);

    // Function to broadcast the updated concessions list to all clients in this team/group.
    const broadcastConcessions = useCallback((newConcessions: ConcessionModel[]) => {
        console.log("Broadcasting updated concessions to hub");
        connection.invoke("ConcessionsUpdated", simulation, participant, newConcessions)
            .catch(console.error);
    }, [simulation, participant, connection]);
    const broadcastConcessionChanged = useCallback((newConcession: ConcessionModel) => {
        connection.invoke("ConcessionsUpdated", simulation, participant, newConcession)
            .catch(console.error);
    }, [simulation, participant, connection]);

    // Reorder the concessions array when an item is dragged
    const moveItem = useCallback(
        (dragIndex: number, hoverIndex: number) => {
            setConcessions(prevItems => {
                const newConcessions = update(prevItems, {
                    $splice: [
                        [dragIndex, 1],
                        [hoverIndex, 0, prevItems[dragIndex]],
                    ],
                })
                broadcastConcessions(newConcessions);
                return newConcessions;
            });
        },
        [setConcessions, broadcastConcessions]
    );

    const handlePointsChange = useCallback((id: number, newPoints: number) => {
        setConcessions((prev) =>
            prev.map((concession) =>
                concession.concession_id === id ? { ...concession, points: newPoints } : concession
            )
        );
        // Get the updated list (you could also use the functional update from above)
        const updatedConcessions = concessions.map(concession => {
            if (concession.concession_id === id) {
                const newConcession: ConcessionModel = { ...concession, points: newPoints, modified_date: new Date() }
                broadcastConcessionChanged(concession);
                return newConcession
            } else return concession;
            }
        );
        broadcastConcessions(updatedConcessions);
    }, [concessions, setConcessions, broadcastConcessions, broadcastConcessionChanged]);

    const totalPoints = concessions.reduce((acc, item) => acc + item.points, 0);

    return (
        <>
            <h2>Concessions</h2>
            <p
                style={totalPoints != 100 ? { color: "red" } : {}}
            >Total Points: {totalPoints}</p>
            <Table className="participant-list">
                <thead>
                    <tr>
                        <th>Points</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody>
                    {concessions.map((item, index) => (
                        <ConcessionsDocumentItem
                            key={item.concession_id}
                            id={item.concession_id}
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

export default ConcessionsDocument;
