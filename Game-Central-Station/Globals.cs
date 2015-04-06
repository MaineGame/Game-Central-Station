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
                        ready = reader["ready"].ToString()
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
    }

    public class Game
    {

        public string id;
        public int versionInteger;
        public string version;
        public string name;
        public string executableName;
        public string displayName;
        public int zipLength;
        public bool ready;
        public bool archived;

        private Game(GameContract contract)
        {
            id = contract.id;
            versionInteger = Int32.Parse(contract.versionString);
            name = contract.name;
            executableName = contract.executableName;

            ready = Boolean.Parse(contract.ready);
            archived = Boolean.Parse(contract.archived);

            zipLength = Int32.Parse(contract.zipLength);

            displayName = name.Replace("&", "&&");
            /*
            int major = versionInteger / 1000000;
            int minor = (versionInteger - (major * 1000000)) / 10000;
            int build = (versionInteger - (major * 1000000) - (minor * 10000)) / 100;
            int revision = versionInteger - (major * 1000000) - (minor * 10000) - (build * 100);
            
            version = major + "." + minor + "." + build + "." + revision;
             */
            version = "" + versionInteger;
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
            return id + "\t" + (archived ? "Archived" : (ready ? "Published" : "Corrupt")) + "\t" + name;
        }

    }
}
