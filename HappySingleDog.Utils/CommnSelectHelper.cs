using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HappySingleDog.Utils
{
    public static class CommnSelect
    {
        public static DataTable CallCommonAssembly(string assemble,string method,object[] parames =null)
        {
            try
            {
                string l_strDll = "plugins/" + assemble + ".dll";
                Assembly ass = Assembly.LoadFrom(l_strDll);
                Type type = ass.GetType(assemble + ".Controller");//项目名称+类的名称
                object ob = Activator.CreateInstance(type);
                MethodInfo show = type.GetMethod(method);//类中的方法名称
                return  (DataTable)show.Invoke(ob, parames);
            }

            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return new DataTable();
            }
        }
    }
}
