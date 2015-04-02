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
    public partial class Signup : Form
    {
        public Signup()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            label5.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int pass1Hash = Globals.hash(textBox2.Text);
            int pass2Hash = Globals.hash(textBox3.Text);
            
            if (pass1Hash != pass2Hash)
            {
                label5.Text = "Passwords do not match.";
                return;
            }

            Globals.maintainDatabaseConnection();


            try
            {
                MySqlCommand command = new MySqlCommand("insert into accounts values(\"" + textBox1.Text.ToLower() + "\", " + pass1Hash + ");");
                command.Connection = Globals.connection;
                command.ExecuteNonQuery();
            }catch(Exception ex) {
                label5.Text = ex.Message;
                return;
            }

            Globals.userName = textBox1.Text;

        }
    }
}
