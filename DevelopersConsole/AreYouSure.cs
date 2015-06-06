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

namespace DeveloperConsole
{
    public partial class AreYouSure : Form
    {
        private Game game;

        public bool deleteSuccessful = false;

        public AreYouSure(Game game)
        {
            InitializeComponent();
            label1.Text += game.displayName + " version " + game.versionInteger + "?";
            this.game = game;
        }

        private void AreYouSure_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {


            deleteSuccessful = DatabaseHelper.deleteGame(game);
            Close();
        }
    }
}
