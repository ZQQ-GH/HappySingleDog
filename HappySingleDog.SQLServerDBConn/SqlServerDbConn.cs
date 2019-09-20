using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace HappySingleDog.SQLServerDBConn
{
    public class SqlServerDbConn
    {
        private string ConnString = "";//连接字符串
        private const int MaxPool = 20;//最大连接数
        private const int MinPool = 5;//最小连接数
        private const bool Asyn_Process = true;//设置异步访问数据库
        private const bool Mars = true;//是否复用连接（只使用sql server 2005）
        private const int Conn_Timeout = 15;//设置连接等待时间
        private const int Conn_Lifetime = 15;//设置连接的生命周期
        private SqlConnection SqlDrConn = null;//连接对象
        public DataSet Data =new DataSet();
        public SqlServerDbConn()//构造函数
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString.ToString();
            SqlDrConn = new SqlConnection(connectionString);
        }

        public SqlServerDbConn(string dataBase)//构造函数
        {
            ConnString = GetConnString(dataBase);
            SqlDrConn = new SqlConnection(ConnString);
        }

        public SqlServerDbConn(string server, string port, string dataBase, string userId, string passWord)//构造函数
        {
            ConnString = GetConnString(server, port, dataBase, userId, passWord);
            SqlDrConn = new SqlConnection(ConnString);
        }

        private string GetConnString(string dataBase)
        {

            return "server=localhost;"
            + "integrated security=sspi;"
            + "database=" + dataBase + ";"
            + "Max Pool Size=" + MaxPool + ";"
            + "Min Pool Size=" + MinPool + ";"
            + "Connect Timeout=" + Conn_Timeout + ";"
            + "Connection Lifetime=" + Conn_Lifetime + ";"
            + "Asynchronous Processing=" + Asyn_Process + ";";
            //+"MultipleActiveResultSets="+Mars+";";
        }

        private string GetConnString(string server, string port, string dataBase, string userId, string passWord)
        {

            return "server=" + server + "," + port + ";"
            + "database =" + dataBase + ";"
            + "User id =" + userId + "; "
            + "password =" + passWord + ";"
            + "Max Pool Size=" + MaxPool + ";"
            + "Min Pool Size=" + MinPool + ";"
            + "Connect Timeout=" + Conn_Timeout + ";"
            + "Connection Lifetime=" + Conn_Lifetime + ";"
            + "Asynchronous Processing=" + Asyn_Process + ";";
            //+"MultipleActiveResultSets="+Mars+";";
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="StrSql">sql语句</param>
        /// <returns></returns>
        public bool GetData(string StrSql)//数据查询
        {
            //当连接处于打开状态时关闭,然后再打开,避免有时候数据不能及时更新
            if (SqlDrConn.State == ConnectionState.Open)
            {
                SqlDrConn.Close();
            }
            try
            {
                SqlDrConn.Open();
                SqlDataAdapter DataAdapter = new SqlDataAdapter(StrSql, SqlDrConn);
                Data.Clear();
                DataAdapter.Fill(Data);
                SqlDrConn.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
            finally
            {
                SqlDrConn.Close();
            }
        }

        /// <summary>
        /// 执行增、删、改
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <returns></returns>
        public bool ExeCommand(string sqlStr)
        {
            if (SqlDrConn.State == ConnectionState.Open)
            {
                SqlDrConn.Close();
            }
            try
            {
                SqlDrConn.Open();
                SqlCommand Command = new SqlCommand(sqlStr, SqlDrConn);
                int Succnum = Command.ExecuteNonQuery();
                SqlDrConn.Close();
                return Succnum > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show(sqlStr);
                return false;
            }
            finally
            {
                SqlDrConn.Close();
            }
        }

        /// <summary>
        /// 插入表数据到数据库表中
        /// </summary>
        /// <param name="aimTable">目的表</param>
        /// <param name="sourceTable">传入表</param>
        /// <returns></returns>
        public bool InsertDataToDataBase(string aimTable, DataTable sourceTable)
        {
            if (sourceTable.Rows.Count == 0)
            {
                return true;
            }

            string strsql = string.Format(" select a.name,b.name as type from syscolumns a" +
                " inner join systypes b on a.xtype=b.xtype " +
                " where id = (select max(id) from sysobjects where xtype = 'u' and name = '{0}') and a.name<>'id'", aimTable);
            DataTable a_dtColumnName = new DataTable();
            if (GetData(strsql))
            {
                a_dtColumnName = Data.Tables[0];
            }
            if (a_dtColumnName.Rows.Count == 0)
            {
                MessageBox.Show(string.Format("目标表:[{0}]在数据库中不存在", aimTable));
                return false;
            }

            StringBuilder a_strColumns = new StringBuilder("(");
            for (int i = 0; i < a_dtColumnName.Rows.Count; i++)
            {
                a_strColumns.Append(a_dtColumnName.Rows[i]["name"].ToString());
                if (i < a_dtColumnName.Rows.Count - 1)
                {
                    a_strColumns.Append(",");
                }
                else
                {
                    a_strColumns.Append(")");
                }
            }

            StringBuilder a_strValues = new StringBuilder("");
            for (int i = 0; i < sourceTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    a_strValues.Append(" select ");
                }
                else
                {
                    a_strValues.Append(" union all select ");
                }
                int a_intFailCount = 0;
                for (int j = 0; j < a_dtColumnName.Rows.Count; j++)
                {
                    if (a_dtColumnName.Rows[j]["name"].ToString().Equals("id")) continue;
                    if (sourceTable.Columns.Contains(a_dtColumnName.Rows[j]["name"].ToString()))
                    {
                        a_strValues.Append(" '" + sourceTable.Rows[i][a_dtColumnName.Rows[j]["name"].ToString()].ToString() + "' ");
                    }
                    else
                    {
                        if (a_dtColumnName.Rows[j]["type"].ToString().Equals("int")
                            || a_dtColumnName.Rows[j]["type"].ToString().Equals("float"))
                        {
                            a_strValues.Append("0");
                        }
                        else if (a_dtColumnName.Rows[j]["type"].ToString().Equals("datetime"))
                        {
                            a_strValues.Append("'1900-01-01'");
                        }
                        else
                        {
                            a_strValues.Append("''");
                        }

                        a_intFailCount++;
                    }
                    if (j < a_dtColumnName.Rows.Count - 1)
                    {
                        a_strValues.Append(",");
                    }
                }
                if (a_intFailCount == a_dtColumnName.Rows.Count - 1)
                {
                    MessageBox.Show("插入表与目标表无相同列!");
                    return false;
                }
            }
            string result = string.Format("insert into {0}{1}{2}", aimTable, a_strColumns, a_strValues);
            if( ExeCommand(result))
            {
                return true;
            }
            else
            {
                MessageBox.Show(string.Format("插入表数据失败:{0}", aimTable));
                return false;
            }
        }

        /// <summary>
        /// 更新数据到数据库表中
        /// </summary>
        /// <param name="aimTable">目标表</param>
        /// <param name="sourceTable">传入表</param>
        /// <param name="a_strKey">关键字</param>
        /// <param name="a_strUpdateFeild">需要更新的字段，传入null则全部更新</param>
        /// <param name="a_strCondition">SQL条件</param>
        /// <returns></returns>
        public bool UpdateDataToDataBase(string aimTable, DataTable sourceTable,
            string[] a_strKey, string[] a_strUpdateFeild, string a_strCondition)
        {
            if (a_strKey == null || a_strKey.Length == 0)
            {
                MessageBox.Show("未设置关键字!");
                return false;
            }
            if (sourceTable.Rows.Count == 0)
            {
                return true;
            }
            //获取目标表的所有字段，除id
            string strsql = string.Format(" select a.name,b.name as type from syscolumns a" +
                " inner join systypes b on a.xtype=b.xtype " +
                " where id = (select max(id) from sysobjects where xtype = 'u' and name = '{0}') and a.name<>'id'", aimTable);
            DataTable a_dtColumnName = new DataTable();
            if (GetData(strsql))
            {
                a_dtColumnName = Data.Tables[0];
            }

            if (a_dtColumnName.Rows.Count == 0)
            {
                MessageBox.Show(string.Format("目标表:[{0}]在数据库中不存在", aimTable));
                return false;
            }

            StringBuilder a_strSql = new StringBuilder("");
            for (int i = 0; i < sourceTable.Rows.Count; i++)
            {
                a_strSql.Append(string.Format("update {0} set ", aimTable));

                string l_strDot = "";
                if (a_strUpdateFeild != null && a_strUpdateFeild.Length > 0)
                {
                    foreach (string a_str in a_strUpdateFeild)
                    {
                        if (a_dtColumnName.Select(string.Format("name ='{0}'", a_str)).Length == 0)
                        {
                            MessageBox.Show(string.Format("字段:{0},在目标表中不存在!", a_str));
                            return false;
                        }
                        if (!sourceTable.Columns.Contains(a_str))
                        {
                            MessageBox.Show(string.Format("字段:{0},在传入表中不存在!", a_str));
                            return false;
                        }
                        a_strSql.Append(l_strDot + a_str + "='" + sourceTable.Rows[i][a_str].ToString() + "'");
                        l_strDot = ",";
                    }
                }
                else
                {
                    int a_intSuccCount = 0;
                    for (int j = 0; j < sourceTable.Columns.Count; j++)
                    {
                        string l_strColumnName = sourceTable.Columns[j].ColumnName;
                        if (a_dtColumnName.Select(string.Format("name ='{0}'", l_strColumnName)).Length == 0)
                        {
                            continue;
                        }
                        a_intSuccCount++;
                        a_strSql.Append(l_strDot + l_strColumnName + "='" + sourceTable.Rows[i][l_strColumnName].ToString() + "'");
                        l_strDot = ",";
                    }
                    if (a_intSuccCount == 0)
                    {
                        MessageBox.Show("传入表与目标表无相同列!");
                        return false;
                    }
                }
                string l_strCndt = " where ";
                foreach (string a_str in a_strKey)
                {
                    if (a_dtColumnName.Select(string.Format("name ='{0}'", a_str)).Length == 0)
                    {
                        MessageBox.Show(string.Format("关键字字段:{0},在目标表中不存在!", a_str));
                        return false;
                    }
                    if (!sourceTable.Columns.Contains(a_str))
                    {
                        MessageBox.Show(string.Format("关键字字段:{0},在传入表中不存在!", a_str));
                        return false;
                    }
                    a_strSql.Append(l_strCndt + a_str + " = '" + sourceTable.Rows[i][a_str].ToString() + "'");
                    l_strCndt = " and ";
                }
                if (a_strCondition != null && a_strCondition.Trim().Length != 0)
                {
                    a_strSql.Append("  " + a_strCondition);
                }
                a_strSql.Append(";");
            }
            //MessageBox.Show(a_strSql.ToString());
            if(ExeCommand(a_strSql.ToString()))
            {
                return true;
            }
            else
            {
                MessageBox.Show(string.Format("更新表数据失败:{0}", aimTable));
                return false;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="aimTable">目标表</param>
        /// <param name="sourceTable">传入表</param>
        /// <param name="a_strKey">关键字，不可为空</param>
        /// <param name="a_strCondition">SQL条件，没有传null</param>
        /// <returns></returns>
        public bool DelateDataInDataBase(string aimTable, DataTable sourceTable,
                                                                  string[] a_strKey, string a_strCondition)
        {
            if (a_strKey == null || a_strKey.Length == 0)
            {
                MessageBox.Show("未设置关键字!");
                return false;
            }
            if (sourceTable.Rows.Count == 0)
            {
                return true;
            }
            //获取目标表的所有字段，除id
            string strsql = string.Format(" select a.name,b.name as type from syscolumns a" +
                " inner join systypes b on a.xtype=b.xtype " +
                " where id = (select max(id) from sysobjects where xtype = 'u' and name = '{0}') and a.name<>'id'", aimTable);
            DataTable a_dtColumnName = new DataTable();
            if (GetData(strsql))
            {
                a_dtColumnName = Data.Tables[0];
            }

            if (a_dtColumnName.Rows.Count == 0)
            {
                MessageBox.Show(string.Format("目标表:[{0}]在数据库中不存在", aimTable));
                return false;
            }
            StringBuilder a_strSql = new StringBuilder("");
            for (int i = 0; i < sourceTable.Rows.Count; i++)
            {
                a_strSql.Append(string.Format("delete from {0} where ", aimTable));
                string l_strAnd = "";
                foreach (string a_str in a_strKey)
                {
                    if (a_dtColumnName.Select(string.Format("name ='{0}'", a_str)).Length == 0)
                    {
                        MessageBox.Show(string.Format("关键字字段:{0},在目标表中不存在!", a_str));
                        return false;
                    }
                    if (!sourceTable.Columns.Contains(a_str))
                    {
                        MessageBox.Show(string.Format("关键字字段:{0},在传入表中不存在!", a_str));
                        return false;
                    }
                    a_strSql.Append(l_strAnd + a_str + " ='" + sourceTable.Rows[i][a_str].ToString() + "'");
                    l_strAnd = " and ";
                }
                if (a_strCondition != null && a_strCondition.Trim().Length != 0)
                {
                    a_strSql.Append("  " + a_strCondition);
                }
                a_strSql.Append(";");
            }
            //MessageBox.Show(a_strSql.ToString());
            if (ExeCommand(a_strSql.ToString()))
            {
                return true;
            }
            else
            {
                MessageBox.Show(string.Format("删除表数据失败:{0}", aimTable));
                return false;
            }
        }
    }
}
