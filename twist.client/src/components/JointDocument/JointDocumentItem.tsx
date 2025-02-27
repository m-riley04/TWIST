import { ChangeEvent } from "react";
import { useRef } from "react";
import { Button } from "react-bootstrap";

export interface JointDocumentItemProps {
    id: number;
    index: number;
    points: number;
    description: string;
    country: string;
    type: string;
    onPointsChanged: (id: number, points: number) => void;
    onRemove: (id: number) => void;
};

const JointDocumentItem: React.FC<JointDocumentItemProps> = ({
    id,
    index,
    points,
    description,
    country,
    type,
    onPointsChanged,
    onRemove
}) => {
    const ref = useRef<HTMLTableRowElement>(null);

    // Handlers
    const handlePointsChange = (e: ChangeEvent<HTMLInputElement>) => {
        const newVal = parseInt(e.target.value, 10) || 0;
        onPointsChanged(id, newVal);
    };

    return (
        <tr
            className="joint-agreements-list-item"
            ref={ref}
        >
            <td>
                <input type="number" value={points} onChange={handlePointsChange} />
            </td>
            <td>{description}</td>
            <td>{country}</td>
            <td style={{ color: (type === "A" ? "green" : "red") }} >{type}</td>
            <td>
                <Button variant="danger" onClick={() => onRemove(id)}>-</Button>
            </td>
        </tr>
    );
};

export default JointDocumentItem;
