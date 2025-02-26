import CountryEnum from "../enums/CountryEnum";

export default interface DefaultAskModel {
    default_ask_id: number;
    description: string;
    country: CountryEnum;
    creation_date: Date;
    modified_date: Date;
    
}