import CountryEnum from '../../enums/CountryEnum';
import RoleEnum from '../../enums/RoleEnum';
import ParticipantModel from '../../models/ParticipantModel';
import ParticipantListItem from './ParticipantListItem';

type ParticipantListProps = {
    participants: ParticipantModel[];
    onKickClicked: (participant: ParticipantModel, index: number) => void;
    onCountryChanged: (participant: ParticipantModel, country: CountryEnum) => void;
    onRoleChanged: (participant: ParticipantModel, role: RoleEnum) => void;
};

const ParticipantList: React.FC<ParticipantListProps> = ({ participants, onKickClicked, onCountryChanged, onRoleChanged }) => {

    return (
        <div>
            {participants.map((p, i) => (<ParticipantListItem
                key={i}
                participant={p}
                onKickClicked={() => onKickClicked(p, i)}
                onCountryChanged={(country) => onCountryChanged(p, country)}
                onRoleChanged={(role) => onRoleChanged(p, role)}
            />))}
        </div>
    );
}

export default ParticipantList;