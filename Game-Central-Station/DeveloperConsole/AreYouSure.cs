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
    public partial class AreYouSure : Form
    {
        private Game game;

        public AreYouSure(Game game)
        {
            InitializeComponent();
        }

        private void AreYouSure_Load(object sender, EventArgs e)
        {
            label1.Text += game.displayName;


        }
    }
}
