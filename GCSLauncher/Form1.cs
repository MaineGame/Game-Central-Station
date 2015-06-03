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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCSLauncher
{
    public partial class Form1 : Form
    {

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

        //copy pasta. i can do this with variables? cool. remember that.
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

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            backgroundWorker1.RunWorkerAsync();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

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

                
                if (ftpVersion <= localVersion)
                {
                    #region open up GCS

                    backgroundWorker1.ReportProgress(-1);
                    Process process = new Process();
                    process.StartInfo.FileName = AssemblyDirectory + "\\Game-Central-Station.exe";
                    process.Start();

                    #endregion
                }
                else
                {

                    MessageBox.Show("There is a newer version of GCS available. Press okay to download it now.");

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

                    backgroundWorker1.ReportProgress(-1);
                    Process.Start(AssemblyDirectory + "\\GCSInstaller.exe");
                    #endregion
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Close();
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
