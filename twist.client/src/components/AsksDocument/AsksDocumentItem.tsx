import Draggable from "react-draggable";

interface AsksDocumentItemProps {
    points: number;
    description: string;
};

const AsksDocumentItem: React.FC<AsksDocumentItemProps> = ({
    points,
    description
}) => {

    return (
        <Draggable
            axis="y"
            >
            <tr className="participant-list-item">
                <td>
                    <input type="number" value={points}/>
                </td>
                <td>{description}</td>
            </tr>
        </Draggable>
    );
};

export default AsksDocumentItem;
