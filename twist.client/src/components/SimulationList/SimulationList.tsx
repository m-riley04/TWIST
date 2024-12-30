import type SimulationModel from '../../models/SimulationModel';
import SimulationListItem from './SimulationListItem';

const SimulationList = ({ simulations }: { simulations: SimulationModel[] }) => {

    return (
        <div>
            {simulations.map((sim, i) => (<SimulationListItem simulation={sim} key={i} />))}
        </div>
    );
}

export default SimulationList;