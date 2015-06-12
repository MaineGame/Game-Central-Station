﻿using MaterialSkin;
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

namespace GameCentralStation
{
    public partial class Mist : MaterialForm
    {

        private Game[] storeGames = null;
        private List<Control> gameCardList = new List<Control>();
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
#if DEBUG
            Text += " [Debug Mode]";
            if (Globals.kioskMode)
                Text += " [Kiosk Mode]";
#endif

            if (Globals.kioskMode)
            {
                materialTabControl1.Height += materialTabSelector1.Height;
                var location = materialTabControl1.Location;
                location.Y -= materialTabSelector1.Height;
                materialTabControl1.Location = location;
                materialTabSelector1.Visible = false;
                materialTabSelector1.Enabled = false;

            }

            newBackgroundWorker();
            string version = File.ReadAllText(Globals.root + "\\version.txt");
            try
            {
                label3.Text += (Int32.Parse(version.Substring(0, 2)) - 10) + "." + (Int32.Parse(version.Substring(2, 2))) + "." + (Int32.Parse(version.Substring(4, 2))) + " Revision " + (Int32.Parse(version.Substring(6, 2))) + ".";
            }
            catch (Exception ex)
            {
                label3.Text = "Version Parsing Error: " + ex.Message;
            }
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
                GameCard gameCard = new GameCard();
                gameCard.setGame(game);


                gameCardList.Add(gameCard);
            }

            //Debug.log("Done creating cards");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            switch (task)
            {
                case STORE:
                    {
                        //ebug.log("RELOADING STORE HARD");

                        reloadStoreGamesList();
                    }
                    break;
                case LIBRARY:
                    break;
                case STORE_HARD:
                    {
                        //Debug.log("RELOADING STORE HARD");

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
                    foreach (Control card in gameCardList) flowLayoutPanel1.Controls.Add(card);
                    //Debug.log("done loading in cards");
                    break;
            }

        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void materialTabControl1_Selected(object sender, TabControlEventArgs e)
        {
            //Debug.log("Selected...");
            runTask(e.TabPageIndex);
        }

        private void Mist_KeyUp(object sender, KeyEventArgs e)
        {
            

        }
    }
}
