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
        private int gameID;
        private string name;
        private Image image;

        public UninstalledGameCard()
        {
            InitializeComponent();
        }

        public void setGameID(int i)
        {
            gameID = i;
            //hardReload();
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
            lock (Mist.storeLock)
            {
                Game self = DatabaseHelper.getGamesWhere("gameID = " + gameID)[0];
                name = self.displayName;
                try
                {
                    Image image = Image.FromStream(Globals.getFile("/games/" + self.id + "/default.jpg"));
                    image = ScaleImage(image, 300, 100);
                    this.image = image;
                }
                catch (Exception ex)
                {
                    try
                    {
                        System.Drawing.Image image = Image.FromStream(Globals.getFile("/games/" + self.id + "/default.png"));
                        image = ScaleImage(image, 300, 100);
                        this.image = image;
                    }
                    catch (Exception exc)
                    {
                        this.image = Image.FromStream(Globals.getFile("/games/default2.jpg"));
                    }
                }

            }
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
    }
}
