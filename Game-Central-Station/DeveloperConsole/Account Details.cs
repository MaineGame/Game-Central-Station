using MySql.Data.MySqlClient;
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
    public partial class Account_Details : Form
    {

        private Game[] games;

        public Account_Details()
        {
            InitializeComponent();
            
            label1.Text = "You are logged in as: " + Globals.userName;
            label2.Text = "Password: ********";

            games = DatabaseHelper.getGamesWhere("username = '" + Globals.userName + "'");
            listBox1.Items.Add(Game.headerGame.ToString());
            foreach (Game game in games)
                listBox1.Items.Add(game.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ChangePassword().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Account_Details_Load(object sender, EventArgs e)
        {
            


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Globals.userName = null;
            Close();
        }
    }
}
