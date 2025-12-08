using OfficeOpenXml;
using System;

namespace NSPC.Common
{
    public static class ExcelHelper
    {
        public static string GetCellValue(ExcelWorksheet worksheet, int rowIndex, int columnIndex, string ignoreType)
        {
            try
            {
                var cell = worksheet.Cells[rowIndex, columnIndex];
                if (cell != null && cell.Value != null)
                    return cell.Value.ToString().Trim();
                else if (cell.Merge == true && ignoreType == "horizontal")
                     return GetCellValue(worksheet, rowIndex, columnIndex - 1, ignoreType);
                else if (cell.Merge == true && ignoreType == "vertical")
                    return GetCellValue(worksheet, rowIndex - 1, columnIndex, ignoreType);

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get cell value with header text
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex">Hàng mục tiêu</param>
        /// <param name="headerText">Tên</param>
        /// <param name="startColumnIndex">Thiết lập dimenson cột header bắt đầu</param>
        /// <param name="startRowIndex">Thiết lập dimenson hàng header bắt đầu</param>
        /// <returns></returns>
        public static string GetCellValue(ExcelWorksheet worksheet, int rowIndex, string headerText, int? startRowIndex, int? startColumnIndex, string ignoreType)
        {
            return GetCellValue(worksheet, rowIndex, GetHeaderColumnIndex(worksheet, startRowIndex, startColumnIndex, headerText, ignoreType), null);
        }
        
        public static int GetHeaderColumnIndex(ExcelWorksheet worksheet, int? startRowIndex, int? startColumnIndex, string headerText, string ignoreType)
        {
            string initialHeaderText = "N/A";
            startRowIndex ??= 1;
            int index = startColumnIndex ?? 0;
            while(!string.IsNullOrEmpty(initialHeaderText))
            {
               
                initialHeaderText = GetCellValue(worksheet, startRowIndex.Value, index, ignoreType);
                
                if (initialHeaderText == headerText)
                    return index;
                if (index > worksheet.Dimension.Columns || index > 100)
                    return 0;
                index++;
            }
            return 0;
        }
    }
}