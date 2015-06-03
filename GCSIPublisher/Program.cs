using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCSIPublisher
{
    static class Program
    {

        static void Main()
        {
            new Thing();
            Console.WriteLine("Finished!");
            Console.ReadLine();
        }

        
    }

    public class Thing
    {

        private const string ftpIP = "169.244.195.143/Installer";
        private const string username = "GCSUser";
        //link to ftp://GCSUser:@169.244.195.143/Installer/GCSInstaller.exe
        //to download latest version.
        /* to be used later, when im less stupid.
        private const string ftpIP = "169.244.195.143/Installer";
        private const string username = "GCSUser";
        */

        public Thing()
        {
            upload(getResource("GCSIPublisher.PublishContent.version.txt"), "version.txt");
            upload(getResource("GCSIPublisher.PublishContent.GCSInstaller.exe"), "GCSInstaller.exe");
        }

        private Stream getResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceStream(resourceName);
        }

        private void upload(Stream stream, string name)
        {

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpIP + "/" + name);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, "");

            // Copy the contents of the file to the request stream.
            byte[] fileContents = readToEnd(stream);
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

            response.Close();

        }

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
