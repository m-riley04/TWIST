import { Table } from "react-bootstrap";
import ConcessionModel from "../../../models/ConcessionModel";
import TeamPickerItem from "./TeamPickerItem";
import { useState } from "react";
import { useEffect } from "react";

interface TeamConcessionsPickerProps {
    concessions: ConcessionModel[];
    onAddClicked: (id: number) => void;
};

const TeamConcessionsPicker: React.FC<TeamConcessionsPickerProps> = ({
    concessions,
    onAddClicked
}) => {

    const [sortedConcessions, setSortedConcessions] = useState<ConcessionModel[]>([]);

    useEffect(() => {
        const _ = [...concessions];
        _.sort((a, b) => b.points - a.points);
        setSortedConcessions(_)
    }, [concessions])

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
                    {sortedConcessions.map((item, index) => (
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
