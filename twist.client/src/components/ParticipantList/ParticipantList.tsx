import ParticipantModel from '../../models/ParticipantModel';
import ParticipantListItem from './ParticipantListItem';

type ParticipantListProps = {
    participants: ParticipantModel[];
    onKickClicked: (participant: ParticipantModel, index: number) => void;
};

const ParticipantList: React.FC<ParticipantListProps> = ({ participants, onKickClicked }) => {

    return (
        <div>
            {participants.map((p, i) => (<ParticipantListItem participant={p} onKickClicked={() => onKickClicked(p, i)} key={i} />))}
        </div>
    );
}

export default ParticipantList;