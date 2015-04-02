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
    public partial class LoginSignup : Form
    {
        public LoginSignup()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Login().ShowDialog();
            if(Globals.userName != null)
                Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoginSignup_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 27)
            {
                Close();
            }
        }

        private void LoginSignup_Load(object sender, EventArgs e)
        {
            if (Globals.userName != null)
            {
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Signup().ShowDialog();
            if (Globals.userName != null)
                Close();
        }
    }
}
