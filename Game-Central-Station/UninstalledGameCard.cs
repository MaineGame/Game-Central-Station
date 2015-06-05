using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCentralStation
{
    public partial class UninstalledGameCard : UserControl
    {
        private Game game;
        private string name;
        private Image image;
        private const int DONE = -1;

        public UninstalledGameCard()
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
            }
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            new Download(game).ShowDialog();
        }
    }
}
