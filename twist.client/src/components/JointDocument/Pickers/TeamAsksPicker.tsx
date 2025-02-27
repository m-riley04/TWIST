import { Table } from "react-bootstrap";
import AskModel from "../../../models/AskModel";
import TeamPickerItem from "./TeamPickerItem";
interface TeamAsksPickerProps {
    asks: AskModel[];
    onAddClicked: (id: number) => void;
};

const TeamAsksPicker: React.FC<TeamAsksPickerProps> = ({
    asks,
    onAddClicked
}) => {

    return (
        <>
            <h2>Team Asks</h2>
            <Table className="picker">
                <thead>
                    <tr>
                        <th>Points</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {asks.map((item, index) => (
                        <TeamPickerItem
                            key={item.ask_id}
                            id={item.ask_id}
                            index={index}
                            description={item.description}
                            points={item.points}
                            onAddClicked={onAddClicked}
                        />
                    ))}
                </tbody>
            </Table>
        </>
    );
};

export default TeamAsksPicker;
