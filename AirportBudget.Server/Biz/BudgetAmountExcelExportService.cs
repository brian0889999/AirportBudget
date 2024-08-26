using AirportBudget.Server.Models;
using AirportBudget.Server.ViewModels;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace AirportBudget.Server.Biz
{
    public class BudgetAmountExcelExportService
    {
        public byte[] ExportBudgetAmountToExcel(BudgetAmountExcelViewModel request, List<BudgetAmount> budgetAmounts)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            // 設定預設行高
            sheet.DefaultRowHeight = 30 * 20; // 單位是1/20個點

            // 創建字型
            IFont headerFont = workbook.CreateFont();
            headerFont.FontHeightInPoints = 16;
            headerFont.FontName = "微軟正黑體";
            headerFont.IsBold = true; // 設置為粗體

            IFont boldFont = workbook.CreateFont();
            boldFont.FontHeightInPoints = 12;
            boldFont.FontName = "微軟正黑體";
            boldFont.IsBold = true;


            // 創建樣式
            ICellStyle headerStyle = workbook.CreateCellStyle();
            headerStyle.SetFont(headerFont);
            headerStyle.Alignment = HorizontalAlignment.Center; // 置中
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            SetBorders(headerStyle); // 設置框線

            ICellStyle boldCenteredStyle = workbook.CreateCellStyle();
            boldCenteredStyle.SetFont(boldFont);
            boldCenteredStyle.Alignment = HorizontalAlignment.Center; // 置中
            boldCenteredStyle.VerticalAlignment = VerticalAlignment.Center;
            SetBorders(boldCenteredStyle); // 設置框線

            ICellStyle currencyCenteredStyle = workbook.CreateCellStyle();
            currencyCenteredStyle.SetFont(boldFont);
            currencyCenteredStyle.DataFormat = workbook.CreateDataFormat().GetFormat("\"$\"#,##0");
            currencyCenteredStyle.Alignment = HorizontalAlignment.Center; // 置中
            currencyCenteredStyle.VerticalAlignment = VerticalAlignment.Center;
            SetBorders(currencyCenteredStyle); // 設置框線

            ICellStyle numberRightAlignedStyle = workbook.CreateCellStyle();
            numberRightAlignedStyle.SetFont(boldFont);
            numberRightAlignedStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            numberRightAlignedStyle.Alignment = HorizontalAlignment.Right; // 右對齊
            numberRightAlignedStyle.VerticalAlignment = VerticalAlignment.Center;
            SetBorders(numberRightAlignedStyle); // 設置框線

            ICellStyle yellowBackgroundStyle = workbook.CreateCellStyle();
            yellowBackgroundStyle.SetFont(boldFont);
            yellowBackgroundStyle.FillForegroundColor = IndexedColors.Yellow.Index;
            yellowBackgroundStyle.FillPattern = FillPattern.SolidForeground;
            yellowBackgroundStyle.DataFormat = workbook.CreateDataFormat().GetFormat("\"$\"#,##0");
            yellowBackgroundStyle.Alignment = HorizontalAlignment.Center;
            yellowBackgroundStyle.VerticalAlignment = VerticalAlignment.Center;
            SetBorders(yellowBackgroundStyle); // 設置框線

            // 創建一個啟用換行的單元格樣式
            ICellStyle wrapTextStyle = workbook.CreateCellStyle();
            wrapTextStyle.SetFont(boldFont);
            wrapTextStyle.WrapText = true;
            wrapTextStyle.Alignment = HorizontalAlignment.Center; // 置中
            wrapTextStyle.VerticalAlignment = VerticalAlignment.Center; // 置中
            SetBorders(wrapTextStyle); // 設置框線

            // 設定標題
            var titles = new[]
            {
            "交通部民用航空局", "臺北國際航空站", $"{request.Year}年度預算控制表"
            };

            for (int i = 0; i < titles.Length; i++)
            {
                var row = sheet.CreateRow(i);
                row.Height = sheet.DefaultRowHeight; // 設定行高
                var cell = row.CreateCell(0);
                cell.SetCellValue(titles[i]);
                cell.CellStyle = headerStyle; // 使用標題樣式
                sheet.AddMergedRegion(new CellRangeAddress(i, i, 0, 11)); // 合併A~L欄
                ApplyBordersToMergedRegion(sheet, i, i, 0, 11, headerStyle); // 設置合併儲存格的框線
            }

            // 設定第4列的合併儲存格
            var headerRow = sheet.CreateRow(3);
            headerRow.Height = sheet.DefaultRowHeight; // 設定行高
            var cell0 = headerRow.CreateCell(0);
            cell0.SetCellValue("組室別：");
            cell0.CellStyle = boldCenteredStyle; // 使用粗體置中樣式
            sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 1)); // 合併A、B欄
            ApplyBordersToMergedRegion(sheet, 3, 3, 0, 1, boldCenteredStyle); // 設置合併儲存格的框線

            var cell2 = headerRow.CreateCell(2);
            cell2.SetCellValue($"{request.GroupName}");
            cell2.CellStyle = boldCenteredStyle;

            // 設定5~15列的A、B欄合併
            sheet.AddMergedRegion(new CellRangeAddress(4, 14, 0, 1)); // 合併A、B欄
            ApplyBordersToMergedRegion(sheet, 4, 14, 0, 1, boldCenteredStyle); // 設置合併儲存格的框線

            string[] rowTitles = ["6級(科目)", "7級(子目)", "8級(細目)", "年度預算額度(1)", "併決算數額(2)", "一般動支數額(3)", "勻出數額(4)", "(不含勻入)\" + \" \\n\" + \"一般預算餘額(5)", "(含勻入)\" + \" \\n\" + \"可用預算餘額(10)", "Title 9", "Title 10"];
            for (int i = 4; i < 15; i++)
            {
                var row = sheet.CreateRow(i);
                row.Height = sheet.DefaultRowHeight; // 設定行高
                if (i == 4)
                {
                    //row.CreateCell(0).SetCellValue("預" + " \n" + "算" + " \n" + "科" + " \n" + "目" + " \n" + "/" + " \n" + "金" + " \n" + "額");
                    var cell = row.CreateCell(0);
                    cell.SetCellValue("預\n算\n科\n目\n/\n金\n額");
                    cell.CellStyle = wrapTextStyle; // 設置單元格樣式
                }
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(rowTitles[i - 4]);
                cell3.CellStyle = boldCenteredStyle;
            }

            // 特別處理5~8列的G欄合併
            sheet.AddMergedRegion(new CellRangeAddress(4, 7, 6, 6));
            ApplyBordersToMergedRegion(sheet, 4, 7, 6, 6, wrapTextStyle); // 設置合併儲存格的框線
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
                row.Height = sheet.DefaultRowHeight; // 設定行高
                var cell = row.CreateCell(7);
                cell.SetCellValue(titlesH[i - 8]);
                cell.CellStyle = boldCenteredStyle;
            }

            // 特別處理12~13列和14~15列的C欄合併
            sheet.AddMergedRegion(new CellRangeAddress(11, 12, 2, 2));
            ApplyBordersToMergedRegion(sheet, 11, 12, 2, 2, boldCenteredStyle); // 設置合併儲存格的框線
            var row12 = sheet.GetRow(11) ?? sheet.CreateRow(11);
            row12.Height = sheet.DefaultRowHeight; // 設定行高
            //var row13 = sheet.CreateRow(12);
            var cell12 = row12.CreateCell(2);
            cell12.SetCellValue("(不含勻入)" + " \n" + "一般預算餘額(5)");
            cell12.CellStyle = boldCenteredStyle;
            //row13.CreateCell(2).SetCellValue("Merged Title in C 1");

            sheet.AddMergedRegion(new CellRangeAddress(13, 14, 2, 2));
            ApplyBordersToMergedRegion(sheet, 13, 14, 2, 2, boldCenteredStyle); // 設置合併儲存格的框線
            var row14 = sheet.GetRow(13) ?? sheet.CreateRow(13);
            row14.Height = sheet.DefaultRowHeight; // 設定行高
            //var row15 = sheet.CreateRow(14);
            var cell14 = row14.CreateCell(2);
            cell14.SetCellValue("(含勻入)" + " \n" + "可用預算餘額(10)");
            cell14.CellStyle = boldCenteredStyle;
            //row15.CreateCell(2).SetCellValue("Merged Title in C 2");

            // 特別處理14~15列的H欄合併
            sheet.AddMergedRegion(new CellRangeAddress(13, 14, 7, 7));
            ApplyBordersToMergedRegion(sheet, 13, 14, 7, 7, boldCenteredStyle); // 設置合併儲存格的框線
            var cell14H = row14.CreateCell(7);
            cell14H.SetCellValue("本科目實付小計(9)");
            cell14H.CellStyle = boldCenteredStyle;
            //row15.CreateCell(7).SetCellValue("Merged Title in H");

            // 設定空白的欄位合併
            sheet.AddMergedRegion(new CellRangeAddress(11, 12, 7, 7));
            ApplyBordersToMergedRegion(sheet, 11, 12, 7, 7, boldCenteredStyle); // 設置合併儲存格的框線
            sheet.AddMergedRegion(new CellRangeAddress(11, 12, 8, 13));
            ApplyBordersToMergedRegion(sheet, 11, 12, 8, 13, boldCenteredStyle); // 設置合併儲存格的框線
            sheet.AddMergedRegion(new CellRangeAddress(4, 7, 7, 13));
            ApplyBordersToMergedRegion(sheet, 4, 7, 7, 13, wrapTextStyle); // 設置合併儲存格的框線

            // 設定合併儲存格範圍內的儲存格樣式
            for (int rowNum = 4; rowNum <= 7; rowNum++)
            {
                IRow row = sheet.GetRow(rowNum) ?? sheet.CreateRow(rowNum);
                row.Height = sheet.DefaultRowHeight; // 設定行高
                for (int colNum = 7; colNum <= 13; colNum++)
                {
                    ICell cell = row.GetCell(colNum) ?? row.CreateCell(colNum);
                    cell.CellStyle = wrapTextStyle;
                }
            }

            // 設定第16列的合併儲存格
            var row16 = sheet.CreateRow(15);
            row16.Height = sheet.DefaultRowHeight; // 設定行高
            var cell16C = row16.CreateCell(2);
            cell16C.SetCellValue("摘要");
            cell16C.CellStyle = boldCenteredStyle;
            sheet.AddMergedRegion(new CellRangeAddress(15, 15, 2, 4)); // 合併C~E欄
            ApplyBordersToMergedRegion(sheet, 15, 15, 2, 4, boldCenteredStyle); // 設置合併儲存格的框線

            var cell16F = row16.CreateCell(5);
            cell16F.SetCellValue("請購金額");
            cell16F.CellStyle = boldCenteredStyle;
            sheet.AddMergedRegion(new CellRangeAddress(15, 15, 5, 6)); // 合併F~G欄
            ApplyBordersToMergedRegion(sheet, 15, 15, 5, 6, boldCenteredStyle); // 設置合併儲存格的框線

            // 設定第16列其他欄位的不同標題
            string[] titles16 = ["請購日期", "類別", "Title in C~E", "Title D", "Title E", "Title in F~G", "Title G", "支付日期", "實付金額", "請購人", "支付人", "備註", "未稅", "已對帳"];
            for (int i = 0; i < 14; i++)
            {
                if (i != 2 && i != 5)
                {
                    var cell = row16.CreateCell(i);
                    cell.SetCellValue(titles16[i]);
                    cell.CellStyle = boldCenteredStyle;
                }
            }


            // Data

            // 特別處理5~8列的D~F欄合併
            //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(4, 7, 3, 5)); // 合併D~F欄
            for (int i = 4; i < 8; i++)
            {
                sheet.AddMergedRegion(new CellRangeAddress(i, i, 3, 5)); // 合併D~F欄
                ApplyBordersToMergedRegion(sheet, i, i, 3, 5, boldCenteredStyle); // 設置合併儲存格的框線
                var row = sheet.GetRow(i);
                row.Height = sheet.DefaultRowHeight; // 設定行高
                var cell = row.CreateCell(3);
                if (i == 7) // 如果是第8列
                {
                    cell.CellStyle = yellowBackgroundStyle; // 設置顏色
                    cell.SetCellValue(Convert.ToDouble(request.AnnualBudgetAmount));
                }
                else
                {
                    cell.CellStyle = boldCenteredStyle;
                    if (i == 4)
                    {
                        cell.SetCellValue($"{request.Subject6}");
                    }
                    else if (i == 5)
                    {
                        cell.SetCellValue($"{request.Subject7}");
                    }
                    else if (i == 6)
                    {
                        cell.SetCellValue($"{request.Subject8}");
                    }
                }
            };

            // 特別處理9~11列的D~F欄合併
            for (int i = 8; i < 11; i++)
            {
                sheet.AddMergedRegion(new CellRangeAddress(i, i, 3, 6)); // 合併D~G欄
                ApplyBordersToMergedRegion(sheet, i, i, 3, 6, currencyCenteredStyle); // 設置合併儲存格的框線
                var row = sheet.GetRow(i);
                row.Height = sheet.DefaultRowHeight; // 設定行高
                var cell = row.CreateCell(3);
                cell.CellStyle = currencyCenteredStyle; // 數字前加上$
                if (i == 8)
                {
                    cell.SetCellValue(Convert.ToDouble(request.FinalBudgetAmount));
                }
                else if (i == 9)
                {
                    cell.SetCellValue(Convert.ToDouble(request.General));
                }
                else if (i == 10)
                {
                    cell.SetCellValue(Convert.ToDouble(request.Out));
                }
            };

            // 特別處理12~13列和14~15列的D欄合併
            sheet.AddMergedRegion(new CellRangeAddress(11, 12, 3, 6));
            ApplyBordersToMergedRegion(sheet, 11, 12, 3, 6, currencyCenteredStyle); // 設置合併儲存格的框線
            //var row12 = sheet.GetRow(11) ?? sheet.CreateRow(11);
            var row12D = row12.CreateCell(3);
            row12.Height = sheet.DefaultRowHeight; // 設定行高
            row12D.SetCellValue(Convert.ToDouble(request.UseBudget));
            row12D.CellStyle = currencyCenteredStyle; // 設置為貨幣格式

            sheet.AddMergedRegion(new CellRangeAddress(13, 14, 3, 6));
            ApplyBordersToMergedRegion(sheet, 13, 14, 3, 6, currencyCenteredStyle); // 設置合併儲存格的框線
            //var row14 = sheet.GetRow(13) ?? sheet.CreateRow(13);
            var row14D = row14.CreateCell(3);
            row14.Height = sheet.DefaultRowHeight; // 設定行高
            row14D.SetCellValue(Convert.ToDouble(request.End));
            row14D.CellStyle = currencyCenteredStyle; // 設置為貨幣格式

            // 設置9~11列的I~N欄合併
            for (int i = 8; i < 11; i++)
            {
                sheet.AddMergedRegion(new CellRangeAddress(i, i, 8, 13)); // 合併I~N欄
                ApplyBordersToMergedRegion(sheet, i, i, 8, 13, currencyCenteredStyle); // 設置合併儲存格的框線
                var row = sheet.GetRow(i);
                row.Height = sheet.DefaultRowHeight; // 設定行高
                var cell = row.CreateCell(8);
                cell.CellStyle = currencyCenteredStyle; // 數字前加上$
                if (i == 8)
                {
                    cell.SetCellValue(Convert.ToDouble(request.In));
                }
                else if (i == 9)
                {
                    cell.SetCellValue(Convert.ToDouble(request.InActual));
                }
                else if (i == 10)
                {
                    cell.SetCellValue(Convert.ToDouble(request.InBalance));
                }
            }

            // 特別處理14~15列的I~N欄合併
            sheet.AddMergedRegion(new CellRangeAddress(13, 14, 8, 13)); // 合併I~N欄
            ApplyBordersToMergedRegion(sheet, 13, 14, 8, 13, currencyCenteredStyle); // 設置合併儲存格的框線
            var row15 = sheet.GetRow(13) ?? sheet.CreateRow(13);
            row15.Height = sheet.DefaultRowHeight; // 設定行高
            var cell15 = row15.CreateCell(8);
            cell15.SetCellValue(Convert.ToDouble(request.SubjectActual));
            cell15.CellStyle = currencyCenteredStyle; // 數字前加上$


            //// 自定義日期格式
            //IDataFormat format = workbook.CreateDataFormat();
            //ICellStyle dateStyle = workbook.CreateCellStyle();
            //dateStyle.DataFormat = format.GetFormat("yyy/MM/dd");

            // 根據資料數量從第17列開始設置數據
            for (int i = 0; i < budgetAmounts.Count; i++)
            {
                var row = sheet.CreateRow(16 + i);
                row.Height = sheet.DefaultRowHeight; // 設定行高
                var budgetAmount = budgetAmounts[i];

                // 設定A到N欄位的資料
                ICell requestDateCell = row.CreateCell(0);
                requestDateCell.CellStyle = boldCenteredStyle;
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
                var typeCell = row.CreateCell(1);
                typeCell.SetCellValue(GetEnumDescription(budgetAmount.Type));
                typeCell.CellStyle = boldCenteredStyle;

                // 設定C~E欄位合併
                sheet.AddMergedRegion(new CellRangeAddress(16 + i, 16 + i, 2, 4));
                ApplyBordersToMergedRegion(sheet, 16 + i, 16 + i, 2, 4, boldCenteredStyle); // 設置合併儲存格的框線
                var descCell = row.CreateCell(2);
                descCell.SetCellValue(budgetAmount.Description);
                descCell.CellStyle = boldCenteredStyle;


                // 設定F~G欄位合併
                sheet.AddMergedRegion(new CellRangeAddress(16 + i, 16 + i, 5, 6));
                ApplyBordersToMergedRegion(sheet, 16 + i, 16 + i, 5, 6, numberRightAlignedStyle); // 設置合併儲存格的框線
                var requestAmountCell = row.CreateCell(5);
                requestAmountCell.SetCellValue(Convert.ToDouble(budgetAmount.RequestAmount));
                requestAmountCell.CellStyle = numberRightAlignedStyle;

                // 設定其他欄位
                ICell paymentDateCell = row.CreateCell(7);
                paymentDateCell.CellStyle = boldCenteredStyle;
                if (budgetAmount.PaymentDate.HasValue)
                {
                    string formattedDate = ConvertToTaiwanCalendar(budgetAmount.PaymentDate.Value);
                    paymentDateCell.SetCellValue(formattedDate);
                }
                else
                {
                    paymentDateCell.SetCellValue(string.Empty);
                }

                var paymentAmountCell = row.CreateCell(8);
                paymentAmountCell.SetCellValue(Convert.ToDouble(budgetAmount.PaymentAmount));
                paymentAmountCell.CellStyle = numberRightAlignedStyle;

                var requestPersonCell = row.CreateCell(9);
                requestPersonCell.SetCellValue(budgetAmount.RequestPerson);
                requestPersonCell.CellStyle = boldCenteredStyle;

                var paymentPersonCell = row.CreateCell(10);
                paymentPersonCell.SetCellValue(budgetAmount.PaymentPerson);
                paymentPersonCell.CellStyle = boldCenteredStyle;

                var remarksCell = row.CreateCell(11);
                remarksCell.SetCellValue(budgetAmount.Remarks);
                remarksCell.CellStyle = boldCenteredStyle;

                // 設置布林值欄位
                var exTaxCell = row.CreateCell(12);
                exTaxCell.SetCellValue(budgetAmount.ExTax ? "✓" : string.Empty);
                exTaxCell.CellStyle = boldCenteredStyle;

                var reconciledCell = row.CreateCell(13);
                reconciledCell.SetCellValue(budgetAmount.Reconciled ? "✓" : string.Empty);
                reconciledCell.CellStyle = boldCenteredStyle;
            }

            //欄位大小
            sheet.SetColumnWidth(0, 60 * 60);
            sheet.SetColumnWidth(1, 60 * 60);
            sheet.SetColumnWidth(2, 100 * 100);
            sheet.SetColumnWidth(7, 100 * 100);
            sheet.SetColumnWidth(8, 60 * 60);
            sheet.SetColumnWidth(11, 80 * 80);

            //// 計算所有欄位的等比寬度
            //int columnCount = 14; // 根據資料的欄位數量設定
            //int maxColumnWidth = 20 * 256; // 30個字符的最大寬度
            //for (int col = 0; col < columnCount; col++)
            //{
            //    sheet.SetColumnWidth(col, maxColumnWidth);
            //}

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

        // 設置儲存格框線
        private void SetBorders(ICellStyle style)
        {
            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
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
