import TeamRoleEnum from "../enums/TeamRoleEnum";

export default interface ParticipantModel {
    participant_id: number;
    team_id: number;
    role: TeamRoleEnum;
    simulation_id: number;
    username: string;
    email: string;
};