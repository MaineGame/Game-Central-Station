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
    public partial class Signup : Form
    {
        public Signup(string username, string password)
        {
            InitializeComponent();
            textBox1.Text = username;
            textBox2.Text = password;

        }

        private void Login_Load(object sender, EventArgs e)
        {
            label5.Text = "";
            if (textBox1.Text == "")
                textBox1.TabIndex = 0;
            else
                if (textBox2.Text == "")
                    textBox2.TabIndex = 0;
                else
                    textBox3.TabIndex = 0;
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

            try
            {
                MySqlCommand command = new MySqlCommand("insert into accounts values(\"" + textBox1.Text.ToLower() + "\", " + pass1Hash + ");");
                command.Connection = DatabaseHelper.connection;
                command.ExecuteNonQuery();
            }catch(Exception ex) {
                label5.Text = ex.Message;
                return;
            }

            Globals.userName = textBox1.Text;
            MessageBox.Show("Account created. Press Okay to continue to the Developer Console.");
            Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
