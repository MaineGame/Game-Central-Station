using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCSInstaller
{
    class Program
    {
        public static bool auto = false;

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Contains<string>("-auto")) auto = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu());

        }
    }
}
