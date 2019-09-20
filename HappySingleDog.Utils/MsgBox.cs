using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HappySingleDog.Utils
{
    public static class MsgBox
    {
        public static DialogResult MsgQuestion(string msg)
        {
            return MessageBox.Show(msg, "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        }

        public static DialogResult MsgWarn(string msg)
        {
            return MessageBox.Show(msg, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static DialogResult MsgPrompt(string msg)
        {
            return MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult MsgSaveSuccess()
        {
            return MsgPrompt("保存成功!");
        }

        public static DialogResult MsgDeleteSuccess()
        {
            return MsgPrompt("删除成功!");
        }

        public static DialogResult MsgModifySuccess()
        {
            return MsgPrompt("修改成功!");
        }

    }
}
