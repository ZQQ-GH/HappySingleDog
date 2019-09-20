using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HappySingleDog.SQLServerDBConn;
using ZQQ.Controls;
using WBF.Controls;
using System.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace HappySingleDog.Utils
{
    public static class ComBoxHelper
    {
        public static void Register(this System.Windows.Forms.ComboBox comboBox, string a_strType)
        {
            string sqlStr = string.Format("select * from simple_code where simple_type ='{0}'", a_strType);
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(sqlStr))
            {
                comboBox.Items.Clear();
                comboBox.DataSource = SSD.Data.Tables[0];
                comboBox.DisplayMember = "simple_name_cn";
                comboBox.ValueMember = "simple_no";
            }
        }

        public static void Register(this System.Windows.Forms.ComboBox comboBox, string a_strSql,string s_strDisplay,string s_strValue)
        {
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(a_strSql))
            {
                comboBox.Items.Clear();
                comboBox.DataSource = SSD.Data.Tables[0];
                comboBox.DisplayMember = s_strDisplay;
                comboBox.ValueMember = s_strValue;
            }
        }

        public static void Register(this QLComboBox  comboBox, string a_strType)
        {
            string sqlStr = string.Format("select * from simple_code where simple_type ='{0}'", a_strType);
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(sqlStr))
            {
                DBComboBox a_cbo = (comboBox.InnerEditor) as DBComboBox;
                a_cbo.Items.Clear();
                foreach (DataRow l_row in SSD.Data.Tables[0].Rows)
                {
                    a_cbo.AddItem(l_row["simple_name_cn"].ToString(), l_row["simple_no"].ToString());
                }
            }
        }

        public static void Register(this QLComboBox comboBox, string a_strSql, string s_strDisplay, string s_strValue)
        {
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(a_strSql))
            {
                DBComboBox a_cbo = (comboBox.InnerEditor) as DBComboBox;
                a_cbo.Items.Clear();
                foreach (DataRow l_row in SSD.Data.Tables[0].Rows)
                {
                    a_cbo.AddItem(l_row[s_strDisplay].ToString(), l_row[s_strValue].ToString());
                }
            }
        }

        public static void Register(this RepositoryItemLookUpEdit l_edit, string a_strType)
        {
            string sqlStr = string.Format("select * from simple_code where simple_type ='{0}'", a_strType);
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(sqlStr))
            {
                l_edit.PopupFormMinSize = new System.Drawing.Size(10, 10);
                l_edit.PopupFormSize = new System.Drawing.Size(10, 10);
                l_edit.Columns.Clear();
                l_edit.Columns.Add(new LookUpColumnInfo("simple_name_cn",""));
                l_edit.ShowFooter = false;
                l_edit.ShowHeader = false;
                l_edit.ReadOnly = false;
                l_edit.DisplayMember = "simple_name_cn";
                l_edit.ValueMember = "simple_no";
                l_edit.TextEditStyle = TextEditStyles.DisableTextEditor;
                l_edit.Properties.DataSource = SSD.Data.Tables[0];
                l_edit.DropDownRows = Math.Min(10, SSD.Data.Tables[0].Rows.Count);
                l_edit.NullText = "";
            }
        }

        public static void Register(this RepositoryItemLookUpEdit l_edit, string a_strSql, string s_strDisplay, string s_strValue)
        {
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(a_strSql))
            {
                l_edit.PopupFormMinSize = new System.Drawing.Size(10, 10);
                l_edit.PopupFormSize = new System.Drawing.Size(10, 10);
                l_edit.Columns.Clear();
                l_edit.Columns.Add(new LookUpColumnInfo(s_strDisplay, ""));
                l_edit.ShowFooter = false;
                l_edit.ShowHeader = false;
                l_edit.ReadOnly = false;
                l_edit.DisplayMember = s_strDisplay;
                l_edit.ValueMember = s_strValue;
                l_edit.TextEditStyle = TextEditStyles.DisableTextEditor;
                l_edit.Properties.DataSource = SSD.Data.Tables[0];
                l_edit.DropDownRows = Math.Min(10, SSD.Data.Tables[0].Rows.Count);
                l_edit.NullText = "";
            }
        }

        public static void Register(this RepositoryItemComboBox l_edit, string a_strType)
        {
            string sqlStr = string.Format("select * from simple_code where simple_type ='{0}'", a_strType);
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(sqlStr))
            {
                l_edit.Items.Clear();
                foreach (DataRow row in SSD.Data.Tables[0].Rows)
                {
                    l_edit.Items.Add( row["simple_name_cn"].ToString());
                }
                l_edit.TextEditStyle = TextEditStyles.Standard;
            }
        }

        public static void Register(this RepositoryItemComboBox l_edit, string a_strSql, string s_strDisplay, string s_strValue)
        {
            SqlServerDbConn SSD = new SqlServerDbConn();
            if (SSD.GetData(a_strSql))
            {
                l_edit.Items.Clear();
                foreach (DataRow row in SSD.Data.Tables[0].Rows)
                {
                    l_edit.Items.Add(s_strDisplay);
                }
                l_edit.TextEditStyle = TextEditStyles.Standard;
            }
        }
    }
}
