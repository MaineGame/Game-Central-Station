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
        public Account_Details()
        {
            InitializeComponent();
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
            label1.Text = "You are logged in as: " + Globals.userName;
            label2.Text = "Password: ********";
            
            MySqlCommand command = new MySqlCommand("SELECT * FROM store WHERE username = \"" + Globals.userName + "\";");
            command.Connection = Globals.connection;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                GameItem item = new GameItem()
                {
                    name = reader["gameName"].ToString(),
                    id = Int32.Parse(reader["gameID"].ToString()),
                    ready = Boolean.Parse(reader["ready"].ToString())
                };
                listBox1.Items.Add(item);



            }
            reader.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Globals.userName = null;
            Close();
        }
    }
}
