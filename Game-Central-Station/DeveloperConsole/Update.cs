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
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
        }

        private Game[] games;

        private void Update_Load(object sender, EventArgs e)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM STORE WHERE username = \"" + Globals.userName + "\";");
            command.Connection = Globals.connection;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                GameItem item = new GameItem() {
                    name = reader["gameName"].ToString(),
                    id = Int32.Parse(reader["gameID"].ToString())
                };

                comboBox1.Items.Add(item);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.CheckFileExists)
            {
                textBox2.Text = 
            }
        }
    }

    private class GameItem
    {
        public string name { get; set; }
        public int id { get; set; }

        public string ToString()
        {
            return "" + id + " - " + name;
        }
    }
}
