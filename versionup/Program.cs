using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace versionup
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            start.refreshlogo();
           // start.login("hjh", "123");
            var listversions = start.checkversion();
            if (listversions.Count == 0)
            {
                Process.Start(Application.StartupPath + "//main.exe", "123654");
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new down(listversions));
            }
        }
    }
}
