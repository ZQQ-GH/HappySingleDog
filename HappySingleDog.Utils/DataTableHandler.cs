using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappySingleDog.Utils
{
    public static class DataTableHandler
    {
        public static string BuildInClause(this DataTable a_dtSource, string a_strFieldName, string a_RetWhenEmpty)
        {
            string l_strCdtn = "";
            string l_strComma = "";

            if (a_dtSource.Rows.Count == 0) return a_RetWhenEmpty;
            if(!a_dtSource.Columns.Contains(a_strFieldName)) return a_RetWhenEmpty;
            foreach (DataRow l_dr in a_dtSource.Rows)
            {
                l_strCdtn = l_strCdtn + l_strComma + l_dr[a_strFieldName].ToString().QuotedStr();
                l_strComma = ",";
            }
            if (l_strCdtn != "")
            {
                l_strCdtn = "(" + l_strCdtn + ")";
            }
            return l_strCdtn;
        }

        /// <summary>
        /// 将查询结果导入到表中返回
        /// </summary>
        /// <param name="a_tb"></param>
        /// <param name="filter">过滤条件</param>
        /// <param name="sort">排序条件</param>
        /// <returns></returns>
        public static DataTable SelectToTable(this DataTable a_tb, string a_filter, string a_sort ="")
        {
            ParseFilter(ref a_filter);
            a_tb.CaseSensitive = false;
            DataRow[] l_rows;
            if (a_sort == "")
            {
                l_rows = a_tb.Select(a_filter);
            }
            else
            {
                l_rows = a_tb.Select(a_filter, a_sort);
            }
            DataTable l_tbCopy = a_tb.Clone();
            foreach (DataRow row in l_rows)
            {
                l_tbCopy.ImportRow(row);
            }
            return l_tbCopy;
        }

        public static string ParseFilter(ref string a_strFilter)
        {
            //如果是等于号则不要替换，如果是like则需要替换
            a_strFilter = a_strFilter.Replace("*", "[*]");
            return a_strFilter;
        }
    }
}
