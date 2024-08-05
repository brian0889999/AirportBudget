using AirportBudget.Server.DTOs;
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
    public  class ExportFundExcelService
    {
        public byte[] ExportFundToExcel(ExportFundRequestViewModel request)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            sheet.SetColumnWidth(0, 20 * 200);
            for (int i = 2; i <= 10; i++)
            {
                sheet.SetColumnWidth(i, 20 * 200);


            }
            // 設定標題
            var titles = new[]
            {
            $"工務組{request.Year}年{request.StartMonth}-{request.EndMonth}月民航事業作業基金執行情形表"
            };

            for (int i = 0; i < titles.Length; i++)
            {
                var row = sheet.CreateRow(i);
                var cell = row.CreateCell(0);
                cell.SetCellValue(titles[i]);
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, 10)); // 合併A~K欄
            }

            // 在第二列的K欄位放置標題
            var secondRow = sheet.CreateRow(1);
            var titleCell = secondRow.CreateCell(10); // K欄位對應的索引是10
            titleCell.SetCellValue("單位 : 元");

            // 第三列的合併和標題設置
            var thirdRow = sheet.CreateRow(2);

            // 合併A和B欄位
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2, 2, 0, 1));
            var cellAB = thirdRow.CreateCell(0);
            cellAB.SetCellValue("加總 - 傳票金額");

            // 合併C和D欄位
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2, 2, 2, 3));
            var cellCD = thirdRow.CreateCell(2);
            cellCD.SetCellValue("科目代碼");

            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }

        }

    }
}
