import CountryEnum from "../enums/CountryEnum";

export default interface AskModel {
    ask_id: number;
    simulation_id: number;
    description: string;
    points: number;
    status: number;
    creation_date: Date;
    modified_date: Date;
    country: CountryEnum;
    last_editor?: number;
}