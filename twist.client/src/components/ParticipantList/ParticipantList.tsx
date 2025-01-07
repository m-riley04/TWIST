import ParticipantModel from '../../models/ParticipantModel';
import ParticipantListItem from './ParticipantListItem';

const ParticipantList = ({ participants }: { participants: ParticipantModel[] }) => {

    return (
        <div>
            {participants.map((p, i) => (<ParticipantListItem participant={p} key={i} />))}
        </div>
    );
}

export default ParticipantList;