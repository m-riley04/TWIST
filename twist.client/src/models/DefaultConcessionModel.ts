import CountryEnum from "../enums/CountryEnum";

export default interface DefaultConcessionModel {
    default_concession_id: number;
    description: string;
    country: CountryEnum;
    creation_date: Date;
    modified_date: Date;
    
}