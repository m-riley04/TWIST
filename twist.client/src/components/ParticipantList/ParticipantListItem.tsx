import { Button, Dropdown } from 'react-bootstrap';
import type ParticipantModel from '../../models/ParticipantModel';
import "./style.scss";
import CountryEnum, { countries, CountryStringMap, StringCountryMap } from '../../enums/CountryEnum';
import RoleEnum, { roles, RoleStringMap, StringRoleMap } from '../../enums/RoleEnum';

type ParticipantListItemProps = {
    participant: ParticipantModel;
    onKickClicked: () => void;
    onCountryChanged: (country: CountryEnum) => void;
    onRoleChanged: (role: RoleEnum) => void;
};

const ParticipantListItem: React.FC<ParticipantListItemProps> = ({ participant, onKickClicked, onCountryChanged, onRoleChanged }) => {

    return (
        <div className="participantListItem" >
            <p>{participant.participant_id}</p>
            <p>{participant.username}</p>
            <p>{participant.email}</p>
            <Dropdown>
                <Dropdown.Toggle>
                    {CountryStringMap.get(participant.country ?? 0)}
                </Dropdown.Toggle>
                <Dropdown.Menu>
                    {countries.map((c: string, i) => (
                        <Dropdown.Item
                            key={i}
                            eventKey={c}
                            onClick={() => onCountryChanged(StringCountryMap.get(c)!)}>
                            {c}
                        </Dropdown.Item>))}
                </Dropdown.Menu>
            </Dropdown>
            <Dropdown>
                <Dropdown.Toggle>
                    {RoleStringMap.get(participant.role ?? 0)}
                </Dropdown.Toggle>
                <Dropdown.Menu>
                    {roles.map((r, i) => (
                        <Dropdown.Item
                            key={i}
                            eventKey={r}
                            onClick={() => onRoleChanged(StringRoleMap.get(r)!)}>
                            {r}
                        </Dropdown.Item>))}
                </Dropdown.Menu>
            </Dropdown>
            <Button onClick={onKickClicked}>Kick</Button>
        </div>
    );
}

export default ParticipantListItem;