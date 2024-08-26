using System.ComponentModel;

namespace AirportBudget.Server.Enums;

public enum ActionType
{
    [Description("無操作")]
    None = 200,
    [Description("新增")]
    Insert = 201,
    [Description("修改")]
    Update = 202,
    [Description("刪除")]
    Delete = 203,
}