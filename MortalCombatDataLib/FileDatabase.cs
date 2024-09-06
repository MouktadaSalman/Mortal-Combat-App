/* 
 * Module: FileTransfer
 * Description: The operations to copy files from client
 *              to server and vice-versa
 * Author: Jauhar
 * ID: 21494299
 * Version: 1.0.1.1
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Versioning;


namespace MortalCombatDataLib
{
    public class FileDatabase
    {
        /* Class fields:
         * _files -> contains all the uploaded file byte data
         * Instance -> to all a single instance of the file database
         */
        private readonly List<FileData> _files;
        public static FileDatabase Instance { get; } = new FileDatabase();

        /* Constructor: FileDatabase
         * Description: The private constructor of the database
         * Parameters: none
         */
        private FileDatabase()
        {
            _files = new List<FileData>();
        }

        /* Method: FileToBytes
         * Description: Compress text data into bytes
         * Parameters: filePath (string)
         * result: fileData (bytes[])
         */
        private byte[] FileToBytes(string filePath)
        {
            byte[] fileData = null;

            using (FileStream fs = File.OpenRead(filePath))
            {
                var binaryRead = new BinaryReader(fs);
                fileData = binaryRead.ReadBytes((int)fs.Length);
            }

            return fileData;
        }

        /* Method: ImageToBytes
         * Description: Compress image data into bytes
         * Parameters: imagePath (string)
         * Result: imageData (bytes[])
         */
        private byte[] ImageToBytes(string imagePath)
        {
            byte[] imageData = null;

            Image image = Image.FromFile(imagePath);

            if (image != null)
            {
                ImageConverter _iConvert = new ImageConverter();

                var imageByte = (byte[])_iConvert.ConvertTo(image, typeof(byte[]));
            }

            return imageData;
        }

        /* Method: CheckFile
         * Description: Checks if the file already exists
         * Parameters: fileName (string)
         * Result: exist (bool)
         */
        private bool CheckFile(string fName)
        {
            //false by default
            bool exist = false;

            foreach(var f in _files)
            {
                if(f.fileName == fName)
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }

        /* Method: UploadFile
         * Description: Upload text files from clients
         * Parameters: imagePath (string)
         * Result: none
         */
        public void UploadFile(string filePath)
        {
            //Initialise variables
            string fName;
            string fFormat;
            int fType;
            
            //Extract file name
            string[] path = filePath.Split('\\');
            fName = path.Last();

            //Extract file format
            string[] format = fName.Split('.');
            fFormat = format.Last();

            if(CheckFile(fName))
            {
                //Check if its a text file
                if (fFormat == "txt")
                {
                    //Add valid text file
                    fType = 2;
                    _files.Add(new FileData(fName, fFormat, fType, FileToBytes(filePath)));
                }
                else
                {
                    //Add valid image data upload
                    fType = 1;
                    _files.Add(new FileData(fName, fFormat, fType, ImageToBytes(filePath)));
                }
            }
            else
            {
                Console.WriteLine("FileExistence:: File already available in database");
            }
        }

        /* Method: DownloadFile
         * Description: To allow download of files from database
         * Parameters: fileName (string)
         * Result:
         */
        public void DownloadFile(string fileName)
        {
            //Get the path to the downloads folder
            string downloadPath = @"" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"Downloads");

            if (downloadPath != @"")
            {
                string finalPath = Path.Combine(downloadPath, fileName);
                Console.WriteLine(downloadPath + " vs " + "C:\\Users\\mjauh\\Downloads");
                foreach (var f in _files)
                {
                    if (f.fileName == fileName)
                    {
                        //If it is an image
                        if (f.fileType == 1) 
                        {
                            ImageConverter converter = new ImageConverter();
                            Image x = (Bitmap)converter.ConvertFrom(f.fileData);
                            x.Save(finalPath);
                        }

                        //If it is a text file
                        if (f.fileType == 2)
                        {
                            File.WriteAllBytes(finalPath, f.fileData);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("DirectoryNotFound:: Failed to path towards the downloads folder");
            }
        }
    }
}
