enum CountryEnum {
    NONE=0,
    USA,
    PRC
}

export const countries: string[] = ["None", "USA", "PRC"];

export const CountryStringMap: Map<CountryEnum, string> = new Map([
    [CountryEnum.NONE, "None"],
    [CountryEnum.USA, "USA"],
    [CountryEnum.PRC, "PRC"],
]);

export const StringCountryMap: Map<string, CountryEnum> = new Map([
    ["None", CountryEnum.NONE],
    ["USA", CountryEnum.USA],
    ["PRC", CountryEnum.PRC],
]);

export default CountryEnum;