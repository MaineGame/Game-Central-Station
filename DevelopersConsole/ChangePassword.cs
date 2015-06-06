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
    public partial class ChangePassword : Form
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlCommand command = new MySqlCommand("select * from accounts where username = \"" + Globals.userName + "\";");
            command.Connection = DatabaseHelper.connection;
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int passwordHash = Int32.Parse(reader["password"].ToString());
            reader.Close();
            if (passwordHash == Globals.hash(textBox1.Text))
            {
                int newpass1 = Globals.hash(textBox2.Text);
                int newpass2 = Globals.hash(textBox3.Text);
                if(newpass1 == newpass2) {

                    command = new MySqlCommand("update accounts set password = " + newpass1 + " where username = \"" + Globals.userName + "\"");
                    command.Connection = DatabaseHelper.connection;
                    command.ExecuteNonQuery();

                    MessageBox.Show("successfully updated password!");

                    Close();

                }
                else
                {
                    MessageBox.Show("New passwords do not match");
                }
            }
            else
            {
                MessageBox.Show("Current password incorrect");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
