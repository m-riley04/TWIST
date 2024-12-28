import type SimulationModel from '../models/SimulationModel';
import SimulationListItem from './SimulationListItem';

const SimulationList = ({ simulations }: { simulations: SimulationModel[] }) => {

    return (
        <div>
            {simulations.map((sim) => (<SimulationListItem simulation={sim} />))}
        </div>
    );
}

export default SimulationList;