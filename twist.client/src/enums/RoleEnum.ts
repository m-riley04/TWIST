enum RoleEnum {
    NONE=0,
    PROTECTIONIST,
    PRO_TRADE,
    SECURITY,
}

export const roles: string[] = ["None", "Protectionist", "Pro-Trade", "Security"];

export const RoleStringMap: Map<RoleEnum, string> = new Map([
    [RoleEnum.NONE, "None"],
    [RoleEnum.PROTECTIONIST, "Protectionist"],
    [RoleEnum.PRO_TRADE, "Pro-Trade"],
    [RoleEnum.SECURITY, "Security"],
]);

export const StringRoleMap: Map<string, RoleEnum> = new Map([
    ["None", RoleEnum.NONE],
    ["Protectionist", RoleEnum.PROTECTIONIST],
    ["Pro-Trade", RoleEnum.PRO_TRADE],
    ["Security", RoleEnum.SECURITY,],
]);

export default RoleEnum;
