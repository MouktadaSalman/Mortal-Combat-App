﻿/* 
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
     * Instance -> to all a single instance of the file database
     */
        private readonly List<byte[]> _files;
        public static FileDatabase Instance { get; } = new FileDatabase();

        /* Constructor: FileDatabase
         * Description: The private constructor of the database
         * Parameters: none
         */
        private FileDatabase()
        {
            _files = new List<byte[]>();
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
            byte[] fileData = FileToBytes(filePath);


        }
    }
}
