using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCentralStation
{
    class DatabaseHelper
    {

        public static MySqlConnection connection = null;
        public const string RDSDOMAIN = "mainegamesteam.cbzhynv0adrl.us-east-1.rds.amazonaws.com";

        public static Game[] getGamesWhere(string where)
        {
            lock (connection)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception e)
                {



                }
                Debug.log("" + connection.State);

                //TODO if this ever ACTUALLY tries to open up a dialog, it will fail because
                //materialskin and cross threadin even nastier than winforms cross threading.

                List<Game> games = new List<Game>();
                MySqlCommand command = new MySqlCommand();
                command.CommandText = "Select * from store where " + where + ";";

                command.Connection = connection;
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
        }

        internal static bool deleteGame(Game game)
        {
            try
            {
                MySqlCommand command = new MySqlCommand("update store set archived = true where gameID = " + game.id + ";");
                command.Connection = connection;

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
}
