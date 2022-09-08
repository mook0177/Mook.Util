using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Mook.Util
{
    /// <summary>
    /// Excel数据帮助类 
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// Excel数据转DataTable
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetIndex">sheet索引</param>
        /// <returns></returns>
        public static DataTable ToDataTable(string filePath, int sheetIndex = 0)
        {
            var dt = new DataTable();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = null;
                if (filePath.EndsWith(".xlsx"))
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else
                {
                    workbook = new HSSFWorkbook(fs);
                }
                var sheet = workbook.GetSheetAt(sheetIndex);
                var header = sheet.GetRow(sheet.FirstRowNum);
                var columns = new List<int>();
                if (header != null)
                {
                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        var obj = GetValueType(header.GetCell(i) as XSSFCell);
                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                        }
                        else
                        {
                            dt.Columns.Add(new DataColumn(obj.ToString().Trim()));
                        }
                        columns.Add(i);
                    }
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        var dr = dt.NewRow();
                        foreach (var col in columns)
                        {
                            dr[col] = GetValueType(sheet.GetRow(i).GetCell(col) as XSSFCell);
                            if (dr[col] != null && dr[col].ToString() != string.Empty)
                            {
                                dt.Rows.Add(dr);
                                break;
                            }
                        }
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// DataTable数据导出至Excel
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="dt">DataTable数据</param>
        /// <param name="isNeedHead">是否需要表头，默认需要</param>
        /// <param name="sheetName">sheet名，默认Sheet1</param>
        public static void ToExcel(string filePath, DataTable dt, bool isNeedHead = true, string sheetName = "Sheet1")
        {
            var xssfworkbook = new XSSFWorkbook();
            var sheet = xssfworkbook.CreateSheet(sheetName);
            var row = sheet.CreateRow(0);
            if (isNeedHead)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var cell = row.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var cell = row1.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row1 = sheet.CreateRow(i);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var cell = row1.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }
            }
            var stream = new MemoryStream();
            xssfworkbook.Write(stream);
            var buf = stream.ToArray();
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns></returns>
        private static object GetValueType(XSSFCell cell)
        {
            if (cell == null) return null;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return null;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                case CellType.Formula:
                default:
                    return "=" + cell.CellFormula;
            }
        }

        /// <summary>
        /// DataTable数据导出至CSV
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="dt">DataTable数据</param>
        public static void ToCSV(string filePath, DataTable dt)
        {
            FileStream fs;
            StreamWriter sw;
            if (!File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            }
            sw = new StreamWriter(fs, Encoding.UTF8);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string data = null;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// CSV数据转DataTable
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static DataTable CsvToDataTable(string filePath)
        {
            var dt = new DataTable();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var sr = new StreamReader(fs, Encoding.UTF8);
                string strLine = null;
                string[] arrayLine = null;
                bool isFirst = true;
                while ((strLine = sr.ReadLine()) != null)
                {
                    strLine = strLine.TrimEnd();
                    arrayLine = strLine.Split(',');
                    int dtColumns = arrayLine.Length;
                    if (isFirst)
                    {
                        isFirst = false;
                        for (int i = 0; i < dtColumns; i++)
                        {
                            string col = arrayLine[i].Trim();
                            if (dt.Columns.Contains(col))
                            {
                                dt.Columns.Add(col + "_");
                            }
                            else dt.Columns.Add(col);
                        }
                    }
                    else
                    {
                        var dataRow = dt.NewRow();
                        for (int i = 0; i < arrayLine.Length; i++)
                        {
                            dataRow[i] = arrayLine[i].Trim();
                        }
                        dt.Rows.Add(dataRow);
                    }
                }
                sr.Close();
                fs.Close();
            }
            return dt;
        }
    }
}