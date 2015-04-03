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
            try
            {
                MySqlCommand command = new MySqlCommand("SELECT * FROM store WHERE username = \"" + Globals.userName + "\";");
                command.Connection = Globals.connection;
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GameItem item = new GameItem()
                    {
                        name = reader["gameName"].ToString(),
                        id = Int32.Parse(reader["gameID"].ToString())
                    };
                    if (Boolean.Parse(reader["ready"].ToString()))
                        comboBox1.Items.Add(item);

                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

    public class GameItem
    {
        public string name { get; set; }
        public int id { get; set; }
        public bool ready { get; set; }

        public override string ToString()
        {
            return "" + id + " - " + name + (ready ? "" : " (unpublished)");
        }
    }

}
