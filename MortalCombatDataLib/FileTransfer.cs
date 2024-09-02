/* 
 * Module: FileTransfer
 * Description: The operations to copy files from client
 *              to server and vice-versa
 * Author: Jauhar
 * ID: 21494299
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
    public class FileTransfer
    {
        public string FilePath { get; set; }

        public FileTransfer(string filePath)
        {
            FilePath = filePath; 
        }

        /* Method: FileToBytes
         * Description: Compress image data into bytes
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
         * result: imageData (bytes[])
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
    }
}
