using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AirportBudget.Server.Utilities
{
    public static class ExcelStyleHelper
    {
        public static ICellStyle CreateCellStyle(IWorkbook workbook)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.WrapText = true;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 12;
            font.IsBold = true;
            font.FontName = "微軟正黑體";
            cellStyle.SetFont(font);

            return cellStyle;
        }

        public static ICellStyle CreateHeaderStyle(IWorkbook workbook)
        {
            ICellStyle headerStyle = CreateCellStyle(workbook);
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 16;
            font.IsBold = true;
            font.FontName = "微軟正黑體";
            headerStyle.SetFont(font);

            headerStyle.BorderTop = BorderStyle.None;
            headerStyle.BorderBottom = BorderStyle.None;
            headerStyle.BorderLeft = BorderStyle.None;
            headerStyle.BorderRight = BorderStyle.None;

            return headerStyle;
        }

        public static ICellStyle CreateYellowCellStyle(IWorkbook workbook)
        {
            ICellStyle yellowCellStyle = CreateCellStyle(workbook);
            yellowCellStyle.FillForegroundColor = IndexedColors.Yellow.Index;
            yellowCellStyle.FillPattern = FillPattern.SolidForeground;
            return yellowCellStyle;
        }
    }
}
