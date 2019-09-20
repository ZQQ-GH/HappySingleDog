using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBF.Controls;
using Microsoft.Office.Interop.Excel;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

namespace HappySingleDog.Utils
{
    public static class DBGridHelper
    {
        public static void ToExcel(this DBGrid a_dbg, bool isShowExcle = true)
        {
            GridView a_gv = a_dbg.MainView as GridView;
            //创建Excel对象
            Application excel = new Application();
            excel.Application.Workbooks.Add(true);

            //生成字段名称
            for (int i = 0; i < a_gv.Columns.Count; i++)
            {
                if (!a_gv.Columns[i].Visible) continue;
                if (string.IsNullOrEmpty(a_gv.Columns[i].Caption) && string.IsNullOrEmpty(a_gv.Columns[i].FieldName)) continue;
                excel.Cells[1, i + 1] = a_gv.Columns[i].Caption;
            }
            //填充数据
            for (int i = 0; i < a_gv.RowCount; i++)   //循环行
            {
                int j = 0;
                foreach (GridColumn col in a_gv.Columns)
                {
                    if (!col.Visible) continue;
                    if (string.IsNullOrEmpty(col.Caption) && string.IsNullOrEmpty(col.FieldName)) continue;
                    excel.Cells[i + 2, j + 1] = Convert.ToString(a_gv.GetRowCellValue(i, col));
                    j++;
                }
            }
            //设置禁止弹出保存和覆盖的询问提示框  
            excel.Visible = isShowExcle;
            excel.DisplayAlerts = false;
            excel.AlertBeforeOverwriting = false;
            //保存到临时工作簿
            //excel.Application.Workbooks.Add(true).Save();
            //保存文件
            //excel.Save("D:" + "\\234.xls");
            //excel.Quit();
        }
    }
}
