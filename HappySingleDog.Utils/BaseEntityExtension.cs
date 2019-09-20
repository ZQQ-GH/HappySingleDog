using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBF.Data;

namespace HappySingleDog.Utils
{
    public static class BaseEntityExtension
    {
        /// <summary>
        /// 导入数据行，并且定位到最后一行
        /// </summary>
        /// <param name="a_dataset"></param>
        /// <param name="a_dr"></param>
        public static void ImportRow(this BaseEntity a_dataset, DataRow a_dr)
        {
            a_dataset.DataTable.ImportRow(a_dr);
            a_dataset.Last();
        }
    }
}
