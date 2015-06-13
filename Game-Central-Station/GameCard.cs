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
using System.Net;
using System.IO.Compression;

namespace GameCentralStation
{
    public partial class GameCard : UserControl
    {
        private Game game;
        private string name;
        private Image image;
        private const int DONE = -1;
        private const int STATE_CONNECTING = -2;
        private const int STATE_EXTRACTING = -3;
        private bool installed;
        private bool downloading;
        private int zipLength;

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
            progressBar1.Minimum = 0;
            progressBar1.Maximum = Int32.MaxValue;

            if (Globals.kioskMode)
            {
                
                double screenWidth = Screen.PrimaryScreen.Bounds.Width;
                double screenHeight = Screen.PrimaryScreen.Bounds.Height;
                double margin = 8;

                //do the resizing here...
                Width = (int)(Math.Floor((screenWidth - (margin * 4d)) / 3d));
                Height = (int)(Math.Floor((Width / 16d) * 9d)) + 72;
            }

        }

        public void hardReload()
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            name =
#if DEBUG
 game.id + " " +
#endif
 game.displayName;

            if (!Globals.offline)
            {
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
            }
            else
            {
                FileStream png = null;
                FileStream jpg = null;
                try
                {
                    jpg = new FileStream(Globals.root + "\\games\\" + game.id + "\\default.jpg", FileMode.Open);
                    Image image = Image.FromStream(jpg);
                    image = ScaleImage(image, 300, 100);
                    this.image = image;
                }
                catch (Exception ex)
                {
                    try
                    {
                        png = new FileStream(Globals.root + "\\games\\" + game.id + "\\default.png", FileMode.Open);
                        Image image = Image.FromStream(png);
                        image = ScaleImage(image, 300, 100);
                        this.image = image;
                    }
                    catch (Exception exc)
                    {
                        this.image = null;
                    }
                    finally
                    {
                        try
                        {
                            png.Close();
                        }
                        catch (Exception exce) { }
                    }
                }
                finally
                {
                    try
                    {
                        jpg.Close();
                    }
                    catch (Exception ex) { }
                }

            }



            if (!downloading)
            {
                //only thing dependant in downloading or not is snagging the downloaded state.
                installed = File.Exists(Globals.root + "\\games\\" + game.id + "\\" + game.executableName);
            }
            else
            {
                //just roll with false, even though it doesn't matter to use
                //downloading supercedes installed when reloading.
                installed = false;
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
                pictureBox1.Image = image;
                //remember kids, only render the buttons if we are not downloading presently...
                if (!downloading)
                {

                    materialLabel1.Text = name;
                    if (installed)
                    {
                        materialFlatButton2.Visible = false;
                        materialFlatButton1.Visible = true && !Globals.kioskMode;
                        materialRaisedButton1.Visible = true;
                    }
                    else
                    {
                        materialFlatButton1.Visible = false;
                        materialRaisedButton1.Visible = false;
                        materialFlatButton2.Visible = true;
                    }
                }
                else
                {
                    materialFlatButton1.Visible = false;
                    materialRaisedButton1.Visible = false;
                    materialFlatButton2.Visible = false;
                    //we're downloading... the other worker takes care of this one brosef.
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
            downloadWorker.RunWorkerAsync();

            hardReload();
        }

        private void downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            zipLength = Globals.getFtpFileSize("/games/" + game.id + "/current.zip");
            downloading = true;
            download();
            downloadWorker.ReportProgress(DONE);
        }

        private void download()
        {
            #region download and extracting the game data files

            //tell the ui we're trying to connect...
            downloadWorker.ReportProgress(STATE_CONNECTING);

            //establish a new webclient because downloading is a thing.
            WebClient client = new WebClient();

            //give the downloader a progress listener. doesn't yell at the ui as much as you think, but nonetheless,
            //it doesn't bother the ui thread, only raises flags for it. so the ui thread only actally recives the update
            //as much as it refreshes itself.
            client.DownloadProgressChanged += delegate(object Object, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
            {
                //we send it the number of bytes we done got so far
                if (downloadWorker.IsBusy) downloadWorker.ReportProgress((int)(downloadProgressChangedEventArgs.BytesReceived));

            };

            Directory.CreateDirectory(Globals.root + "\\games");
            Directory.CreateDirectory(Globals.root + "\\games\\" + game.id);

            if (Directory.Exists(Globals.root + "\\games\\" + game.id))
                //delete the old if you have one
                Directory.Delete(Globals.root + "\\games\\" + game.id, true);

            Directory.CreateDirectory(Globals.root + "\\games\\" + game.id);

            //download it
            try
            {
                client.Credentials = new NetworkCredential(Globals.FTPUser, Globals.password);
                client.DownloadFileTaskAsync(new Uri("ftp://" + Globals.FTPIP + "/games/" + game.id + "/current.zip"), "" + Globals.root + "\\games\\" + game.id + "\\temp.zip").Wait();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //okay, we good downloading, tell the ui we're extracting now
            downloadWorker.ReportProgress(STATE_EXTRACTING);


            //then you know, actually start that bit...
            ZipFile.ExtractToDirectory(Globals.root + "\\games\\" + game.id + "\\temp.zip", Globals.root + "\\games\\" + game.id);
            //File.Delete(Globals.root + "\\games\\temp.zip");

            #endregion
        }

        private void downloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == DONE)
            {
                // Debug.log("Done installing game?");
                downloading = false;
                progressBar1.Visible = false;
                hardReload();
            }
            else if (e.ProgressPercentage == STATE_CONNECTING)
            {
                progressBar1.Maximum = zipLength;
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Visible = true;
                materialLabel1.Text = "Downloading " +
#if DEBUG
 game.id + " " +
#endif
 game.displayName + "...";
                materialFlatButton1.Visible = false;
                materialRaisedButton1.Visible = false;
                materialFlatButton2.Visible = false;

            }
            else if (e.ProgressPercentage == STATE_EXTRACTING)
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                materialLabel1.Text = "Installing " + game.displayName +

#if DEBUG
 game.id + " " +
#endif
 "...";
                progressBar1.MarqueeAnimationSpeed = 20;
            }
            else
            {
                progressBar1.Value = e.ProgressPercentage;
            }
        }


    }
}
