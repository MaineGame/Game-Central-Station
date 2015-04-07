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
    public partial class Delete : Form
    {
        public Delete()
        {
            InitializeComponent();
        }

        private Game[] games;

        private void Delete_Load(object sender, EventArgs e)
        {
            games = Globals.getGamesWhere("username = \"" + Globals.userName + "\" and archived = false and ready = true");
            listBox1.Items.Add(Game.headerGame);
            foreach (Game game in games)
                listBox1.Items.Add(game.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game toDelete = games[listBox1.SelectedIndex - 1];
            AreYouSure delete = new AreYouSure(toDelete);
            delete.ShowDialog();

            if (delete.deleteSuccessful)
            {
                Close();
                MessageBox.Show("" + toDelete.displayName + " successfully deleted.");
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
