import { Button, FormCheck } from 'react-bootstrap';
import type SimulationModel from '../../models/SimulationModel';
import "./style.scss";

const SimulationsListItem = ({ simulation, onDeleteClicked }: { simulation: SimulationModel, onDeleteClicked: ()=>void }) => {

    return (
        <div className="simulationListItem" >
            <p>{simulation.name}</p>
            <p>{simulation.code}</p>
            <p>{new Date(simulation.start_date).toDateString()}</p>
            <p>{simulation.end_date ? new Date(simulation.end_date).toDateString() : ""}</p>
            <p>{simulation.round}</p>
            <FormCheck checked={simulation.active} readOnly />
            <Button onClick={() => window.location.assign(`/instructor/room/${simulation.code}`)}>Open</Button>
            <Button>Edit</Button>
            <Button variant="danger" onClick={onDeleteClicked}>Delete</Button>
        </div>
    );
}

export default SimulationsListItem;