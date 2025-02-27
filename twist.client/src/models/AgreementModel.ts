import AgreementTypeEnum from "../enums/AgreementTypeEnum";
import CountryEnum from "../enums/CountryEnum";

export default interface AgreementModel {
    agreement_id: number;
    simulation_id: number;
    type: AgreementTypeEnum;
    type_id: number;
    description: string;
    points: number;
    country: CountryEnum;
}