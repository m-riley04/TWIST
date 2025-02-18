import CountryEnum from "../enums/CountryEnum";
import RoleEnum from "../enums/RoleEnum";

export default interface ParticipantModel {
    participant_id: number;
    country?: CountryEnum;
    role?: RoleEnum;
    simulation_id: number;
    username: string;
    email: string;
};