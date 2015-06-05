using MaterialSkin;
using MaterialSkin.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameCentralStation.DeveloperConsole;

namespace GameCentralStation
{
    public partial class Mist : MaterialForm
    {

        private Game[] storeGames = null;
        private List<UninstalledGameCard> gameCardList = new List<UninstalledGameCard>();
        private Game[] libraryGames = null;
        private const int STORE = 0;
        private const int LIBRARY = 1;
        private const int DOWNLOADS = 2;
        private const int ABOUT = 3;
        private const int STORE_HARD = -1;
        private const int LIBRARY_HARD = -2;
        private int task = 0;
        private BackgroundWorker backgroundWorker1;

        private MaterialSkinManager manager = MaterialSkinManager.Instance;

        public Mist()
        {
            Globals.root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            InitializeComponent();
            manager.AddFormToManage(this);
        }

        private void newBackgroundWorker()
        {
            backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
        }

        private void Mist_Load(object sender, EventArgs e)
        {
            newBackgroundWorker();
            string version = File.ReadAllText("version.txt");
            try
            {
                label3.Text += (Int32.Parse(version.Substring(0, 2)) - 10) + "." + (Int32.Parse(version.Substring(2, 2))) + "." + (Int32.Parse(version.Substring(4, 2))) + " Revision " + (Int32.Parse(version.Substring(6, 2))) + ".";
            }
            catch (Exception ex)
            {
                label3.Text = "Version Parsing Error: " + ex.Message;
            }
            materialFlatButton1.ForeColor = Color.White;
            BackColor = ((int)Primary.LightBlue800).ToColor();
            WindowState = FormWindowState.Normal;
            if (Globals.hasArg("-K"))
            {
                WindowState = FormWindowState.Maximized;
                Sizable = false;
            }
            runTask(STORE_HARD);
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

        private void uninstall(Game game)
        {
            new Uninstall(game).ShowDialog();
            runTask(STORE_HARD); //TODO SAME
        }

        private void downloadGame(Game game)
        {
            new Download(game).ShowDialog();
            runTask(STORE_HARD); //TODO make CURRENT PAGE
        }

        private void openGame(Game game)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = Globals.root + "\\games\\" + game.id + "\\" + game.executableName;
                process.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //300 x 169

        //major time comes from the downloading of images, so thats what needs to be optimized.
        //done through background workers in cards.
        private void reloadStoreGamesList()
        {
            storeGames = DatabaseHelper.getGamesWhere("archived = false and ready = true");
        }

        private void addGamesToStore()
        {
            gameCardList.Clear();
            foreach (Game game in storeGames)
            {
                if (backgroundWorker1.CancellationPending) return;
                UninstalledGameCard gameCard = new UninstalledGameCard();
                gameCard.setGame(game);


                gameCardList.Add(gameCard);
            }

            Debug.log("DONE LOADING THINGS INTO PANEL");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            switch (task)
            {
                case STORE:
                    {
                        Debug.log("RELOADING STORE HARD");

                        reloadStoreGamesList();
                    }
                    break;
                case LIBRARY:
                    break;
                case STORE_HARD:
                    {
                        Debug.log("RELOADING STORE HARD");

                        reloadStoreGamesList();
                    }
                    break;

            }

            backgroundWorker1.ReportProgress(DONE);

        }

        //TODO use a NEW background worker everytime
        private void runTask(int task)
        {
            this.task = task;
            if(backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
            newBackgroundWorker();
            backgroundWorker1.RunWorkerAsync();
        }

        private const int DONE = -1;

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //DONE?

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //TODO stuff
            switch (e.ProgressPercentage)
            {
                case DONE:
                    addGamesToStore();
                    flowLayoutPanel1.Controls.Clear();
                    foreach (UninstalledGameCard card in gameCardList) flowLayoutPanel1.Controls.Add(card);
                    Debug.log("DONE LOADING THINGS INTO PANEL");
                    break;
            }

        }

        private void advance()
        {
            if (Globals.userName != null)
            {
                new DeveloperMenu().ShowDialog();
            }
            else
            {
                //yeah this means you cancelled, not you failed.
                //failing is handled in the login window.
            }
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {

            new Login().ShowDialog();
            advance();

        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void materialTabControl1_Selected(object sender, TabControlEventArgs e)
        {
            Debug.log("Selected...");
            runTask(e.TabPageIndex);
        }
    }
}
