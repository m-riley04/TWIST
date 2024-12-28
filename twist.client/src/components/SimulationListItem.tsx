import type SimulationModel from '../models/SimulationModel';

const SimulationsList = ({ simulation }: { simulation: SimulationModel }) => {

    return (
        <div>
            <p>Name: {simulation.name}</p>
            <p>Round: {simulation.round}</p>

            <button>View</button>
        </div>
    );
}

export default SimulationsList;