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

namespace DeveloperConsole
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
            games = DatabaseHelper.getGamesWhere("username = \"" + Globals.userName + "\" and ready = true and archived = true");
            listBox1.Items.Add(Game.headerGame);
            foreach (Game game in games)
                listBox1.Items.Add(game.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game game = games[listBox1.SelectedIndex - 1];
            try
            {

                string commandString = "update store set archived = true where idGroup = " + game.idGroup + "; update store set archived = false where gameID = " + game.id + ";";
                MySqlCommand command = new MySqlCommand(commandString);
                command.Connection = DatabaseHelper.connection;
                command.ExecuteNonQuery();

                MessageBox.Show("" + game.displayName + " sucessfully restored!");
                Close();
                


            }
            catch (MySqlException ex)
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
