using AirportBudget.Server.DTOs;
using AirportBudget.Server.Enums;
using AirportBudget.Server.Interfaces.Repositorys;
using AirportBudget.Server.Models;
using AirportBudget.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using LinqKit;

namespace AirportBudget.Server.Biz
{
    public class ExportFundExcelService(
        IGenericRepository<BudgetAmount> budgetAmountRepository,
        IGenericRepository<User> userRepository)
    {
        private readonly IGenericRepository<BudgetAmount> _budgetAmountRepository = budgetAmountRepository;
        private readonly IGenericRepository<User> _userRepository = userRepository;

        public byte[] ExportFundToExcel(ExportFundRequestViewModel request)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            // 設置樣式
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 12; // 字體大小 12
            font.FontName = "Microsoft JhengHei"; // 設定字體為微軟正黑體
            headerStyle.SetFont(font);
            headerStyle.Alignment = HorizontalAlignment.Center; // 置中
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;

            ICellStyle rightAlignedStyle = workbook.CreateCellStyle();
            rightAlignedStyle.SetFont(font);
            rightAlignedStyle.Alignment = HorizontalAlignment.Right; // 靠右
            rightAlignedStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直置中
            rightAlignedStyle.BorderTop = BorderStyle.Thin;
            rightAlignedStyle.BorderBottom = BorderStyle.Thin;
            rightAlignedStyle.BorderLeft = BorderStyle.Thin;
            rightAlignedStyle.BorderRight = BorderStyle.Thin;

            ICellStyle leftAlignedStyle = workbook.CreateCellStyle();
            leftAlignedStyle.SetFont(font);
            leftAlignedStyle.Alignment = HorizontalAlignment.Left; // 靠左
            leftAlignedStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直置中
            leftAlignedStyle.BorderTop = BorderStyle.Thin;
            leftAlignedStyle.BorderBottom = BorderStyle.Thin;
            leftAlignedStyle.BorderLeft = BorderStyle.Thin;
            leftAlignedStyle.BorderRight = BorderStyle.Thin;

            ICellStyle centerAlignedStyle = workbook.CreateCellStyle();
            centerAlignedStyle.SetFont(font);
            centerAlignedStyle.Alignment = HorizontalAlignment.Center; // 置中
            centerAlignedStyle.VerticalAlignment = VerticalAlignment.Center;
            centerAlignedStyle.BorderTop = BorderStyle.Thin;
            centerAlignedStyle.BorderBottom = BorderStyle.Thin;
            centerAlignedStyle.BorderLeft = BorderStyle.Thin;
            centerAlignedStyle.BorderRight = BorderStyle.Thin;

            ICellStyle numberStyle = workbook.CreateCellStyle();
            numberStyle.SetFont(font);
            numberStyle.Alignment = HorizontalAlignment.Right; // 數字靠右
            numberStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直置中
            numberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            numberStyle.BorderTop = BorderStyle.Thin;
            numberStyle.BorderBottom = BorderStyle.Thin;
            numberStyle.BorderLeft = BorderStyle.Thin;
            numberStyle.BorderRight = BorderStyle.Thin;


            sheet.DefaultRowHeight = 20 * 20;
            // 設置列寬
            sheet.SetColumnWidth(0, 20 * 256);
            for (int i = 2; i <= 10; i++)
            {
                sheet.SetColumnWidth(i, 20 * 256);
            }

            // 設定標題
            var titles = new[]
            {
            $"工務組{request.Year}年{request.StartMonth}-{request.EndMonth}月民航事業作業基金執行情形表"
            };

            for (int i = 0; i < titles.Length; i++)
            {
                var row = sheet.CreateRow(i);
                row.HeightInPoints = 25; // 設置行高
                var cell = row.CreateCell(0);
                cell.SetCellValue(titles[i]);
                cell.CellStyle = headerStyle;
                sheet.AddMergedRegion(new CellRangeAddress(i, i, 0, 10)); // 合併A~K欄
                ApplyBordersToMergedRegion(sheet, i, i, 0, 10, headerStyle);
            }

            // 在第二列的K欄位放置標題
            var secondRow = sheet.CreateRow(1);
            secondRow.HeightInPoints = 20; // 設置行高
            var titleCell = secondRow.CreateCell(10); // K欄位對應的索引是10
            titleCell.SetCellValue("單位 : 元");
            titleCell.CellStyle = headerStyle;

            // 第三列的合併和標題設置
            var thirdRow = sheet.CreateRow(2);
            thirdRow.HeightInPoints = 20; // 設置行高

            // 合併A和B欄位
            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 1));
            var cellAB = thirdRow.CreateCell(0);
            cellAB.SetCellValue("加總 - 傳票金額");
            cellAB.CellStyle = centerAlignedStyle;

            ApplyBordersToMergedRegion(sheet, 2, 2, 0, 1, centerAlignedStyle);

            // 合併C和D欄位
            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 2, 3));
            var cellCD = thirdRow.CreateCell(2);
            cellCD.SetCellValue("科目代碼");
            cellCD.CellStyle = centerAlignedStyle;

            ApplyBordersToMergedRegion(sheet, 2, 2, 2, 3, centerAlignedStyle);

            // 第四列設置標題
            var fourthRow = sheet.CreateRow(3);
            fourthRow.HeightInPoints = 20; // 設置行高
            var headers = new[] { "隸屬系統", "承辦人", "21", "23", "25", "27", "28", "31", "32", "68", "總計" };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = fourthRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = i >= 2 ? rightAlignedStyle : centerAlignedStyle; // C~K欄靠右
            }

            // 準備存放承辦人金額的字典
            var personTotalAmounts = new Dictionary<string, Dictionary<string, int>>();

            // 遍歷 UserSystemType 的所有值
            foreach (UserSystemType system in Enum.GetValues(typeof(UserSystemType)))
            {
                // 取得該系統的所有 People 名單
                var people = ExportRequestPerson(request.Year, request.StartMonth, request.EndMonth, system);

                foreach (var person in people)
                {
                    // 初始化每個承辦人的金額字典
                    if (!personTotalAmounts.ContainsKey(person))
                    {
                        personTotalAmounts[person] = new Dictionary<string, int>();
                    }

                    // 對每個 sectionCode 進行 ExportFund 查詢
                    foreach (var sectionCode in new[] { "21", "23", "25", "27", "28", "31", "32", "68" })
                    {
                        var fundResults = ExportFund(request.Year, request.StartMonth, request.EndMonth, person, sectionCode);

                        // 累加相同承辦人和科目代碼的金額
                        foreach (var fund in fundResults)
                        {
                            if (!personTotalAmounts[person].ContainsKey(sectionCode))
                            {
                                personTotalAmounts[person][sectionCode] = 0;
                            }
                            personTotalAmounts[person][sectionCode] += fund.Money;
                        }
                    }
                }
            }

            // 將匯總的資料寫入 Excel
            int currentRow = 4; // 從第五列開始寫入資料
            var grandTotalAmounts = new int[headers.Length - 2]; // 用來儲存總計的金額

            // 設置第 4 到第 28 列的格式
            for (int rowIndex = currentRow; rowIndex < currentRow + 25; rowIndex++)
            {
                var row = sheet.CreateRow(rowIndex);

                var systemDescriptionCell = row.CreateCell(0);
                systemDescriptionCell.CellStyle = leftAlignedStyle; // 文字靠左且垂直置中

                var personCell = row.CreateCell(1);
                personCell.CellStyle = centerAlignedStyle; // 置中

                for (int i = 2; i < headers.Length; i++)
                {
                    var cell = row.CreateCell(i);
                    cell.CellStyle = rightAlignedStyle; // 文字靠右且垂直置中
                }
            }

            foreach (var system in Enum.GetValues(typeof(UserSystemType)))
            {
                var systemPeople = ExportRequestPerson(request.Year, request.StartMonth, request.EndMonth, (UserSystemType)system);

                // 儲存系統的起始列以便稍後進行合併
                int startRowForSystem = currentRow;

                foreach (var person in systemPeople.Distinct())
                {
                    var row = sheet.CreateRow(currentRow++);
                    row.HeightInPoints = 20; // 設置行高

                    // 使用 GetEnumDescription 方法取得系統描述
                    string systemDescription = GetEnumDescription((UserSystemType)system) + "系統";

                    var systemCell = row.CreateCell(0);
                    systemCell.SetCellValue(systemDescription);
                    systemCell.CellStyle = leftAlignedStyle; // 系統名稱靠左

                    var personCell = row.CreateCell(1);
                    personCell.SetCellValue(person);
                    personCell.CellStyle = centerAlignedStyle; // 承辦人置中

                    int totalAmount = 0;
                    for (int i = 0; i < headers.Length - 3; i++) // 不包括 "隸屬系統"、"承辦人"、"總計"
                    {
                        var sectionCode = headers[i + 2];
                        var amount = personTotalAmounts.ContainsKey(person) && personTotalAmounts[person].ContainsKey(sectionCode)
                            ? personTotalAmounts[person][sectionCode]
                            : 0;

                        var amountCell = row.CreateCell(i + 2);
                        if (amount == 0)
                        {
                            amountCell.SetCellValue(""); // 如果數字為 0，設置為空白
                        }
                        else
                        {
                            amountCell.SetCellValue(amount);
                        }
                        amountCell.CellStyle = numberStyle; // 設定格式化為數字靠右

                        totalAmount += amount;

                        // 將每個科目代碼的金額加入到總計
                        grandTotalAmounts[i] += amount;
                    }

                    // 計算總計
                    var totalCell = row.CreateCell(headers.Length - 1);
                    if (totalAmount == 0)
                    {
                        totalCell.SetCellValue(""); // 如果總計為 0，設置為空白
                    }
                    else
                    {
                        totalCell.SetCellValue(totalAmount);
                    }
                    totalCell.CellStyle = numberStyle;

                    grandTotalAmounts[headers.Length - 3] += totalAmount; // 加入每個系統的總計
                }

                // 合併系統的 "隸屬系統" 欄位
                if (currentRow - startRowForSystem > 1)
                {
                    sheet.AddMergedRegion(new CellRangeAddress(startRowForSystem, currentRow - 1, 0, 0));
                    ApplyBordersToMergedRegion(sheet, startRowForSystem, currentRow - 1, 0, 0, leftAlignedStyle);
                }

                // 增加系統合計列
                var totalRow = sheet.CreateRow(currentRow++);
                totalRow.HeightInPoints = 20; // 設置行高
                // 合併系統合計的 A 和 B 欄位
                sheet.AddMergedRegion(new CellRangeAddress(currentRow - 1, currentRow - 1, 0, 1));
                var totalTitleCell = totalRow.CreateCell(0);
                totalTitleCell.SetCellValue($"{GetEnumDescription((UserSystemType)system)}系統合計");
                totalTitleCell.CellStyle = leftAlignedStyle; // 系統合計靠左置中

                int systemTotalAmount = 0;

                for (int i = 2; i < headers.Length - 1; i++)
                {
                    int sectionTotal = 0;
                    for (int rowIdx = startRowForSystem; rowIdx < currentRow - 1; rowIdx++)
                    {
                        var cell = sheet.GetRow(rowIdx).GetCell(i);
                        if (cell.CellType == CellType.Numeric)
                        {
                            sectionTotal += (int)cell.NumericCellValue; // 取得欄位的數字
                        }
                    }

                    var sectionTotalCell = totalRow.CreateCell(i);
                    if (sectionTotal == 0)
                    {
                        sectionTotalCell.SetCellValue(""); // 如果數字為 0，設置為空白
                    }
                    else
                    {
                        sectionTotalCell.SetCellValue(sectionTotal);
                    }
                    sectionTotalCell.CellStyle = numberStyle; // 設定為數字格式
                    systemTotalAmount += sectionTotal;
                }

                var systemTotalCell = totalRow.CreateCell(headers.Length - 1);
                if (systemTotalAmount == 0)
                {
                    systemTotalCell.SetCellValue(""); // 如果總計為 0，設置為空白
                }
                else
                {
                    systemTotalCell.SetCellValue(systemTotalAmount);
                }
                systemTotalCell.CellStyle = numberStyle;
            }

            // 增加最下面的總計列
            var grandTotalRow = sheet.CreateRow(currentRow++);
            grandTotalRow.HeightInPoints = 20; // 設置行高
            sheet.AddMergedRegion(new CellRangeAddress(currentRow - 1, currentRow - 1, 0, 1));
            var grandTotalTitleCell = grandTotalRow.CreateCell(0);
            grandTotalTitleCell.SetCellValue("總計");
            grandTotalTitleCell.CellStyle = rightAlignedStyle; // 底部總計靠右

            for (int i = 2; i < headers.Length; i++)
            {
                var grandTotalCell = grandTotalRow.CreateCell(i);
                if (grandTotalAmounts[i - 2] == 0)
                {
                    grandTotalCell.SetCellValue(""); // 如果總計為 0，設置為空白
                }
                else
                {
                    grandTotalCell.SetCellValue(grandTotalAmounts[i - 2]);
                }
                grandTotalCell.CellStyle = numberStyle;
            }

            // 設定所有框線
            ApplyBordersToMergedRegion(sheet, currentRow - 1, currentRow - 1, 0, 1, rightAlignedStyle);

            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }

        }


        public List<string> ExportRequestPerson(int year, int startMonth, int endMonth, UserSystemType system)
        {
            // 將民國年轉換為西元年
            int westernYear = year + 1911;

            Expression<Func<BudgetAmount, bool>> condition = item => true;
            condition = condition.And(item => item.Reconciled == true);
            condition = condition.And(item => item.PaymentDate >= new DateTime(westernYear, startMonth, 1) && item.PaymentDate <= new DateTime(westernYear, endMonth, DateTime.DaysInMonth(westernYear, endMonth)));
            condition = condition.And(item => item.AmountYear == year);

            // 取得符合條件的 BudgetAmount 資料
            var budgetAmounts = _budgetAmountRepository.GetByCondition(condition)
                .ToList();

            // 取得所有符合 System 的 User 資料
            var users = _userRepository.GetByCondition(user => user.System == system)
                .ToList();

            // 使用 LINQ Join 進行連接
            var results = budgetAmounts
                .Join(users,
                      budget => budget.RequestPerson,
                      user => user.Name,
                      (budget, user) => new
                      {
                          user.Name
                          // 可以選擇其他需要的欄位
                      })
                .Select(u => u.Name)
                .Distinct() // 確保結果不重複
                .ToList();

            return results;
        }

        private List<ExportFundDTO> ExportFund(int year, int startMonth, int endMonth, string requestPerson, string sectionCode)
        {
            // 將民國年轉換為西元年
            int westernYear = year + 1911;

            Expression<Func<BudgetAmount, bool>> condition = item => true;
            //condition = condition.And(item => item.Status != null && item.Status.Trim() == "O");
            condition = condition.And(item => item.IsValid == true);
            condition = condition.And(item => item.Reconciled == true);
            condition = condition.And(item => item.PaymentDate >= new DateTime(westernYear, startMonth, 1) && item.PaymentDate <= new DateTime(westernYear, endMonth, DateTime.DaysInMonth(westernYear, endMonth)));
            condition = condition.And(item => item.RequestPerson == requestPerson);
            condition = condition.And(item => item.AmountYear == year);
            condition = condition.And(item => item.Budget!.BudgetName.Substring(0, 2) == sectionCode);
            condition = condition.And(item => item.Budget!.GroupId == 1);


            var results = _budgetAmountRepository.GetByCondition(condition)
            .GroupBy(item => new
            {
                Name = item.Budget!.BudgetName.Substring(0, 2),
                item.RequestPerson
            })
            .Select(g => new ExportFundDTO
            {
                Name = g.Key.Name,
                RequestPerson = g.Key.RequestPerson,
                Money = g.Sum(x => x.Type == AmountType.Ordinary || x.Type == AmountType.BalanceIn ? x.PaymentAmount : 0)
            })
            .ToList();

            return results;
        }

        private string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            //var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }


        // 設置合併儲存格範圍內的框線
        private void ApplyBordersToMergedRegion(ISheet sheet, int firstRow, int lastRow, int firstCol, int lastCol, ICellStyle style)
        {
            for (int i = firstRow; i <= lastRow; i++)
            {
                IRow row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                for (int j = firstCol; j <= lastCol; j++)
                {
                    ICell cell = row.GetCell(j) ?? row.CreateCell(j);
                    cell.CellStyle = style;
                }
            }
        }

    }

}
