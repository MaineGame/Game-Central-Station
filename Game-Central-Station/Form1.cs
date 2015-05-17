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
        private Tab selecting = Tab.NOT_SET;
        private Tab selected = Tab.NOT_SET;

        private Game[] games = null;

        private MaterialSkinManager manager = MaterialSkinManager.Instance;

        public Mist()
        {
            Globals.root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            InitializeComponent();
            manager.AddFormToManage(this);
        }

        private void Mist_Load(object sender, EventArgs e)
        {
            materialLabel1.Text = "";
            WindowState = FormWindowState.Normal;
            if (Globals.hasArg("-K"))
            {
                WindowState = FormWindowState.Maximized;
                Sizable = false;
            }
            switchTabs(Tab.STORE);
        }

        private FlowLayoutPanel mainPanel = null;

        //called upon selecting the store. called from the worker completed thing.
        private void updateListings()
        {
            mainPanel = new FlowLayoutPanel();
            mainPanel.AutoScroll = true;
            mainPanel.FlowDirection = FlowDirection.LeftToRight;
            //mainPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Size = materialTabControl1.TabPages[materialTabControl1.TabIndex].Size;
            mainPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            foreach (Game game in games)
            {
                //Console.Beep();

                bool downloaded = File.Exists(Globals.root + "\\games\\" + game.id + "\\" + game.executableName);

                FlowLayoutPanel panel = new FlowLayoutPanel();
                panel.FlowDirection = FlowDirection.LeftToRight;
                panel.Size = new Size(400, 100);
                //panel.BorderStyle = BorderStyle.FixedSingle;
                //panel.BackColor = Color.Transparent; 

                FlowLayoutPanel textPanel = new FlowLayoutPanel();
                
                var textPanelMargin = textPanel.Margin;
                textPanel.MouseEnter += focusscroll;
                textPanelMargin.All = 0;
                textPanel.Margin = textPanelMargin;
                textPanel.Size = new Size(300, 100);
                textPanel.FlowDirection = FlowDirection.TopDown;

                MaterialLabel nameLabel = new MaterialLabel();
                
                nameLabel.Font = new Font(nameLabel.Font.FontFamily, 20, FontStyle.Regular);
                nameLabel.AutoSize = true;
                nameLabel.Text = game.displayName;
                var nameMargin = nameLabel.Margin;
                nameMargin.Top = 10;
                nameLabel.Margin = nameMargin;
                nameLabel.BackColor = Color.Transparent;

                MaterialLabel versionLabel = new MaterialLabel();

                versionLabel.Text = "version " + game.version;
                versionLabel.BackColor = Color.Transparent;

                try
                {
                    Image image = Image.FromStream(Globals.getFile("/games/" + game.id + "/default.jpg"));
                    image = ScaleImage(image, 300, 100);
                    textPanel.BackgroundImage = image;
                }
                catch (Exception e)
                {
                    try
                    {
                        System.Drawing.Image image = Image.FromStream(Globals.getFile("/games/" + game.id + "/default.png"));
                        image = ScaleImage(image, 300, 100);
                        textPanel.BackgroundImage = image;
                    }catch(Exception ex) {

                        textPanel.Controls.Add(nameLabel);
                        textPanel.Controls.Add(versionLabel);
                    }
                }

                

                FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
                buttonPanel.FlowDirection = FlowDirection.BottomUp;
                //buttonPanel.BorderStyle = BorderStyle.FixedSingle;
                buttonPanel.Size = new Size(100, 100);
                var buttonPanelMargin = buttonPanel.Margin;
                buttonPanelMargin.All = 0;
                buttonPanel.Margin = buttonPanelMargin;
                buttonPanel.MouseEnter += focusscroll;
                //buttonPanel.BorderStyle = BorderStyle.FixedSingle;
                Size buttonSize = new Size(93, 20);

                if (!downloaded)
                {
                    MaterialRaisedButton installButton = new MaterialRaisedButton();
                    installButton.ForeColor = Color.Azure;

                    installButton.Text = "Install";
                    installButton.Click += delegate(object sender, EventArgs e)
                    {
                        downloadGame(game);
                    };

                    installButton.Size = buttonSize;
                    //installButton.Dock = DockStyle.Right;
                    buttonPanel.Controls.Add(installButton);
                }
                else
                {
                    MaterialRaisedButton installButton = new MaterialRaisedButton();

                    installButton.Text = "Uninstall";
                    installButton.Click += delegate(object sender, EventArgs e)
                    {
                        uninstall(game);
                    };

                    installButton.Size = buttonSize;
                    //installButton.Dock = DockStyle.Right;
                    buttonPanel.Controls.Add(installButton);

                    MaterialRaisedButton playButton = new MaterialRaisedButton();
                    playButton.BackColor = Color.Gray;
                    playButton.Text = "Play";
                    playButton.Click += delegate(object sender, EventArgs e)
                    {
                        openGame(game);
                    };
                    playButton.Size = buttonSize;
                    buttonPanel.Controls.Add(playButton);
                }



                panel.Controls.Add(textPanel);
                panel.Controls.Add(buttonPanel);

                mainPanel.Controls.Add(panel);



            }

            materialTabControl1.TabPages[materialTabControl1.TabIndex].Controls.Clear();
            materialTabControl1.TabPages[materialTabControl1.TabIndex].Controls.Add(mainPanel);

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
            switchTabs(selected);
        }

        private void downloadGame(Game game)
        {
            new Download(game).ShowDialog();
            switchTabs(selected);
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

        private void switchTabs(Tab tab)
        {
            if (!backgroundWorker1.IsBusy)
            {
                selecting = tab;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //this happens directly after click, before any animation...
        private void materialTabControl1_Selected(object sender, TabControlEventArgs e)
        {
            switchTabs(Globals.convert(e.TabPage.Text));
        }

        //pretty simple method, thought there would be more to this. there wasn't
        //oh well, just in case there is later...
        private void loadStore()
        {
            games = Globals.getGamesWhere("archived = false and ready = true");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //primary objective of this method is to load the right games into the games array.

            //leaving this here to alert future me that thisis a BAD IDEA.
            //i know it seems like a good idea, but picture this:
            //you open up the store and just moments after a new game title is added.
            //in order ti get that title, you immediately refresh the page. but that
            //doesn't work as you are never actually contacting the server.
            //this should only ever happen if the array is not set to change.
            //and the only time that would make any sense is when you install a game and simply
            //need to reload the screen to enble the play button.
            if (selecting == selected)
            {
                //dont need to load anything new into our games array.
                //this is just a refreshing call.
                //return;
            }
            
            //if we want the store, i.e. parameter passed
            if (selecting == Tab.STORE)
            {
                loadStore();
            }
        }

        

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateListings();
            selected = selecting;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

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
            switchTabs(selected);
        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void focusscroll(object sender, EventArgs e)
        {
            if (mainPanel != null)
            {
                mainPanel.Focus();
            }
        }
    }
}
