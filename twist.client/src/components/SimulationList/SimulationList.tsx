import type SimulationModel from '../../models/SimulationModel';
import SimulationListItem from './SimulationListItem';

type SimulationListProps = {
    simulations: SimulationModel[];
    onDeleteClicked: (sim: SimulationModel, index: number) => void;
};

const SimulationList: React.FC<SimulationListProps> = ({ simulations, onDeleteClicked }) => {

    return (
        <div>
            {simulations.map((sim, i) => (<SimulationListItem simulation={sim} key={i} onDeleteClicked={() => onDeleteClicked(sim, i)} />))}
        </div>
    );
}

export default SimulationList;