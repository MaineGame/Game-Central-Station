using MaterialSkin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Collections;
using System.IO.Compression;

namespace GameCentralStation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        [STAThread]
        static void Main(String[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var materialSkinManager = MaterialSkinManager.Instance;
                materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
                materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue700, Primary.LightBlue800, Primary.Red100, Accent.Amber200, TextShade.WHITE);
                new Connect().ShowDialog();

                if (Globals.offline)
                    materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey700, Primary.BlueGrey800, Primary.Red100, Accent.Cyan400, TextShade.WHITE);

                Application.Run(new Mist());
                
            }
            catch (Exception e)
            {
                reportError(e);
            }
        }

        private class SimpleError
        {
            public string message;
            public IDictionary data;
            public string trace;
        }

        private static void reportError(Exception e)
        {
#if !DEBUG
            //started outside so can be used in catch.
            string str = "";
            try
            {
                //create the output string
                str += "ERROR\n" + (Globals.userName == null ? "null" : Globals.userName) + "\n";
                //TODO maybe add room for headers or something later?
                var json = new JavaScriptSerializer().Serialize(new SimpleError()
                {
                    message = e.Message,
                    data = e.Data,
                    trace = e.StackTrace
                });
                str += json;

                //grab the server.
                TcpClient client = new TcpClient();
                client.Connect("localhost", 4272);
                //and the stream
                var stream = client.GetStream();
                //write the string to it.
                stream.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);

                stream.Close();


            }
            catch (Exception ex)
            {
                //welp, you like... dont have internet.
                //or the server is down.
                //either way, don't stress it yo.
                //just log this somewhere.
                //TODO
            }

            
#else
            Debug.log("" + e.Message);
#endif

        }

    }
}