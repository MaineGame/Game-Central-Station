
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeveloperConsole
{
    class Program
    {
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new Login().ShowDialog();
            if (Globals.userName != null) new DeveloperMenu().ShowDialog();
        }
    }
}
