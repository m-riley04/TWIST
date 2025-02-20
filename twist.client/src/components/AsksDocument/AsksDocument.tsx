import { Table } from "react-bootstrap";
import AsksDocumentItem from "./AsksDocumentItem";
import update from 'immutability-helper';
import { useCallback, useState } from "react";
import Ask from "../../models/AskModel";
import CountryEnum from "../../enums/CountryEnum";
import SimulationModel from "../../models/SimulationModel";
import { HubConnection } from "@microsoft/signalr";
import ParticipantModel from "../../models/ParticipantModel";
import { useEffect } from "react";
import { getAsksBySimAndCountry } from "../../server/simulation_management";

interface AsksDocumentProps {
    simulation: SimulationModel;
    participant: ParticipantModel;
    connection: HubConnection;
};

const AsksDocument: React.FC<AsksDocumentProps> = ({
    simulation,
    participant,
    connection
}) => {
    const [asks, setAsks] = useState<Ask[]>([]);

    // Load asks from the database
    useEffect(() => {
        if (participant.country === CountryEnum.USA) {
            getAsksBySimAndCountry(simulation.simulation_id, CountryEnum.USA)
                .then((data) => setAsks(data ?? []))
        } else if (participant.country === CountryEnum.PRC) {
            getAsksBySimAndCountry(simulation.simulation_id, CountryEnum.PRC)
                .then((data) => setAsks(data ?? []))
        } else {
            console.error("Error: Participant has no country, so there probably won't be any asks/concessions")
            getAsksBySimAndCountry(simulation.simulation_id, CountryEnum.NONE)
                .then((data) => setAsks(data ?? []))
        }
    }, [simulation, participant]);

    useEffect(() => {
        if (!connection) return;

        const onAsksUpdated = (updatedAsks: Ask[]) => {
            console.log("Received updated asks from hub", updatedAsks);
            setAsks(updatedAsks);
        };

        connection.on("AsksUpdated", onAsksUpdated);

        return () => {
            connection.off("AsksUpdated", onAsksUpdated);
        };
    }, [connection]);

    // Function to broadcast the updated asks list to all clients in this team/group.
    const broadcastAsks = useCallback((newAsks: Ask[]) => {
        console.log("Broadcasting updated asks to hub");
        connection.invoke("AsksUpdated", simulation, participant, newAsks)
            .catch(console.error);
    }, [simulation, participant, connection]);
    const broadcastAskChanged = useCallback((newAsk: Ask) => {
        connection.invoke("AskUpdated", simulation, participant, newAsk)
            .catch(console.error);
    }, [simulation, participant, connection]);

    // Reorder the asks array when an item is dragged
    const moveItem = useCallback(
        (dragIndex: number, hoverIndex: number) => {
            setAsks(prevItems => {
                const newAsks = update(prevItems, {
                    $splice: [
                        [dragIndex, 1],
                        [hoverIndex, 0, prevItems[dragIndex]],
                    ],
                })
                broadcastAsks(newAsks);
                return newAsks;
            });
        },
        [setAsks, broadcastAsks]
    );

    const handlePointsChange = useCallback((id: number, newPoints: number) => {
        setAsks((prev) =>
            prev.map((ask) =>
                ask.ask_id === id ? { ...ask, points: newPoints } : ask
            )
        );
        // Get the updated list (you could also use the functional update from above)
        const updatedAsks = asks.map(ask => {
            if (ask.ask_id === id) {
                const newAsk: Ask = { ...ask, points: newPoints, modified_date: new Date() }
                broadcastAskChanged(ask);
                return newAsk
            } else return ask;
            }
        );
        broadcastAsks(updatedAsks);
    }, [asks, setAsks, broadcastAsks, broadcastAskChanged]);

    const totalPoints = asks.reduce((acc, item) => acc + item.points, 0);

    return (
        <Table className="participant-list">
            <thead>
                <tr>
                    <th>Points</th>
                    <th>Description</th>
                </tr>
            </thead>
            <tbody>
                {asks.map((item, index) => (
                    <AsksDocumentItem
                        key={item.ask_id}
                        id={item.ask_id}
                        index={index}
                        description={item.description}
                        points={item.points}
                        moveItem={moveItem}
                        onPointsChanged={handlePointsChange}
                    />
                ))}
            </tbody>
        </Table>
    );
};

export default AsksDocument;
