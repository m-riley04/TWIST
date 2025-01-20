import { Table } from "react-bootstrap";
import AsksDocumentItem from "./AsksDocumentItem";
import update from 'immutability-helper';
import { useCallback } from "react";
import { useState } from "react";

// The default USA asks. These are the default concessions for PRC.
const DEFAULT_USA_ASKS = [
    "China to issue an improved nationwide negative list for foreign investment (especially take measures to liberalize the financial sector)."
    , "China to remove or reduce restrictions on foreign investment identified by the U.S."
    , "China to eliminate laws and regulations, such as licensing or procurement, that treat foreign entities less favorably than domestic Chinese firms."
    , "China to remove specified non-tariff barriers on US imports (ex. Onerous permitting, excessive inspections etc)"
    , "China to recognize that the U.S. may impose import quotas and tariffs on products in critical sectors."
    , "China to eliminate specific policies and practices linked to forced technology transfer."
    , "China to strengthen intellectual property protection."
    , "China to increase import of American agricultural products by $30-40 billion."
    , "China to increase import of American energy products by $20-30 billion."
    , "China to commit to the reduction of the trade deficit between China and the U.S.by $100-200 billion by 2020."
    , "China to cease government-sponsored or tolerated cyber espionage and intrusions into U.S. commercial networks."
    , "China to cease subsidies and other forms of assistance that support industries targeted in the MiC 2025 plan and other emerging and strategic industries."
    , "China to establish a high-level dialogue with U.S. to discuss dual-use technologies."
    , "China to refrain from military development of man-made islands in the South China Sea."
    , "China to help identify and discourage Chinese firms that evade U.S. sanctions against Iran and North Korea."
    , "China to guarantee human rights and democracy in Hong Kong"
]

// The default PRC asks. These are the default concessions for USA.
const DEFAULT_PRC_ASKS = [
    "The U.S. to reduce tariffs on Chinese imports to 2017 levels."
    , "The U.S. to lift bans on high technology exports such as integrated circuits and aircraft to China."
    , "The U.S. to agree to a more limited approach in defining its export control regime."
    , "The U.S. to  remove Chinese companies like Huawei from the entities list."
    , "The U.S. to give equal treatment to Chinese companies in national security review (CFIUS)."
    , "The U.S. to open government procurement to Chinese technology products and services."
    , "The U.S. to refrain from restricting visas for Chinese students and professionals."
    , "The U.S. to recognize core Chinese national interests: keeping national unity of mainland China and Tibet, Xinjiang, Hong Kong, and Taiwan."
    , "The U.S. to recognize core Chinese national interests: sovereignty over South China Sea."
    , "The U.S. to agree not to send warships or military personnel to Taiwan."
]

interface Ask {
    id: number;
    text: string;
    points: number;
}

interface AsksDocumentProps {
    def: string
};

const AsksDocument: React.FC<AsksDocumentProps> = () => {
    // Initializes the asks
    const initialAsks: Ask[] = DEFAULT_USA_ASKS.map((str, index) => ({
        id: index,
        text: str,
        points: 0
    }));

    const [asks, setAsks] = useState<Ask[]>(initialAsks);

    // Reorder the asks array when an item is dragged
    const moveItem = useCallback(
        (dragIndex: number, hoverIndex: number) => {
            setAsks(prevItems =>
                update(prevItems, {
                    $splice: [
                        [dragIndex, 1],
                        [hoverIndex, 0, prevItems[dragIndex]],
                    ],
                })
            );
        },
        [setAsks]
    );

    const handlePointsChange = useCallback((id: number, newPoints: number) => {
        setAsks((prev) =>
            prev.map((ask) =>
                ask.id === id ? { ...ask, points: newPoints } : ask
            )
        );
    }, []);

    const totalPoints = asks.reduce((acc, item) => acc + item.points, 0);

    return (
        <Table className="participant-list">
            <thead>
                <tr>
                    <th>Points</th>
                    <th>Description</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                {asks.map((item, index) => (
                    <AsksDocumentItem
                        key={item.id}
                        id={item.id}
                        index={index}
                        description={item.text}
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
