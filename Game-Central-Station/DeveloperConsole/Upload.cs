using MaterialSkin;
using MaterialSkin.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCentralStation.DeveloperConsole
{
    public partial class Upload : MaterialForm
    {
        public bool success = false;
        private int version = 1;
        private int idGroup = -1;

        public Upload()
        {
            InitializeComponent();
        }

        public Upload(Game template)
        {
            InitializeComponent();
            materialSingleLineTextField3.Text = template.name;
            version = template.versionInteger + 1;
            idGroup = template.idGroup;

        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Upload_Load(object sender, EventArgs e)
        {
            MaterialSkinManager.Instance.AddFormToManage(this);
            materialLabel5.Text = "";

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void browse1(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName))
            {
                materialSingleLineTextField1.Text = openFileDialog1.FileName;
            }
        }

        private void browse2(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            if (File.Exists(openFileDialog2.FileName))
            {
                materialSingleLineTextField2.Text = openFileDialog2.FileName;
            }
        }

        private void browse3(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                materialSingleLineTextField4.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        //like honestly methodize this please.
        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                materialFlatButton1.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //this method is pretty much copy pasta from msdn
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //check if database exists
            //TODO actually check later because updating is a thing.
            bool exists = false;
            if (!exists)
            {
                //try
                //{

                //variables for later methodization.
                string backgroundImagePath = materialSingleLineTextField1.Text;
                string executablePath = materialSingleLineTextField2.Text;
                string executableName = executablePath.Substring(executablePath.LastIndexOf("\\") + 1);
                string dataFolderPath = materialSingleLineTextField4.Text;
                string dataFolderName = dataFolderPath.Substring(dataFolderPath.LastIndexOf("\\") + 1);
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                //Le Toucan Has Arrived

                backgroundWorker1.ReportProgress(ZIP_CREATE);
                #region copy all file into the right places.

                //first copy everything in to a temp directory in root.
                if (!Directory.Exists(appdata + "\\GCS"))
                {
                    Directory.CreateDirectory(appdata + "\\GCS");
                }
                if (!Directory.Exists(appdata + "\\GCS\\temp"))
                    Directory.CreateDirectory(appdata + "\\GCS\\temp");
                if (File.Exists(appdata + "\\GCS\\temp\\default.jpg"))
                    File.Delete(appdata + "\\GCS\\temp\\default.jpg");
                if(backgroundImagePath != "")
                    File.Copy(backgroundImagePath, appdata + "\\GCS\\temp\\default.jpg");

                if (File.Exists(appdata + "\\GCS\\temp\\" + executableName))
                    File.Delete(appdata + "\\GCS\\temp\\" + executableName);
                File.Copy(executablePath, appdata + "\\GCS\\temp\\" + executableName);
                if (Directory.Exists(appdata + "\\GCS\\temp\\" + dataFolderName))
                    Directory.Delete(appdata + "\\GCS\\temp\\" + dataFolderName, true);
                DirectoryCopy(dataFolderPath, appdata + "\\GCS\\temp\\" + dataFolderName, true);

                //take everything and make it into a file /datzipdoe/
                if (File.Exists(appdata + "\\GCS\\current.zip"))
                    File.Delete(appdata + "\\GCS\\current.zip");
                ZipFile.CreateFromDirectory(appdata + "\\GCS\\temp\\", appdata + "\\GCS\\current.zip");

                #endregion

                //will be set in the next region if thegame is successfully added.
                int gameID = -1;

                #region tell the rds database that hey, this game exists man!

                MySqlCommand command = new MySqlCommand();
                command.Connection = DatabaseHelper.connection;

                //this thing man. tries to add the sql listing 999 times before it realizes there are no more game slots left. hopefully never going to happen?
                //jk no do dat no more. probably horrible practice but eh.
                {
                    bool done = false;
                    while (!done)
                    {
                        for (int i = 1; i < Int32.MaxValue && !done; i++)
                        {
                            try
                            {
                                long fileLength = new FileInfo(appdata + "\\GCS\\current.zip").Length;
                                command.CommandText = "INSERT INTO store VALUES(\"" +
                                    i + "\",\"" +
                                    materialSingleLineTextField3.Text + "\",\"" +
                                    version + "\",\"" +
                                    executableName + "\",\"" +
                                    fileLength + "\"," +
                                    "false, \"" +
                                    Globals.userName + "\"," +
                                    "false," +
                                    "current_timestamp()," +
                                    (idGroup == -1 ? i : idGroup) + "" +
                                ");";
                                command.ExecuteNonQuery();
                                
                                done = true;
                                gameID = i;
                            }
                            catch (Exception ex)
                            {
                                
                            }
                        }
                    }
                }

                //MessageBox.Show("gameID = " + gameID);

                #endregion

                //MessageBox.Show("We Told the database to hold you place while you upload.");

                #region create the ID Folder on ftp

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + Globals.FTPUser + ":" + Globals.password + "@" + Globals.FTPIP + "/games/" + gameID + "/");
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpRequest.GetResponse();

                //MessageBox.Show("Created the ID Folder on the FTP Server.");

                #endregion

                backgroundWorker1.ReportProgress(ZIP_UPLOAD);
                #region upload zip file to server

                //upload the zip to the ftp server

                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + Globals.FTPUser + ":" + Globals.password + "@" + Globals.FTPIP + "/games/" + gameID + "/current.zip");
                request.Method = WebRequestMethods.Ftp.UploadFile;

                //so like double authentication is doubly secure. logical.
                request.Credentials = new NetworkCredential(Globals.FTPUser, Globals.password);

                // Copy the contents of the file to the request stream.
                byte[] fileContents = File.ReadAllBytes(appdata + "\\GCS\\current.zip");
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                #endregion

                #region upload the image file
                Boolean isController = false;
                if(backgroundImagePath != "") {
                    isController = true;
                    Image image = Globals.createController();

                    image.Save(appdata + "\\GCS\\temp\\default.jpg");
                }

                FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create("ftp://" + Globals.FTPUser + ":" + Globals.password + "@" + Globals.FTPIP + "/games/" + gameID + (isController ? "/controller.png" : "/default.jpg"));
                request2.Method = WebRequestMethods.Ftp.UploadFile;

                //so like double authentication is doubly secure. logical.
                request2.Credentials = new NetworkCredential(Globals.FTPUser, Globals.password);

                // Copy the contents of the file to the request stream.
                fileContents = File.ReadAllBytes(appdata + "\\GCS\\temp\\default.jpg");
                request2.ContentLength = fileContents.Length;

                requestStream = request2.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                #endregion

                //finalizing in database
                #region tell rds the files are ready!

                //Le Toucan Has Departed
                command = new MySqlCommand();
                command.Connection = DatabaseHelper.connection;
                command.CommandText = "UPDATE store SET ready = true WHERE gameID = " + gameID;
                command.ExecuteNonQuery();

                #endregion

                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
            }
            else
            {
                //so this is an update.
            }
            MessageBox.Show("Upload Succesful!");
            backgroundWorker1.ReportProgress(DONESKI);
        }

        private const int ZIP_UPLOAD = -1;
        private const int ZIP_CREATE = -2;
        private const int DONESKI = -3;

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case ZIP_UPLOAD:

                    materialLabel5.Text = "Uploading zip to Game Central Station";
                    break;
                case ZIP_CREATE:

                    materialLabel5.Text = "Creating zip";
                    break;
                case DONESKI:
                    success = true;
                    Close();
                    break;

            }
        }
    }
}
