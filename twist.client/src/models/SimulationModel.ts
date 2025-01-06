export default interface SimulationModel {
    simulation_id: number;
    name: string;
    participants: object[];
    start_date: Date;
    end_date: Date | null;
    active: boolean;
    responses: object[];
    asks: object[];
    concessions: object[];
    round: number;
    code: string;
};