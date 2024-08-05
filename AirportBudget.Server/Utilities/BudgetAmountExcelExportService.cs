using AirportBudget.Server.Models;
using AirportBudget.Server.ViewModels;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace AirportBudget.Server.Utilities
{
    public  class BudgetAmountExcelExportService
    {
        public byte[] ExportBudgetAmountToExcel(BudgetAmountExcelViewModel request, List<BudgetAmount> budgetAmounts)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            // 設定標題
            var titles = new[]
            {
            "交通部民用航空局", "臺北國際航空站", $"{request.Year}年度預算控制表"
        };

            for (int i = 0; i < titles.Length; i++)
            {
                var row = sheet.CreateRow(i);
                var cell = row.CreateCell(0);
                cell.SetCellValue(titles[i]);
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, 11)); // 合併A~L欄
            }

            // 設定第4列的合併儲存格
            var headerRow = sheet.CreateRow(3);
            headerRow.CreateCell(0).SetCellValue("組室別：");
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(3, 3, 0, 1)); // 合併A、B欄
            headerRow.CreateCell(2).SetCellValue($"{request.GroupName}");

            // 創建一個啟用換行的單元格樣式
            ICellStyle wrapTextStyle = workbook.CreateCellStyle();
            wrapTextStyle.WrapText = true;

            // 設定5~15列的A、B欄合併
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(4, 14, 0, 1)); // 合併A、B欄
            string[] rowTitles = ["6級(科目)", "7級(子目)", "8級(細目)", "年度預算額度(1)", "併決算數額(2)", "一般動支數額(3)", "勻出數額(4)", "(不含勻入)\" + \" \\n\" + \"一般預算餘額(5)", "(含勻入)\" + \" \\n\" + \"可用預算餘額(10)", "Title 9", "Title 10"];
            for (int i = 4; i < 15; i++)
            {
                var row = sheet.CreateRow(i);
                if (i == 4)
                {
                    //row.CreateCell(0).SetCellValue("預" + " \n" + "算" + " \n" + "科" + " \n" + "目" + " \n" + "/" + " \n" + "金" + " \n" + "額");
                    var cell = row.CreateCell(0);
                    cell.SetCellValue("預\n算\n科\n目\n/\n金\n額");
                    cell.CellStyle = wrapTextStyle; // 設置單元格樣式
                }
                row.CreateCell(2).SetCellValue(rowTitles[i - 4]);
            }

            // 特別處理5~8列的G欄合併
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(4, 7, 6, 6));
            for (int i = 4; i < 8; i++)
            {
                var row = sheet.GetRow(i);
                //row.CreateCell(6).SetCellValue("用" + " \n" + "途" + " \n" + "說" + " \n" + "明");
                var cell = row.CreateCell(6);
                cell.SetCellValue("用\n途\n說\n明");
                cell.CellStyle = wrapTextStyle; // 設置單元格樣式
            }

            // 設置9~11列的H欄，不合併，放三個標題
            string[] titlesH = ["勻入數額(6)", "勻入實付數額(7)", "勻入數餘額(8)"];
            for (int i = 8; i < 11; i++)
            {
                var row = sheet.GetRow(i) ?? sheet.CreateRow(i);
                row.CreateCell(7).SetCellValue(titlesH[i - 8]);
            }

            // 特別處理12~13列和14~15列的C欄合併
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(11, 12, 2, 2));
            var row12 = sheet.GetRow(11) ?? sheet.CreateRow(11);
            //var row13 = sheet.CreateRow(12);
            row12.CreateCell(2).SetCellValue("(不含勻入)" + " \n" + "一般預算餘額(5)");
            //row13.CreateCell(2).SetCellValue("Merged Title in C 1");

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(13, 14, 2, 2));
            var row14 = sheet.GetRow(13) ?? sheet.CreateRow(13);
            //var row15 = sheet.CreateRow(14);
            row14.CreateCell(2).SetCellValue("(含勻入)" + " \n" + "可用預算餘額(10)");
            //row15.CreateCell(2).SetCellValue("Merged Title in C 2");

            // 特別處理14~15列的H欄合併
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(13, 14, 7, 7));
            row14.CreateCell(7).SetCellValue("本科目實付小計(9)");
            //row15.CreateCell(7).SetCellValue("Merged Title in H");

            // 設定空白的欄位合併
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(11, 12, 7, 7));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(11, 12, 8, 13));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(4, 7, 7, 13));

            // 設定第16列的合併儲存格
            var row16 = sheet.CreateRow(15);
            row16.CreateCell(2).SetCellValue("摘要");
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(15, 15, 2, 4)); // 合併C~E欄
            row16.CreateCell(5).SetCellValue("請購金額");
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(15, 15, 5, 6)); // 合併F~G欄

            // 設定第16列其他欄位的不同標題
            string[] titles16 = ["請購日期", "類別", "Title in C~E", "Title D", "Title E", "Title in F~G", "Title G", "支付日期", "實付金額", "請購人", "支付人", "備註", "未稅", "已對帳"];
            for (int i = 0; i < 14; i++)
            {
                if (i != 2 && i != 5)
                {
                    row16.CreateCell(i).SetCellValue(titles16[i]);
                }
            }


            // Data

            // 特別處理5~8列的D~F欄合併
            //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(4, 7, 3, 5)); // 合併D~F欄
            for (int i = 4; i < 8; i++)
            {
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 3, 5)); // 合併D~F欄
                var row = sheet.GetRow(i);
                if (i == 4)
                {
                    row.CreateCell(3).SetCellValue($"{request.Subject6}");
                }
                else if (i == 5)
                {
                    row.CreateCell(3).SetCellValue($"{request.Subject7}");
                }
                else if (i == 6)
                {
                    row.CreateCell(3).SetCellValue($"{request.Subject8}");
                }
                else if (i == 7)
                {
                    row.CreateCell(3).SetCellValue($"{request.AnnualBudgetAmount}");
                }
            };

            // 特別處理9~11列的D~F欄合併
            for (int i = 8; i < 11; i++)
            {
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 3, 6)); // 合併D~G欄
                var row = sheet.GetRow(i);
                if (i == 8)
                {
                    row.CreateCell(3).SetCellValue($"{request.FinalBudgetAmount}");
                }
                else if (i == 9)
                {
                    row.CreateCell(3).SetCellValue($"{request.General}");
                }
                else if (i == 10)
                {
                    row.CreateCell(3).SetCellValue($"{request.Out}");
                }
            };

            // 特別處理12~13列和14~15列的D欄合併
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(11, 12, 3, 6));
            //var row12 = sheet.GetRow(11) ?? sheet.CreateRow(11);
            row12.CreateCell(3).SetCellValue($"{request.UseBudget}");
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(13, 14, 3, 6));
            //var row14 = sheet.GetRow(13) ?? sheet.CreateRow(13);
            row14.CreateCell(3).SetCellValue($"{request.End}");

            // 設置9~11列的I~N欄合併
            for (int i = 8; i < 11; i++)
            {
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 8, 13)); // 合併I~N欄
                var row = sheet.GetRow(i);
                if (i == 8)
                {
                    row.CreateCell(8).SetCellValue($"{request.In}");
                }
                else if (i == 9)
                {
                    row.CreateCell(8).SetCellValue($"{request.InActual}");
                }
                else if (i == 10)
                {
                    row.CreateCell(8).SetCellValue($"{request.InBalance}");
                }
            }

            // 特別處理14~15列的I~N欄合併
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(13, 14, 8, 13)); // 合併I~N欄
            var row15 = sheet.GetRow(13) ?? sheet.CreateRow(13);
            row15.CreateCell(8).SetCellValue($"{request.SubjectActual}");

            //// 自定義日期格式
            //IDataFormat format = workbook.CreateDataFormat();
            //ICellStyle dateStyle = workbook.CreateCellStyle();
            //dateStyle.DataFormat = format.GetFormat("yyy/MM/dd");

            // 根據資料數量從第17列開始設置數據
            for (int i = 0; i < budgetAmounts.Count; i++)
            {
                var row = sheet.CreateRow(16 + i);
                var budgetAmount = budgetAmounts[i];

                // 設定A到N欄位的資料
                ICell requestDateCell = row.CreateCell(0);
                if (budgetAmount.RequestDate.HasValue)
                {
                    string formattedDate = ConvertToTaiwanCalendar(budgetAmount.RequestDate.Value);
                    requestDateCell.SetCellValue(formattedDate);
                }
                else
                {
                    requestDateCell.SetCellValue(string.Empty);
                }

                // 將枚舉轉換為描述文字
                row.CreateCell(1).SetCellValue(GetEnumDescription(budgetAmount.Type));

                // 設定C~E欄位合併
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(16 + i, 16 + i, 2, 4));
                row.CreateCell(2).SetCellValue(budgetAmount.Description);

                // 設定F~G欄位合併
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(16 + i, 16 + i, 5, 6));
                row.CreateCell(5).SetCellValue(budgetAmount.RequestAmount);

                // 設定其他欄位
                ICell paymentDateCell = row.CreateCell(7);
                if (budgetAmount.PaymentDate.HasValue)
                {
                    string formattedDate = ConvertToTaiwanCalendar(budgetAmount.PaymentDate.Value);
                    paymentDateCell.SetCellValue(formattedDate);
                }
                else
                {
                    paymentDateCell.SetCellValue(string.Empty);
                }
                row.CreateCell(8).SetCellValue(budgetAmount.PaymentAmount);
                row.CreateCell(9).SetCellValue(budgetAmount.RequestPerson);
                row.CreateCell(10).SetCellValue(budgetAmount.PaymentPerson);
                row.CreateCell(11).SetCellValue(budgetAmount.Remarks);
                // 設置布林值欄位
                row.CreateCell(12).SetCellValue(budgetAmount.ExTax ? "✓" : string.Empty);
                row.CreateCell(13).SetCellValue(budgetAmount.Reconciled ? "✓" : string.Empty);
            }

            //欄位大小
            sheet.SetColumnWidth(0, 60 * 60);
            sheet.SetColumnWidth(1, 60 * 60);
            sheet.SetColumnWidth(2, 100 * 100);
            sheet.SetColumnWidth(7, 100 * 100);
            sheet.SetColumnWidth(8, 60 * 60);
            sheet.SetColumnWidth(11, 80 * 80);

            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }

        }


        private string ConvertToTaiwanCalendar(DateTime date)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            int year = taiwanCalendar.GetYear(date);
            string month = date.Month.ToString().PadLeft(2, '0');
            string day = date.Day.ToString().PadLeft(2, '0');
            return $"{year}/{month}/{day}";
        }

        private string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            //var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

    }
}
