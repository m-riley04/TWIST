export default interface SimulationModel {
    simulationId: number;
    name: string;
    participants: object[];
    startDate: Date;
    endDate: Date;
    active: boolean;
    responses: object[];
    asks: object[];
    concessions: object[];
    round: number;
    code: string;
};