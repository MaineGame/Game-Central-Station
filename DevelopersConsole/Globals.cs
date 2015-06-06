using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeveloperConsole
{

    public class Debug
    {
        public static void log(string stuff)
        {
#if DEBUG
            MessageBox.Show (stuff);
#endif
        }

        internal static void ohFuck(Exception e)
        {
            throw new NotImplementedException();
        }
    }

    public class Globals
    {
        public const string FTPIP = "169.244.195.143";
        public const string FTPUser = "GCSUser";
        public const string password = "";
        private static Random random = new Random();
        public static string userName = null;
        

        //cant be const because has to be set a runtime.
        //but please don't change it?
        public static string root = "";

        public static Process process = null;

        public static string[] args;

        public static bool hasArg(string arg)
        {
            return args.Contains(arg);
        }

        public static int hash(String str)
        {
            int prime = 164973157;
            int sum = prime;
            foreach (char _char in str.ToCharArray())
                sum = sum * _char + prime;
            return sum;
        }

        /**
         * path starts with /
         */
        public static Stream getFile(string path)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + FTPIP + path);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(FTPUser, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            return responseStream;
        }

        internal static void sendErrorLog(Exception e)
        {
            throw new NotImplementedException();
        }


        

        private static Image controller = null;

        //TODO make this cached and stuff.

        public static Image createController()
        {

            if (Globals.controller == null)
                Globals.controller = Image.FromStream(Globals.getFile("/games/default.jpg"));

            Bitmap controller = new Bitmap((Image)Globals.controller.Clone());

            //find a pastel - y color multiplier
            double r = random.NextDouble() / 2 + .5;
            double g = random.NextDouble() / 2 + .5;
            double b = random.NextDouble() / 2 + .5;

            for(int y = 0; y < 100; y ++) {
                for(int x = 0; x < 300; x ++) {

                    //snag the color in
                    Color colorIn = controller.GetPixel(x, y);

                    //grab the components
                    int Rin = 0xFF & colorIn.R;
                    int Gin = 0xFF & colorIn.G;
                    int Bin = 0xFF & colorIn.B;

                    //multiplicitave overlay
                    int Rout = (int)(Rin * r);
                    int Gout = (int)(Gin * g);
                    int Bout = (int)(Bin * b);

                    //Console.WriteLine("Color: " + Rout + ", " + Gout + ", " + Bout);

                    //color out and set new pixel
                    Color colorOut = Color.FromArgb((255 << 24) | (Rout << 16) | (Gout << 8) | (Bout));
                    controller.SetPixel(x, y, colorOut);
                    
                }

            }


            return controller;
        }
    }

    //because everything pumps out of here a as a string.
    
}
