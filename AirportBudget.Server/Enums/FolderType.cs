using System.ComponentModel;

namespace MyGisMIS.Server.Enums;

public enum FolderType
{
    [Description("一般")]
    general = 1000,
    [Description("勻出")]
    balanceout = 1001,
    [Description("勻入")]
    balancein = 1002,
}