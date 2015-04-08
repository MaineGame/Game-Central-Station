﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCentralStation.DeveloperConsole
{
    public partial class Restore : Form
    {
        public Restore()
        {
            InitializeComponent();
        }

        private Game[] games;

        private void Restore_Load(object sender, EventArgs e)
        {
            games = Globals.getGamesWhere("username = \"" + Globals.userName + "\" and ready = true and archived = true");
            listBox1.Items.Add(Game.headerGame);
            foreach (Game game in games)
                listBox1.Items.Add(game.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game game = games[listBox1.SelectedIndex - 1];
            Globals.maintainDatabaseConnection();
            try
            {
            
                MySqlCommand command = new MySqlCommand("update store set archived = true where uploadTimeStamp = '" + game.uploadTimeStamp + "';");
                command.Connection = Globals.connection;
                command.ExecuteNonQuery();

                command = new MySqlCommand("update store set archived = false where gameID = " + game.id + ";");
                command.Connection = Globals.connection;
                command.ExecuteNonQuery();

                MessageBox.Show("" + game.displayName + " sucessfully restored!");
                Close();
                


            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }
    }
}
