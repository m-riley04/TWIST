import { Table } from "react-bootstrap";

type ParticipantPageLoadingStatusProps = {
    connection: "connecting" | "connected" | "error";
    simulation: "loading" | "loaded" | "error";
};

const ParticipantList: React.FC<ParticipantPageLoadingStatusProps> = ({
    connection,
    simulation,
}) => {
    return (
        <Table className="participant-list">
            <thead>
                <tr>
                    <th>Component</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>SignalR Connection</td>
                    <td>{connection}</td>
                </tr>
                <tr>
                    <td>Simulation Data</td>
                    <td>{simulation}</td>
                </tr>
            </tbody>
        </Table>
    );
};

export default ParticipantList;
