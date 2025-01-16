import { Button, Dropdown } from 'react-bootstrap';
import type ParticipantModel from '../../models/ParticipantModel';
import CountryEnum, { countries, CountryStringMap, StringCountryMap } from '../../enums/CountryEnum';
import RoleEnum, { roles, RoleStringMap, StringRoleMap } from '../../enums/RoleEnum';

interface ParticipantListItemProps {
    participant: ParticipantModel;
    onKickClicked: () => void;
    onCountryChanged: (country: CountryEnum) => void;
    onRoleChanged: (role: RoleEnum) => void;
};

const ParticipantListItem: React.FC<ParticipantListItemProps> = ({
    participant,
    onKickClicked,
    onCountryChanged,
    onRoleChanged
}) => {

    return (
        <tr className="participant-list-item">
            <td>{participant.participant_id}</td>
            <td>{participant.username}</td>
            <td>{participant.email}</td>
            <td>
                <Dropdown>
                    <Dropdown.Toggle>
                        {CountryStringMap.get(participant.country ?? 0)}
                    </Dropdown.Toggle>
                    <Dropdown.Menu>
                        {countries.map((c: string, i) => (
                            <Dropdown.Item
                                key={i}
                                onClick={() => onCountryChanged(StringCountryMap.get(c)!)}
                            >
                                {c}
                            </Dropdown.Item>
                        ))}
                    </Dropdown.Menu>
                </Dropdown>
            </td>
            <td>
                <Dropdown>
                    <Dropdown.Toggle>
                        {RoleStringMap.get(participant.role ?? 0)}
                    </Dropdown.Toggle>
                    <Dropdown.Menu>
                        {roles.map((r, i) => (
                            <Dropdown.Item
                                key={i}
                                onClick={() => onRoleChanged(StringRoleMap.get(r)!)}
                            >
                                {r}
                            </Dropdown.Item>
                        ))}
                    </Dropdown.Menu>
                </Dropdown>
            </td>
            <td>
                <Button onClick={onKickClicked}>Kick</Button>
            </td>
        </tr>
    );
};

export default ParticipantListItem;
