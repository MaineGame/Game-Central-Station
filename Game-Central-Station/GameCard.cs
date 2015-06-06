using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GameCentralStation
{
    public partial class GameCard : UserControl
    {
        private Game game;
        private string name;
        private Image image;
        private const int DONE = -1;
        private bool installed;

        public GameCard()
        {
            InitializeComponent();
        }

        public void setGame(Game game)
        {
            this.game = game;
            hardReload();
        }

        private void UninstalledGameCard_Load(object sender, EventArgs e)
        {

        }

        public void hardReload()
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.log("Loading for card");
            name = game.displayName;
            Debug.log("name on card: " + name);

            try
            {
                Image image = Image.FromStream(Globals.getFile("/games/" + game.id + "/default.jpg"));
                image = ScaleImage(image, 300, 100);
                this.image = image;
            }
            catch (Exception ex)
            {
                try
                {
                    System.Drawing.Image image = Image.FromStream(Globals.getFile("/games/" + game.id + "/default.png"));
                    image = ScaleImage(image, 300, 100);
                    this.image = image;
                }
                catch (Exception exc)
                {
                    this.image = Image.FromStream(Globals.getFile("/games/default2.jpg"));
                }
            }



            installed = File.Exists(Globals.root + "\\games\\" + game.id + "\\" + game.executableName);

            backgroundWorker1.ReportProgress(DONE);

        }

        private static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Max(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == DONE)
            {
                materialLabel1.Text = name;
                pictureBox1.Image = image;
                if (installed)
                {
                    materialFlatButton2.Visible = false;
                    materialFlatButton1.Visible = true;
                    materialRaisedButton1.Visible = true;
                }
                else
                {
                    materialFlatButton1.Visible = false;
                    materialRaisedButton1.Visible = false;
                    materialFlatButton2.Visible = true;
                }
            }
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            new Uninstall(game).ShowDialog();
            hardReload();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!installed) new Download(game).ShowDialog();
            else playGame();
        }

        private void playGame()
        {
            Globals.openGame(game);
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            playGame();
        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            new Download(game).ShowDialog();
            hardReload();
        }
    }
}
