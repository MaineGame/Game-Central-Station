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
        private Game[] libraryGames = null;
        private const int STORE = 0;
        private const int LIBRARY = 1;
        private const int ABOUT = 2;
        private const int STORE_HARD = 3;
        private const int LIBRARY_HARD = 4;
        private int task = 0;

        private MaterialSkinManager manager = MaterialSkinManager.Instance;

        public Mist()
        {
            Globals.root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            InitializeComponent();
            manager.AddFormToManage(this);
        }

        private void Mist_Load(object sender, EventArgs e)
        {
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
            BackColor = ((int)Primary.LightBlue700).ToColor();
            WindowState = FormWindowState.Normal;
            if (Globals.hasArg("-K"))
            {
                WindowState = FormWindowState.Maximized;
                Sizable = false;
            }
            hardReloadStorePage();
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
            hardReloadStorePage(); //TODO SAME
        }

        private void downloadGame(Game game)
        {
            new Download(game).ShowDialog();
            hardReloadStorePage(); //TODO make CURRENT PAGE
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

        private void reloadStoreGamesList()
        {
            storeGames = Globals.getGamesWhere("archived = false and ready = true");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            switch (task)
            {
                case STORE:
                    break;
                case LIBRARY:
                    break;
                case STORE_HARD:
                    {
                        reloadStoreGamesList();



                    }
                    break;

            }

        }

        private void reloadStorePage()
        {
            task = STORE;
            backgroundWorker1.RunWorkerAsync();
        }

        private void hardReloadStorePage()
        {
            task = STORE_HARD;
            backgroundWorker1.RunWorkerAsync();
        }

        private void reloadLibraryPage()
        {
            task = LIBRARY;
            backgroundWorker1.RunWorkerAsync();
        }

        private void hardReloadLibraryPage()
        {
            task = LIBRARY_HARD;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO stuff
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //TODO stuff
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
            
        }
    }
}
