import { Button } from 'react-bootstrap';
import type SimulationModel from '../../models/SimulationModel';
import "./style.scss";

const SimulationsList = ({ simulation }: { simulation: SimulationModel }) => {

    return (
        <div className="simulationListItem" >
            <p>{simulation.name}</p>
            <p>{simulation.round}</p>

            <Button>View</Button>
        </div>
    );
}

export default SimulationsList;