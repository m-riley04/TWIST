import { Button, Dropdown, DropdownItem } from 'react-bootstrap';
import type ParticipantModel from '../../models/ParticipantModel';
import "./style.scss";

const ROLES: string[] = ["Protectionist", "Pro-Trade", "Security"];
const COUNTRIES: string[] = ["USA", "China"];

const ParticipantListItem = ({ participant, onKickClicked }: { participant: ParticipantModel, onKickClicked: () => void }) => {
    return (
        <div className="participantListItem" >
            <p>{participant.participant_id}</p>
            <p>{participant.username}</p>
            <p>{participant.email}</p>
            <Dropdown>{COUNTRIES.map((c, i) => <DropdownItem key={i}>{c}</DropdownItem>)}</Dropdown>
            <Dropdown>{ROLES.map((r, i) => <DropdownItem key={i}>{r}</DropdownItem>)}</Dropdown>
            <Button onClick={onKickClicked}>Kick</Button>
        </div>
    );
}

export default ParticipantListItem;