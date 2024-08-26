using System.ComponentModel;

namespace AirportBudget.Server.Enums;

public enum MyEntityType
{
    [Description("預算金額")]
    BudgetAmount = 301, // 可以擴充為其他表，例如 User, Transaction 等等
    [Description("使用者")]
    User = 302,
}