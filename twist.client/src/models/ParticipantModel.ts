import CountryEnum from "../enums/CountryEnum";
import TeamRoleEnum from "../enums/TeamRoleEnum";

export default interface ParticipantModel {
    participant_id: number;
    country?: CountryEnum;
    role?: TeamRoleEnum;
    simulation_id: number;
    username: string;
    email: string;
    connection_id?: string;
};