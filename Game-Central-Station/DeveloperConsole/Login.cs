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
            if (Globals.userName != null) Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            attemptLogin();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                attemptLogin();
            }
        }

        public void attemptLogin()
        {
            MySqlDataReader reader;
            try
            {
                MySqlCommand command = new MySqlCommand("select * from accounts where username = \"" + textBox1.Text + "\";");
                command.Connection = Globals.connection;
                reader = command.ExecuteReader();
                reader.Read();
                int passhash = Int32.Parse(reader["password"].ToString());
                if (Globals.hash(textBox2.Text) == passhash)
                {
                    Globals.userName = textBox1.Text;
                    reader.Close();
                    Close();

                }
                else
                {
                    MessageBox.Show("Username and password combination incorrect.");
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Username and password combination incorrect.", "Error: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Signup(textBox1.Text, textBox2.Text).ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
