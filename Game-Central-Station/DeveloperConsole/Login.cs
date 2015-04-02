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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Globals.maintainDatabaseConnection();
                MySqlCommand command = new MySqlCommand("select * from accounts where username = \"" + textBox1.Text + "\";");
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int passhash = Int32.Parse(reader["password"].ToString());
                if (Globals.hash(textBox2.Text) == passhash)
                {
                    Globals.userName = textBox1.Text;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
