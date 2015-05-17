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

namespace GameCentralStation
{

    public class Globals
    {
        public const string RDSDOMAIN = "mainegamesteam.cbzhynv0adrl.us-east-1.rds.amazonaws.com";
        public const string FTPIP = "169.244.195.143";
        public const string FTPUser = "GCSUser";
        public const string password = "";
        public static string userName = "rbowden";
        private static Random random = new Random();

        public static Game[] getGamesWhere(string where)
        {
            //TODO if this ever ACTUALLY tries to open up a dialog, it will fail because
            //materialskin and cross threadin even nastier than winforms cross threading.
            Globals.maintainDatabaseConnection();

            List<Game> games = new List<Game>();

            MySqlCommand command = new MySqlCommand();
            command.CommandText = "Select * from store where " + where + ";";
            command.Connection = Globals.connection;
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    GameContract contract = new GameContract
                    {
                        executableName = reader["executable"].ToString(),
                        versionString = reader["gameVersion"].ToString(),
                        name = reader["gameName"].ToString(),
                        id = reader["gameID"].ToString(),
                        zipLength = reader["zipLength"].ToString(),
                        archived = reader["archived"].ToString(),
                        ready = reader["ready"].ToString(),
                        uploadTimeStamp = reader["uploadTimeStamp"].ToString(),
                        idGroup = reader["idGroup"].ToString()
                    };
                    Game game = Game.getGame(contract);
                    if (contract != null)
                        games.Add(game);
                }
                catch (Exception e)
                {
                    //some part of this listing was malformed.
                    Globals.sendErrorLog(e);
                }
            }

            reader.Close();

            return games.ToArray<Game>();
        }

        //cant be const because has to be set a runtime.
        //but please don't change it?
        public static string root = "";

        public static MySqlConnection connection = null;
        public static Process process = null;

        //quick thing to call before accessing database commands just to be sure.
        //also, should always call this from UI thread as it opens up a dialog.
        public static void maintainDatabaseConnection()
        {
            //if for some reason that connection goes bad, reconnect it.
            while (Globals.connection == null
                || Globals.connection.State != ConnectionState.Open
                || Globals.connection.IsPasswordExpired)

                new Connect().ShowDialog();
        }

        public static Tab convert(string tab)
        {
            switch (tab.ToLower())
            {
                case "strore":
                    return Tab.STORE;
                case "library":
                    return Tab.LIBRARY;
                default:
                    return Tab.NOT_SET;
            }
        }


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


        internal static bool deleteGame(Game game)
        {

            Globals.maintainDatabaseConnection();
            try
            {
                MySqlCommand command = new MySqlCommand("update store set archived = true where gameID = " + game.id + ";");
                command.Connection = Globals.connection;

                command.ExecuteNonQuery();
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message);
                return false;
            }

        }

        private static Image controller = null;

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

    public enum Tab
    {
        STORE,
        LIBRARY,
        NOT_SET
    }

    //because everything pumps out of here a as a string.
    public class GameContract
    {
        public string id { get; set; }

        public string name { get; set; }

        public string executableName { get; set; }

        public string versionString { get; set; }

        public string zipLength { get; set; }

        public string ready { get; set; }

        public string archived { get; set; }

        public string uploadTimeStamp { get; set; }

        public string idGroup { get; set; }
    }

    public class Game
    {
        public class HeaderGame : Game
        {
            public HeaderGame()
                : base(null)
            {
            }


            public override string ToString()
            {
                return "Version\tStatus\tName";
            }
        }

        public static Game headerGame = new HeaderGame();

        public string id;
        public int versionInteger;
        public string version;
        public string name;
        public string executableName;
        public string displayName;
        public int zipLength;
        public bool ready;
        public bool archived;
        public string uploadTimeStamp;
        public int idGroup;

        private Game(GameContract contract)
        {
            if (contract != null)
            {
                id = contract.id;
                versionInteger = Int32.Parse(contract.versionString);
                name = contract.name;
                executableName = contract.executableName;
                ready = Boolean.Parse(contract.ready);
                archived = Boolean.Parse(contract.archived);
                zipLength = Int32.Parse(contract.zipLength);
                displayName = name.Replace("&", "&&");
                version = "" + versionInteger;
                this.uploadTimeStamp = contract.uploadTimeStamp;
                this.idGroup = Int32.Parse(contract.idGroup);
            }
        }

        public static Game getGame(GameContract contract)
        {
            try
            {
                Game game = new Game(contract);
                return game;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return versionInteger + "\t" + (archived ? "Archived" : (ready ? "Published" : "Corrupt")) + "\t" + name;
        }

    }
}
