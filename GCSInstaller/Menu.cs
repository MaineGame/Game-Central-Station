using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCSInstaller
{
    public partial class Menu : Form
    {

        private static string installDir;

        public Menu()
        {
            installDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\The Maine Game\\GCS";
            Console.WriteLine("Appdata set to " + installDir);
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            if (Program.auto)
            {
                tabControl1.SelectedIndex = 2;
                backgroundWorker1.RunWorkerAsync();
                button5.Enabled = false;
                button6.Enabled = false;
            }
            else
            {
                checkBox1.Checked = true;
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "GCSInstaller.license.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    richTextBox1.Text = result;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex--;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex++;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "Install")
            {
                backgroundWorker1.RunWorkerAsync();
                button5.Enabled = false;
                button6.Enabled = false;
            }
            else
            {

                Close();
                if(checkBox1.Checked)
                    System.Diagnostics.Process.Start(installDir + "\\Game-Central-Station.exe");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex--;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //#420installit

            copyResource("GCSInstaller.InstallerContent.MaterialSkin.dll", installDir + "\\MaterialSkin.dll");
            backgroundWorker1.ReportProgress(1);
            copyResource("GCSInstaller.InstallerContent.MySql.Data.dll", installDir + "\\MySql.Data.dll");
            backgroundWorker1.ReportProgress(2);
            copyResource("GCSInstaller.InstallerContent.Game-Central-Station.exe", installDir + "\\Game-Central-Station.exe");
            backgroundWorker1.ReportProgress(3);
            copyResource("GCSInstaller.InstallerContent.version.txt", installDir + "\\version.txt");
            backgroundWorker1.ReportProgress(4);

            //TODO add config thing

            {
                string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                StreamWriter writer = new StreamWriter(deskDir + "\\Game Central Station.url");
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + installDir + "\\Game-Central-Station.exe");
                writer.Flush();
                writer.Close();
            }


            backgroundWorker1.ReportProgress(DONE);
        }


        private const int UPDATE_NAME = -2;
        private const int DONE = -1;

        private void copyResource(string resourceName, string newPath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine("Copying " + resourceName);
            Directory.CreateDirectory(installDir);
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                currentFile = "Copying " + resourceName;
                backgroundWorker1.ReportProgress(UPDATE_NAME);
                byte[] stuff = readStream(stream);
                FileStream file = new FileStream(newPath, FileMode.Create);
                file.Write(stuff, 0, stuff.Length);
                file.Close();
            }
            Console.WriteLine("Finsihed " + resourceName);
        }

        private string currentFile = "";

        //love copy pasta
        public static byte[] readStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == DONE)
            {
                button5.Enabled = true;
                button5.Text = "Finished";
                label5.Text = "Installation Complete!";
                if (Program.auto)
                {
                    Close();
                    if (checkBox1.Checked)
                        System.Diagnostics.Process.Start(installDir + "\\Game-Central-Station.exe", "-K");
                }
            }
            else if (e.ProgressPercentage == UPDATE_NAME)
            {
                label5.Text = currentFile;
            }
            else
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 4;
                progressBar1.PerformStep();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
