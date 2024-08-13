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
            
            sheet.DefaultRowHeight = 40 * 30;

            // 定義每一欄的寬度
            int[] columnWidths = { 30, 30, 30, 30, 30, 30, 30, 30, 45, 30, 30, 35, 45, 30 };

            // 設定每一欄的寬度
            for (int i = 0; i < columnWidths.Length; i++)
            {
                sheet.SetColumnWidth(i, columnWidths[i] * 256);
            }

            // 找出當月的最後一天
            DateTime lastDayOfMonth = new DateTime(request.Year, request.EndMonth, DateTime.DaysInMonth(request.Year, request.EndMonth));

            // 將日期格式化為 "YYYY年MM月DD日" 的格式
            string formattedDate = $"{request.Year}年{request.EndMonth}月{lastDayOfMonth.Day}日";

            // 創建文字置中和框線的樣式 (16號字體)
            ICellStyle centeredBorderStyle16 = workbook.CreateCellStyle();
            centeredBorderStyle16.Alignment = HorizontalAlignment.Center;
            centeredBorderStyle16.VerticalAlignment = VerticalAlignment.Center;
            centeredBorderStyle16.BorderBottom = BorderStyle.Thin;
            centeredBorderStyle16.BorderTop = BorderStyle.Thin;
            centeredBorderStyle16.BorderLeft = BorderStyle.Thin;
            centeredBorderStyle16.BorderRight = BorderStyle.Thin;
            IFont font16 = workbook.CreateFont();
            font16.FontHeightInPoints = 16;
            font16.FontName = "Microsoft JhengHei";
            centeredBorderStyle16.SetFont(font16);

            // 創建第四列的文字大小樣式 (14號字體)
            ICellStyle centeredBorderStyle12 = workbook.CreateCellStyle();
            centeredBorderStyle12.Alignment = HorizontalAlignment.Center;
            centeredBorderStyle12.VerticalAlignment = VerticalAlignment.Center;
            centeredBorderStyle12.BorderBottom = BorderStyle.Thin;
            centeredBorderStyle12.BorderTop = BorderStyle.Thin;
            centeredBorderStyle12.BorderLeft = BorderStyle.Thin;
            centeredBorderStyle12.BorderRight = BorderStyle.Thin;
            IFont font12 = workbook.CreateFont();
            font12.FontHeightInPoints = 14;
            //font12.FontHeightInPoints = 12;
            font12.FontName = "PMingLiU";
            centeredBorderStyle12.SetFont(font12);

            // 創建合計和總計的標題樣式（靠右且置中，16號字體）
            ICellStyle rightAlignedCenteredStyle = workbook.CreateCellStyle();
            rightAlignedCenteredStyle.Alignment = HorizontalAlignment.Right;
            rightAlignedCenteredStyle.VerticalAlignment = VerticalAlignment.Center;
            rightAlignedCenteredStyle.BorderBottom = BorderStyle.Thin;
            rightAlignedCenteredStyle.BorderTop = BorderStyle.Thin;
            rightAlignedCenteredStyle.BorderLeft = BorderStyle.Thin;
            rightAlignedCenteredStyle.BorderRight = BorderStyle.Thin;
            rightAlignedCenteredStyle.SetFont(font16);

            // 創建數字欄位的樣式（靠右對齊並置中，16號字體）
            ICellStyle rightAlignedNumberStyle = workbook.CreateCellStyle();
            rightAlignedNumberStyle.Alignment = HorizontalAlignment.Right;
            rightAlignedNumberStyle.VerticalAlignment = VerticalAlignment.Center;
            rightAlignedNumberStyle.BorderBottom = BorderStyle.Thin;
            rightAlignedNumberStyle.BorderTop = BorderStyle.Thin;
            rightAlignedNumberStyle.BorderLeft = BorderStyle.Thin;
            rightAlignedNumberStyle.BorderRight = BorderStyle.Thin;
            rightAlignedNumberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0"); // 設定數字格式
            rightAlignedNumberStyle.SetFont(font16);

            // 創建自動換行的樣式（16號字體）
            ICellStyle wrappedTextStyle = workbook.CreateCellStyle();
            wrappedTextStyle.Alignment = HorizontalAlignment.Center;
            wrappedTextStyle.VerticalAlignment = VerticalAlignment.Center;
            wrappedTextStyle.BorderBottom = BorderStyle.Thin;
            wrappedTextStyle.BorderTop = BorderStyle.Thin;
            wrappedTextStyle.BorderLeft = BorderStyle.Thin;
            wrappedTextStyle.BorderRight = BorderStyle.Thin;
            wrappedTextStyle.WrapText = true; // 啟用自動換行
            wrappedTextStyle.SetFont(font16);

            // 設定標題
            var titles = new[]
            {
            "預算控制執行情形表", $"{formattedDate}止"
        };

            for (int i = 0; i < titles.Length; i++)
            {
                var row = sheet.CreateRow(i);
                row.Height = 40 * 30; // 設定行高
                                      // 合併儲存格範圍
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, 11)); // 合併A~L欄

                var firstCell = row.CreateCell(0);
                firstCell.SetCellValue(titles[i]);
                firstCell.CellStyle = centeredBorderStyle16;

                // 為合併範圍內的儲存格設置框線樣式
                for (int col = 1; col <= 11; col++)
                {
                    var mergedCell = row.CreateCell(col);
                    mergedCell.CellStyle = centeredBorderStyle16;
                }
            }


            // 在第三列的M欄位放置標題
            var thirdRow = sheet.CreateRow(2);
            thirdRow.Height = 40 * 30; // 設定行高
            var titleCell = thirdRow.CreateCell(12); // M欄位對應的索引是12
            titleCell.SetCellValue("單位 : 元");
            titleCell.CellStyle = centeredBorderStyle16;

            // 在第四列A~N欄位放置標題
            var fourthRow = sheet.CreateRow(3);
            fourthRow.Height = 40 * 30; // 設定行高
            var columnTitles = new[]
            {
        "組室別", "6級(科目)", "7級(子目)", "8級(細目)", "年度預算額度(1)", "併決算數(2)", "一般動支數(3)", "勻出數(4)", "可用預算餘額 \r\n(5)=(1)+(2)-(3)-(4)", "勻入數(6)", "勻入實付數(7)", "勻入數餘額 \r\n(8)=(6)-(7)", "可用預算餘額 \r\n(含勻入) \r\n(10) = (5) + (8)", "本科目實付數(9)"
            };

            for (int i = 0; i < columnTitles.Length; i++)
            {
                var cell = fourthRow.CreateCell(i);
                cell.SetCellValue(columnTitles[i]);
                cell.CellStyle = centeredBorderStyle12;
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

            // 收集所有資料進行排序
            var allData = new List<ExportBudgetAmountDTO>();

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
                    var budgetAmountData = GetBudgetAmountData(budgetId, request.Year, request.StartMonth, request.EndMonth);
                    allData.AddRange(budgetAmountData);

                    // 累計當前 Subject6 的所有欄位
                    totalAnnualBudgetAmount += budgetAmountData.Sum(data => data.AnnualBudgetAmount);
                    totalFinalBudgetAmount += budgetAmountData.Sum(data => data.FinalBudgetAmount);
                    totalGeneral += budgetAmountData.Sum(data => data.General);
                    totalOut += budgetAmountData.Sum(data => data.Out);
                    totalUseBudget += budgetAmountData.Sum(data => data.UseBudget);
                    totalIn += budgetAmountData.Sum(data => data.In);
                    totalInActual += budgetAmountData.Sum(data => data.InActual);
                    totalInBalance += budgetAmountData.Sum(data => data.InBalance);
                    totalInUseBudget += budgetAmountData.Sum(data => data.InUseBudget);
                    totalSubjectActual += budgetAmountData.Sum(data => data.SubjectActual);
                }

                //// 添加每個 Subject6 的合計行到 allData
                //allData.Add(new ExportBudgetAmountDTO
                //{
                //    Subject6 = subject6,
                //    Subject7 = "合計",
                //    AnnualBudgetAmount = totalAnnualBudgetAmount,
                //    FinalBudgetAmount = totalFinalBudgetAmount,
                //    General = totalGeneral,
                //    Out = totalOut,
                //    UseBudget = totalUseBudget,
                //    In = totalIn,
                //    InActual = totalInActual,
                //    InBalance = totalInBalance,
                //    InUseBudget = totalInUseBudget,
                //    SubjectActual = totalSubjectActual
                //});

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

            // 排序所有收集到的資料
            var sortedData = allData
                .OrderBy(data =>
                {
                    var subStr = data.Subject7.Substring(0, Math.Min(4, data.Subject7.Length));
                    return int.TryParse(subStr, out int num) ? num : int.MaxValue;
                })
                .ThenBy(data =>
                {
                    if (data.Subject8 != null)
                    {
                        var subStr = data.Subject8.Substring(0, Math.Min(6, data.Subject8.Length));
                        return int.TryParse(subStr, out int num) ? num : int.MaxValue;
                    }
                    return int.MaxValue;
                })
                .ToList();

            // 將排序後的資料填充到 Excel 中
            string currentSubject6 = null;
            foreach (var data in sortedData)
            {
                // 檢查是否需要插入合計行
                if (currentSubject6 != null && currentSubject6 != data.Subject6)
                {
                    // 插入合計行
                    var totalRow = sheet.CreateRow(currentRow++);
                    totalRow.Height = 40 * 30; // 設定行高
                    var totalCells = new ICell[14];
                    for (int i = 0; i < totalCells.Length; i++)
                    {
                        totalCells[i] = totalRow.CreateCell(i);
                        totalCells[i].CellStyle = rightAlignedNumberStyle; // 為所有合計儲存格添加邊框樣式
                    }
                    totalCells[3].SetCellValue("合計"); // D 欄位為標題
                    totalCells[4].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.AnnualBudgetAmount));
                    totalCells[5].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.FinalBudgetAmount));
                    totalCells[6].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.General));
                    totalCells[7].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.Out));
                    totalCells[8].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.UseBudget));
                    totalCells[9].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.In));
                    totalCells[10].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.InActual));
                    totalCells[11].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.InBalance));
                    totalCells[12].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.InUseBudget));
                    totalCells[13].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.SubjectActual));
                }

                // 更新當前 Subject6
                currentSubject6 = data.Subject6;

                // 填充資料
                var row = sheet.CreateRow(currentRow++);
                row.Height = 40 * 30; // 設定行高

                // 找出對應的 GroupName
                var group = groups.FirstOrDefault(g => g.GroupId == request.GroupId);
                var groupName = group != null ? group.GroupName : "未知";

                // 建立儲存格的陣列
                var cells = new ICell[14];
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = row.CreateCell(i);
                }

                // 定義要插入的值
                var values = new object[]
                {
            groupName,
            data.Subject6,
            data.Subject7,
            data.Subject8 ?? "", // 確保不為 null
            data.AnnualBudgetAmount,
            data.FinalBudgetAmount,
            data.General,
            data.Out,
            data.UseBudget,
            data.In,
            data.InActual,
            data.InBalance,
            data.InUseBudget,
            data.SubjectActual
                };

                // 將值設置到儲存格中，並根據條件設置樣式
                for (int i = 0; i < cells.Length; i++)
                {
                    if (values[i] is int)
                    {
                        cells[i].SetCellValue(Convert.ToDouble(values[i]));
                        if (i >= 4) // E~N欄位
                        {
                            cells[i].CellStyle = rightAlignedNumberStyle;
                        }
                    }
                    else
                    {
                        cells[i].SetCellValue(values[i].ToString());
                        if (i < 4) // A~D欄位
                        {
                            if (i == 2 || i == 3) // 針對 Subject7 和 Subject8 欄位
                            {
                                cells[i].CellStyle = wrappedTextStyle;
                            }
                            else
                            {
                                cells[i].CellStyle = centeredBorderStyle16;
                            }
                        }
                    }
                }
            }

            // 添加最後一組的合計行
            if (currentSubject6 != null)
            {
                var totalRow = sheet.CreateRow(currentRow++);
                totalRow.Height = 40 * 30; // 設定行高
                var totalCells = new ICell[14];
                for (int i = 0; i < totalCells.Length; i++)
                {
                    totalCells[i] = totalRow.CreateCell(i);
                    totalCells[i].CellStyle = rightAlignedNumberStyle; // 為所有合計儲存格添加邊框樣式
                }
                totalCells[3].SetCellValue("合計"); // D 欄位為標題
                totalCells[4].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.AnnualBudgetAmount));
                totalCells[5].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.FinalBudgetAmount));
                totalCells[6].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.General));
                totalCells[7].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.Out));
                totalCells[8].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.UseBudget));
                totalCells[9].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.In));
                totalCells[10].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.InActual));
                totalCells[11].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.InBalance));
                totalCells[12].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.InUseBudget));
                totalCells[13].SetCellValue(allData.Where(d => d.Subject6 == currentSubject6).Sum(d => d.SubjectActual));
            }



            // 添加最下面的總計行
            var grandTotalRow = sheet.CreateRow(currentRow++);
            grandTotalRow.Height = 40 * 30; // 設定行高
            var grandTotalCells = new ICell[14];
            for (int i = 0; i < grandTotalCells.Length; i++)
            {
                grandTotalCells[i] = grandTotalRow.CreateCell(i);
                grandTotalCells[i].CellStyle = rightAlignedNumberStyle; // 為所有總計儲存格添加邊框樣式
            }
            grandTotalCells[3].SetCellValue("總計"); // D 欄位為標題
            grandTotalCells[3].CellStyle = rightAlignedCenteredStyle; // 總計標題的樣式
            grandTotalCells[4].SetCellValue(grandTotalAnnualBudgetAmount);
            grandTotalCells[5].SetCellValue(grandTotalFinalBudgetAmount);
            grandTotalCells[6].SetCellValue(grandTotalGeneral);
            grandTotalCells[7].SetCellValue(grandTotalOut);
            grandTotalCells[8].SetCellValue(grandTotalUseBudget);
            grandTotalCells[9].SetCellValue(grandTotalIn);
            grandTotalCells[10].SetCellValue(grandTotalInActual);
            grandTotalCells[11].SetCellValue(grandTotalInBalance);
            grandTotalCells[12].SetCellValue(grandTotalInUseBudget);
            grandTotalCells[13].SetCellValue(grandTotalSubjectActual);


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

        private List<ExportBudgetAmountDTO> GetBudgetAmountData(int budgetId, int year, int startMonth, int endMonth)
        {
            Expression<Func<BudgetAmount, bool>> condition = item => true;
            condition = condition.And(BudgetAmount => BudgetAmount.Status != null && (BudgetAmount.Status.Trim() == "O" || BudgetAmount.Status.Trim() == "C"));
            condition = condition.And(BudgetAmount => BudgetAmount.IsValid == true && BudgetAmount.CreatedYear == year);

            // 新增 RequestDate 的月份範圍條件，並檢查是否為 null
            condition = condition.And(BudgetAmount => BudgetAmount.RequestDate.HasValue
                                                    && BudgetAmount.RequestDate.Value.Month >= startMonth
                                                    && BudgetAmount.RequestDate.Value.Month <= endMonth);

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
