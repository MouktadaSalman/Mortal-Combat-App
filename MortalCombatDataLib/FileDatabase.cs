/* 
 * Module: FileTransfer
 * Description: The operations to copy files from client
 *              to server and vice-versa
 * Author: Jauhar
 * ID: 21494299
 * Version: 1.0.0.2
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace MortalCombatDataLib
{
    public class FileDatabase
    {
        /* Class fields:
         * _files -> contains all the uploaded file byte data
         * _imageFormats -> all the acceptable image formats
         * Instance -> to all a single instance of the file database
         */
        private readonly List<FileData> _files;
        public static FileDatabase Instance { get; } = new FileDatabase();

        private readonly string[] _imageFormats =
        {
            "ai", "dwg","jpeg", "jpg", "png", "gif", "webp", "bmp", "svg", "eps", "raw", "tiff", "heif", "heic", "indd", "psd"
        };

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
            string[] path = filePath.Split('/');
            fName = path.Last();

            //Extract file format
            string[] format = fName.Split('.');
            fFormat = format.Last();

            //Check if its a text file
            if (fFormat == "txt")
            {
                //Add valid text file
                fType = 2;
                _files.Add(new FileData(fName, fFormat, fType, FileToBytes(filePath)));
            }

            //Check if file an accepted image
            foreach (string f in _imageFormats)
            {
                if (f == fFormat)
                {
                    //Add valid image data upload
                    fType = 1;
                    _files.Add(new FileData(fName, fFormat, fType, ImageToBytes(filePath)));
                    break;
                }
            }
        }

        /* Method: DownloadFile
         * Description: To allow download of files from database
         * Parameters: fileName (string)
         * Result:
         */
        public void DownloadFile(string fileName)
        {
            string downloadPath = @"";

            if (downloadPath != @"")
            {
                foreach (FileData f in _files)
                {
                    if (f.fileName == fileName)
                    {
                        if (f.fileType == 2)
                        {
                            File.WriteAllBytes(downloadPath, f.fileData);
                        }
                    }
                }
            }
        }
    }
}
