﻿using MySql.Data.MySqlClient;
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
            if (Globals.CheckForInternetConnection())
            {
                #region internet related task
            lock (connection)
            {
                if (connection.State != System.Data.ConnectionState.Open) connection.Open();

                //if this ever ACTUALLY tries to open up a dialog, it will fail because
                //materialskin and cross threadin even nastier than winforms cross threading.
                //yeah, that was fixed. i just assume the database works now. and if it doesn't,
                //yell out why. at the developer.

                List<Game> games = new List<Game>();
                MySqlCommand command = new MySqlCommand();
                command.CommandText = "Select * from store where " + where + ";";

                command.Connection = connection;
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
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

                reader.Close();

                return games.ToArray<Game>();
            }
            #endregion
            }
            else
            {
                Globals.restartInOffline();
                return null;
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
