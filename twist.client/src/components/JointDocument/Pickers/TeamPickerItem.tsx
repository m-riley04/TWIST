import { useRef } from "react";
import { Button } from "react-bootstrap";

export interface TeamPickerItemProps {
    id: number;
    index: number;
    points: number;
    description: string;
    onAddClicked: (id: number) => void;
};

const TeamPickerItem: React.FC<TeamPickerItemProps> = ({
    id,
    index,
    points,
    description,
    onAddClicked
}) => {
    const ref = useRef<HTMLTableRowElement>(null);

    return (
        <tr
            className="team-picker-item"
            ref={ref}
        >
            <td>
                <input type="number" value={points} readOnly />
            </td>
            <td>
                {description}
            </td>
            <td>
                <Button onClick={() => onAddClicked(id)} >+</Button>
            </td>
        </tr>
    );
};

export default TeamPickerItem;
