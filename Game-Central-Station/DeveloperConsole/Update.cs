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
            games = Globals.getGamesWhere("username = " + Globals.userName);
            foreach(Game game in games)
                comboBox1.Items.Add(game);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.CheckFileExists)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

    }

}
