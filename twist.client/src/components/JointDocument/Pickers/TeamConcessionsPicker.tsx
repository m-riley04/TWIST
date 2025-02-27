import { Table } from "react-bootstrap";
import ConcessionModel from "../../../models/ConcessionModel";
import TeamPickerItem from "./TeamPickerItem";

interface TeamConcessionsPickerProps {
    concessions: ConcessionModel[];
    onAddClicked: (id: number) => void;
};

const TeamConcessionsPicker: React.FC<TeamConcessionsPickerProps> = ({
    concessions,
    onAddClicked
}) => {

    return (
        <>
            <h2>Team Concessions</h2>
            <Table className="picker">
                <thead>
                    <tr>
                        <th>Points</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {concessions.map((item, index) => (
                        <TeamPickerItem
                            key={item.concession_id}
                            id={item.concession_id}
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

export default TeamConcessionsPicker;
