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

namespace GameCentralStation
{
    public partial class Connect : MaterialForm
    {

        private static string checkingUpdates = "Checking for Updates";
        private static string connectingToDatabase = "Connecting to Game Central Station";
        private const string ftpIP = "169.244.195.143/Installer";
        private const string username = "GCSUser";
        //link to ftp://GCSUser:@169.244.195.143/Installer/GCSInstaller.exe
        //to download latest version.
        /* to be used later, when im less stupid.
        private const string ftpIP = "169.244.195.143/Installer";
        private const string username = "GCSUser";
        */
        private const string password = "";
        private const string versionFile = "version.txt";
        private int localVersion = -1;
        private int ftpVersion = -1;

        public Connect()
        {
            InitializeComponent();
            MaterialSkinManager manager = MaterialSkinManager.Instance;
            manager.AddFormToManage(this);
        }

        private void Connect_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 20;
            backgroundWorker1.RunWorkerAsync();
        }

        private void connect() {
            try
            {
                Console.WriteLine("Connecting...");
                DatabaseHelper.connection = new MySqlConnection(
                    "Server=" + DatabaseHelper.RDSDOMAIN + ";" +
                    "Database=mainegamesteam;" +
                    "Uid=mainegamesteam;" +
                    "Pwd=mainegamesteam1!;"
                    );
                DatabaseHelper.connection.Open();
                Console.WriteLine("wuddup!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                DatabaseHelper.connection = null;
            }
        }

        private const int DONE = -1;
        private const int CONNECTING = -2;
        private const int CHECKING_UPDATES = -3;

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void checkUpdates()
        {

            try
            {
                Thread.Sleep(1000);

                #region get version from server
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpIP + "/" + versionFile);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    request.Credentials = new NetworkCredential(username, password);

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    Int32.TryParse(reader.ReadToEnd(), out ftpVersion);


                    Console.WriteLine("Download Complete, status {0}", response.StatusDescription);

                    reader.Close();
                    response.Close();
                }
                #endregion

                #region get version from local

                Int32.TryParse(Encoding.ASCII.GetString(File.ReadAllBytes(AssemblyDirectory + "\\version.txt")), out localVersion);

                #endregion


                if (ftpVersion > localVersion)
                {

                    MessageBox.Show("There is a newer version of GCS available.\nPress okay to download it now.");


                    #region download the installer and reopen it
                    {
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpIP + "/GCSInstaller.exe");
                        request.Method = WebRequestMethods.Ftp.DownloadFile;

                        // This example assumes the FTP site uses anonymous logon.
                        request.Credentials = new NetworkCredential(username, password);

                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                        Stream responseStream = response.GetResponseStream();

                        FileStream fileStream = new FileStream(AssemblyDirectory + "\\GCSInstaller.exe", FileMode.Create);
                        byte[] file = readToEnd(responseStream);
                        fileStream.Write(file, 0, file.Length);

                        Console.WriteLine("Download Complete, status {0}", response.StatusDescription);

                        response.Close();
                        fileStream.Close();
                    }

                    Process.Start(AssemblyDirectory + "\\GCSInstaller.exe");
                    Application.Exit();
                    #endregion
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(CHECKING_UPDATES);
            checkUpdates();
            backgroundWorker1.ReportProgress(CONNECTING);
            connect();
            backgroundWorker1.ReportProgress(DONE);

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case DONE:
                    Close();
                    break;
                case CHECKING_UPDATES:
                    Text = checkingUpdates;
                    break;
                case CONNECTING:
                    Text = connectingToDatabase;
                    break;
            }

        }

        //copy pasta
        public static byte[] readToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
