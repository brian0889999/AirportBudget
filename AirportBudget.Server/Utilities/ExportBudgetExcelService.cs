using AirportBudget.Server.Enums;
using AirportBudget.Server.Interfaces.Repositorys;
using AirportBudget.Server.Models;
using Microsoft.EntityFrameworkCore;
using AirportBudget.Server.ViewModels;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using LinqKit;
using AirportBudget.Server.DTOs;
using AutoMapper;
using NPOI.SS.Formula.Functions;
//using System.Text.RegularExpressions;

namespace AirportBudget.Server.Utilities
{
    public  class ExportBudgetExcelService
    {
        private readonly IGenericRepository<Budget> _budgetRepository;
        private readonly IGenericRepository<BudgetAmount> _budgetAmountRepository;
        private readonly IGenericRepository<Group> _groupRepository;
        private readonly IMapper _mapper;
        public ExportBudgetExcelService(
        IGenericRepository<Budget> budgetRepository,
        IGenericRepository<BudgetAmount> budgetAmountRepository,
        IGenericRepository<Group> groupRepository,
        IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _budgetAmountRepository = budgetAmountRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
        }
        public byte[] ExportBudgetToExcel(ExportBudgetRequestViewModel request)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            
            sheet.DefaultRowHeight = 30 * 30;

            // 定義每一欄的寬度
            int[] columnWidths = { 20, 20, 20, 30, 25, 20, 20, 20, 20, 45, 20, 35, 35, 25 };

            // 設定每一欄的寬度
            for (int i = 0; i < columnWidths.Length; i++)
            {
                sheet.SetColumnWidth(i, columnWidths[i] * 256);
            }

            // 找出當月的最後一天
            DateTime lastDayOfMonth = new DateTime(request.Year, request.EndMonth, DateTime.DaysInMonth(request.Year, request.EndMonth));

            // 將日期格式化為 "YYYY年MM月DD日" 的格式
            string formattedDate = $"{request.Year}年{request.EndMonth}月{lastDayOfMonth.Day}日";

            // 設定標題
            var titles = new[]
            {
            "預算控制執行情形表", $"{formattedDate}止"
            };

            for (int i = 0; i < titles.Length; i++)
            {
                var row = sheet.CreateRow(i);
                row.Height = 30 * 20; // 設定行高
                var cell = row.CreateCell(0);
                cell.SetCellValue(titles[i]);
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, 11)); // 合併A~L欄
            }

            // 在第三列的M欄位放置標題
            var thirdRow = sheet.CreateRow(2);
            thirdRow.Height = 30 * 20; // 設定行高
            var titleCell = thirdRow.CreateCell(12); // M欄位對應的索引是12
            titleCell.SetCellValue("單位 : 元");

            // 在第四列A~N欄位放置標題
            var fourthRow = sheet.CreateRow(3);
            fourthRow.Height = 30 * 20; // 設定行高
            var columnTitles = new[]
            {
        "組室別", "6級(科目)", "7級(子目)", "8級(細目)", "年度預算額度(1)", "併決算數(2)", "一般動支數(3)", "勻出數(4)", "可用預算餘額 \r\n(5)=(1)+(2)-(3)-(4)", "勻入數(6)", "勻入實付數(7)", "勻入數餘額 \r\n(8)=(6)-(7)", "可用預算餘額 \r\n(含勻入) \r\n(10) = (5) + (8)", "本科目實付數(9)"
            };

            for (int i = 0; i < columnTitles.Length; i++)
            {
                var cell = fourthRow.CreateCell(i);
                cell.SetCellValue(columnTitles[i]);
            }

            // 獲取所有的群組資料
            var groups = GetGroup();

            // 獲取所有的 Subject6 值
            var subject6s = GetSubject6(request.GroupId, request.Year);

            int currentRow = 4; // 從第五行開始填充資料

            // 總計所有 Subject6 的欄位
            int grandTotalAnnualBudgetAmount = 0;
            int grandTotalFinalBudgetAmount = 0;
            int grandTotalGeneral = 0;
            int grandTotalOut = 0;
            int grandTotalUseBudget = 0;
            int grandTotalIn = 0;
            int grandTotalInActual = 0;
            int grandTotalInBalance = 0;
            int grandTotalInUseBudget = 0;
            int grandTotalSubjectActual = 0;

            foreach (var subject6 in subject6s)
            {
                // 根據 Subject6 獲取所有 BudgetId
                var budgetIds = GetBudgetId(request.GroupId, request.Year, subject6);

                // 用於計算每個 Subject6 的各欄位總和
                int totalAnnualBudgetAmount = 0;
                int totalFinalBudgetAmount = 0;
                int totalGeneral = 0;
                int totalOut = 0;
                int totalUseBudget = 0;
                int totalIn = 0;
                int totalInActual = 0;
                int totalInBalance = 0;
                int totalInUseBudget = 0;
                int totalSubjectActual = 0;

                foreach (var budgetId in budgetIds)
                {
                    // 根據 BudgetId 獲取 BudgetAmount 資料
                    var budgetAmountData = GetBudgetAmountData(budgetId, request.Year);
                    // 在這裡進行排序
                    //var sortedData = budgetAmountData
                    //    .OrderBy(data => int.Parse(data.Subject7.Substring(0, 4)))
                    //    .ToList();
                    foreach (var data in budgetAmountData)
                    {
                        var row = sheet.CreateRow(currentRow++);
                        row.Height = 30 * 20; // 設定行高

                        // 找出對應的 GroupName
                        var group = groups.FirstOrDefault(g => g.GroupId == request.GroupId);
                        var groupName = group != null ? group.GroupName : "未知";

                        row.CreateCell(0).SetCellValue(groupName);
                        row.CreateCell(1).SetCellValue(data.Subject6);
                        row.CreateCell(2).SetCellValue(data.Subject7);
                        row.CreateCell(3).SetCellValue(data.Subject8);
                        row.CreateCell(4).SetCellValue(data.AnnualBudgetAmount.ToString("N0"));
                        row.CreateCell(5).SetCellValue(data.FinalBudgetAmount.ToString("N0"));
                        row.CreateCell(6).SetCellValue(data.General.ToString("N0"));
                        row.CreateCell(7).SetCellValue(data.Out.ToString("N0"));
                        row.CreateCell(8).SetCellValue(data.UseBudget.ToString("N0"));
                        row.CreateCell(9).SetCellValue(data.In.ToString("N0"));
                        row.CreateCell(10).SetCellValue(data.InActual.ToString("N0"));
                        row.CreateCell(11).SetCellValue(data.InBalance.ToString("N0"));
                        row.CreateCell(12).SetCellValue(data.InUseBudget.ToString("N0"));
                        row.CreateCell(13).SetCellValue(data.SubjectActual.ToString("N0"));

                        // 累計當前 Subject6 的所有欄位
                        totalAnnualBudgetAmount += data.AnnualBudgetAmount;
                        totalFinalBudgetAmount += data.FinalBudgetAmount;
                        totalGeneral += data.General;
                        totalOut += data.Out;
                        totalUseBudget += data.UseBudget;
                        totalIn += data.In;
                        totalInActual += data.InActual;
                        totalInBalance += data.InBalance;
                        totalInUseBudget += data.InUseBudget;
                        totalSubjectActual += data.SubjectActual;
                    }
                }
                // 在每個 Subject6 資料結束後，添加一行顯示加總結果
                var totalRow = sheet.CreateRow(currentRow++);
                totalRow.Height = 30 * 20; // 設定行高
                totalRow.CreateCell(3).SetCellValue("合計"); // D 欄位為標題
                totalRow.CreateCell(4).SetCellValue(totalAnnualBudgetAmount.ToString("N0")); // E 欄位為加總值
                totalRow.CreateCell(5).SetCellValue(totalFinalBudgetAmount.ToString("N0"));
                totalRow.CreateCell(6).SetCellValue(totalGeneral.ToString("N0"));
                totalRow.CreateCell(7).SetCellValue(totalOut.ToString("N0"));
                totalRow.CreateCell(8).SetCellValue(totalUseBudget.ToString("N0"));
                totalRow.CreateCell(9).SetCellValue(totalIn.ToString("N0"));
                totalRow.CreateCell(10).SetCellValue(totalInActual.ToString("N0"));
                totalRow.CreateCell(11).SetCellValue(totalInBalance.ToString("N0"));
                totalRow.CreateCell(12).SetCellValue(totalInUseBudget.ToString("N0"));
                totalRow.CreateCell(13).SetCellValue(totalSubjectActual.ToString("N0"));

                // 累計到總計
                grandTotalAnnualBudgetAmount += totalAnnualBudgetAmount;
                grandTotalFinalBudgetAmount += totalFinalBudgetAmount;
                grandTotalGeneral += totalGeneral;
                grandTotalOut += totalOut;
                grandTotalUseBudget += totalUseBudget;
                grandTotalIn += totalIn;
                grandTotalInActual += totalInActual;
                grandTotalInBalance += totalInBalance;
                grandTotalInUseBudget += totalInUseBudget;
                grandTotalSubjectActual += totalSubjectActual;
            }

            // 添加最下面的總計行
            var grandTotalRow = sheet.CreateRow(currentRow++);
            grandTotalRow.Height = 30 * 20; // 設定行高
            grandTotalRow.CreateCell(3).SetCellValue("總計"); // D 欄位為標題
            grandTotalRow.CreateCell(4).SetCellValue(grandTotalAnnualBudgetAmount.ToString("N0"));
            grandTotalRow.CreateCell(5).SetCellValue(grandTotalFinalBudgetAmount.ToString("N0"));
            grandTotalRow.CreateCell(6).SetCellValue(grandTotalGeneral.ToString("N0"));
            grandTotalRow.CreateCell(7).SetCellValue(grandTotalOut.ToString("N0"));
            grandTotalRow.CreateCell(8).SetCellValue(grandTotalUseBudget.ToString("N0"));
            grandTotalRow.CreateCell(9).SetCellValue(grandTotalIn.ToString("N0"));
            grandTotalRow.CreateCell(10).SetCellValue(grandTotalInActual.ToString("N0"));
            grandTotalRow.CreateCell(11).SetCellValue(grandTotalInBalance.ToString("N0"));
            grandTotalRow.CreateCell(12).SetCellValue(grandTotalInUseBudget.ToString("N0"));
            grandTotalRow.CreateCell(13).SetCellValue(grandTotalSubjectActual.ToString("N0"));


            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }

        }

        private List<Group> GetGroup()
        {
            var result = _groupRepository.GetAll().ToList();

            return (result);
        }

        private List<string> GetSubject6(int groupId, int year)
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(b => b.GroupId == groupId && b.CreatedYear == year);

            // 使用 LINQ 查詢篩選條件
            var subject6s = _budgetRepository.GetByCondition(condition)
                                             .Select(b => b.Subject6)  // 只選取 Subject6 欄位
                                             .Distinct()               // 去重複
                                             .ToList();                // 轉換為 List
            return subject6s;
        }

        private List<int> GetBudgetId(int groupId, int year, string subject6)
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(b => b.GroupId == groupId && b.CreatedYear == year && b.Subject6 == subject6);

            var budgetId = _budgetRepository.GetByCondition(condition)
                                             .Select(b => b.BudgetId)
                                             .ToList();
            return budgetId;
        }

        private List<ExportBudgetAmountDTO> GetBudgetAmountData(int budgetId, int year)
        {
            Expression<Func<BudgetAmount, bool>> condition = item => true;
            condition = condition.And(BudgetAmount => BudgetAmount.Status != null && (BudgetAmount.Status.Trim() == "O" || BudgetAmount.Status.Trim() == "C"));
            condition = condition.And(BudgetAmount => BudgetAmount.IsValid == true && BudgetAmount.CreatedYear == year);

            var results = _budgetAmountRepository.GetByCondition(condition)
               .Include(b => b.Budget) // 確保包含 Budget 表資料
               .Where(b => b.BudgetId == budgetId)
               .GroupBy(b => new
               {
                   b.Budget!.Subject6,
                   b.Budget.Subject7,
                   b.Budget.Subject8,
                   b.Budget.AnnualBudgetAmount,
                   b.Budget.FinalBudgetAmount,
                   b.Budget.BudgetName,
               })
               .Select(g => new ExportBudgetAmountDTO
               {
                   Subject6 = g.Key.Subject6,
                   Subject7 = g.Key.Subject7,
                   Subject8 = g.Key.Subject8,
                   AnnualBudgetAmount = g.Key.AnnualBudgetAmount,
                   FinalBudgetAmount = g.Key.FinalBudgetAmount,
                   General = g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0),
                   Out = g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0),
                   UseBudget = (g.Key.AnnualBudgetAmount -
                               g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0) - 
                               g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0)) + g.Key.FinalBudgetAmount,
                   In = g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0),
                   InActual = g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0),
                   InBalance = g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0),
                   SubjectActual = g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0) + g.Sum(b => b.Type == AmountType.Ordinary ? b.PaymentAmount : 0),
                   InUseBudget = g.Key.AnnualBudgetAmount - g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0) +
                                 g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0),
                   END = g.Key.AnnualBudgetAmount + g.Key.FinalBudgetAmount - g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0) + g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0)
               })
               .ToList();

            //List<ExportBudgetAmountDTO> ExportBudgetAmountDTO = _mapper.Map<List<ExportBudgetAmountDTO>>(results);

            return results;
        }

    }
}
