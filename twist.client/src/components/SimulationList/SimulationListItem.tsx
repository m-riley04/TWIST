import type SimulationModel from '../../models/SimulationModel';
import "./style.scss";

const SimulationsList = ({ simulation }: { simulation: SimulationModel }) => {

    return (
        <div className="simulationListItem" >
            <p>{simulation.name}</p>
            <p>{simulation.round}</p>

            <button>View</button>
        </div>
    );
}

export default SimulationsList;