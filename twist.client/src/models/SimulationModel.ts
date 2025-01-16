import RoundEnum from "../enums/RoundEnum";
import SimulationStateEnum from "../enums/SimulationStateEnum";

export default interface SimulationModel {
    simulation_id: number;
    code: string;
    name: string;
    start_date: Date;
    end_date?: Date;
    modified_date?: Date;
    round: RoundEnum;
    state: SimulationStateEnum;
    active: boolean;
    instructor_id: number;
};