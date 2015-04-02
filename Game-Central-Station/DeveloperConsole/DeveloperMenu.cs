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
    public partial class DeveloperMenu : Form
    {
        public DeveloperMenu()
        {
            InitializeComponent();
        }

        private void DeveloperMenu_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Upload().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Update().ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Account_Details().ShowDialog();
        }
    }
}
