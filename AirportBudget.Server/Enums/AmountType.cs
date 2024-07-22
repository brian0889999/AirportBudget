using System.ComponentModel;

namespace AirportBudget.Server.Enums;

public enum AmountType
{
    [Description("一般")]
    Ordinary = 1,
    //一般 = 1,
    [Description("勻出")]
    BalanceOut = 2,
    //勻出 = 2,
    [Description("勻入")]
    BalanceIn = 3,
    //勻入 = 3,
}