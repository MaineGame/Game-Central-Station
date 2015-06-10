using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCentralStation
{

    public class Debug
    {
        public static void log(string stuff)
        {
#if DEBUG
            MessageBox.Show(stuff);
#endif
        }

        internal static void ohFuck(Exception e)
        {
            throw new NotImplementedException();
        }
    }

    public class Globals
    {

        static Globals()
        {
            Globals.root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public const string FTPIP = "169.244.195.143";
        public const string FTPUser = "GCSUser";
        public const string password = "";
        private static Random random = new Random();
        public static string userName = null;

        public static void openGame(Game game)
        {
            try
            {
                if (process != null) process.Kill();
                process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = Globals.root + "\\games\\" + game.id + "\\" + game.executableName;
                process.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

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

            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 300; x++)
                {

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

        public static void loadArgs()
        {

            string[] args = File.ReadAllLines("config");

            Globals.args = args;

            kioskMode = args.Contains<string>("-K");
            if (kioskMode) Debug.log("Kiosk Mode enabled.");

        }

        public static bool
            kioskMode { get; set; }

        internal static void updateAll()
        {
            string[] games = Directory.GetDirectories("" + Globals.root + "\\games\\");
            foreach (string file in games)
            {
                
                string id = file.Substring(file.LastIndexOf('\\') + 1);

                Game game = DatabaseHelper.getGamesWhere("gameID = " + id)[0];
                string groupID = "" + game.idGroup;

                Game newGame = DatabaseHelper.getGamesWhere("idGroup = " + groupID + " and archived = false and ready = true")[0];

                if (game.id != newGame.id)
                {
                    Debug.log("Found update for " + game.displayName);
                    //TODO MAKE THIS SHIT MORE MODULAR. UGH.

                    #region nasty download

                    #region download and extracting the game data files

                    //establish a new webclient because downloading is a thing.
                    WebClient client = new WebClient();

                    Directory.CreateDirectory(Globals.root + "\\games");
                    Directory.CreateDirectory(Globals.root + "\\games\\" + newGame.id);

                    if (Directory.Exists(Globals.root + "\\games\\" + newGame.id))
                        //delete the old if you have one
                        Directory.Delete(Globals.root + "\\games\\" + newGame.id, true);

                    //download it
                    try
                    {
                        client.Credentials = new NetworkCredential(Globals.FTPUser, Globals.password);
                        client.DownloadFileTaskAsync(new Uri("ftp://" + Globals.FTPIP + "/games/" + newGame.id + "/current.zip"), "" + Globals.root + "\\games\\temp.zip").Wait();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    //then you know, actually start that bit...
                    ZipFile.ExtractToDirectory(Globals.root + "\\games\\temp.zip", Globals.root + "\\games\\" + newGame.id);
                    //File.Delete(Globals.root + "\\games\\temp.zip");

                    //houston, we're done here.
                    #endregion

                    #endregion

                }

            }

        }
    }

    //because everything pumps out of here a as a string.

}
