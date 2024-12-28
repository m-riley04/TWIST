import type SimulationModel from '../../models/SimulationModel';
import "./style.scss";

const SimulationsList = ({ simulation }: { simulation: SimulationModel }) => {

    return (
        <div className="simulationListItem" >
            <p>Name: {simulation.name}</p>
            <p>Round: {simulation.round}</p>

            <button>View</button>
        </div>
    );
}

export default SimulationsList;