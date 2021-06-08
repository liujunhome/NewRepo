using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace TestDll
{
    public class MessageInfo
    {
        public MessageInfo()
        {

        }
        public void ShowInfo(string s)
        {
            MessageBox.Show(s);
        }
    }
}
