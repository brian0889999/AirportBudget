using System.ComponentModel;

namespace AirportBudget.Server.Enums;

public enum UserSystemType
{
    [Description("土木")]
    CivilEngineering = 101,
    [Description("水電")]
    ElectricalPlumbing = 102,
    [Description("建築")]
    Architecture = 103,
    [Description("綜合")]
    Comprehensive = 104,
    [Description("機械")]
    Mechanical = 105,
}