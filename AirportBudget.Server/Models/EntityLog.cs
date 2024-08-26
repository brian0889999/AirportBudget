using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirportBudget.Server.Enums;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AirportBudget.Server.Models
{
    public class EntityLog
    {
        [Key]
        public int EntityLogId { get; set; }

        public int EntityId { get; set; } // 改為代表資料的 Id
        public MyEntityType EntityType { get; set; } // 用來表示 Id 所屬的資料表

        [StringLength(50)]
        public ActionType ActionType { get; set; } // 新增: 'Insert', 修改: 'Update', 刪除: 'Delete'

        [StringLength(100)]
        public string ChangedBy { get; set; } = string.Empty; // 變更的使用者ID

        public DateTime ChangeTime { get; set; } // 變更時間

        public string Values { get; set; } = string.Empty; // JSON 格式的資料紀錄
        //public string? OldValues { get; set; } = string.Empty; // JSON 格式的舊資料紀錄

        //public string? NewValues { get; set; } = string.Empty; // JSON 格式的新資料紀錄
    }
}
