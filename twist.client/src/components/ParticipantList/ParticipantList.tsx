import ParticipantModel from '../../models/ParticipantModel';
import ParticipantListItem from './ParticipantListItem';
import CountryEnum from '../../enums/CountryEnum';
import RoleEnum from '../../enums/RoleEnum';
import { Table } from "react-bootstrap";

interface ParticipantListProps {
    participants: ParticipantModel[];
    onKickClicked: (participant: ParticipantModel, index: number) => void;
    onCountryChanged: (participant: ParticipantModel, country: CountryEnum) => void;
    onRoleChanged: (participant: ParticipantModel, role: RoleEnum) => void;
};

const ParticipantList: React.FC<ParticipantListProps> = ({
    participants,
    onKickClicked,
    onCountryChanged,
    onRoleChanged
}) => {
    return (
        <Table className="participant-list">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Username</th>
                    <th>Email</th>
                    <th>Country</th>
                    <th>Role</th>
                    <th>Action</th>
                </tr>
            </thead>
            <tbody>
                {participants.map((p, i) => (
                    <ParticipantListItem
                        key={p.participant_id}
                        participant={p}
                        onKickClicked={() => onKickClicked(p, i)}
                        onCountryChanged={(country) => onCountryChanged(p, country)}
                        onRoleChanged={(role) => onRoleChanged(p, role)}
                    />
                ))}
            </tbody>
        </Table>
    );
};

export default ParticipantList;
