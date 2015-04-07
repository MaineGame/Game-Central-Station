using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCentralStation.DeveloperConsole
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
        }

        private Game[] games;

        private void Update_Load(object sender, EventArgs e)
        {
            games = Globals.getGamesWhere("username = \"" + Globals.userName + "\" and archived = false and ready = true");
            foreach (Game game in games)
                comboBox1.Items.Add(game.displayName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Game game = games[comboBox1.SelectedIndex];

            Upload upload = new Upload(game);

            upload.ShowDialog();

            if (upload.success)
            {
                Globals.deleteGame(game);
            }

        }

    }

}
