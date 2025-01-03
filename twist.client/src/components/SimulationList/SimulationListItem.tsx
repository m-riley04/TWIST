import { Button, FormCheck } from 'react-bootstrap';
import type SimulationModel from '../../models/SimulationModel';
import "./style.scss";

const SimulationsList = ({ simulation }: { simulation: SimulationModel }) => {

    return (
        <div className="simulationListItem" >
            <p>{simulation.name}</p>
            <p>{simulation.round}</p>
            <FormCheck checked={simulation.active} style={{ pointerEvents: 'none' }} />
            <Button onClick={() => window.location.assign(`/instructor/room/${simulation.code}`)}>Open</Button>
        </div>
    );
}

export default SimulationsList;