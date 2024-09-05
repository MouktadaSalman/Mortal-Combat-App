/* 
 * Module: File
 * Description: To hold the data of files
 * Author: Jauhar
 * ID: 21494299
 * Version: 1.0.0.1
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatDataLib
{
    public class FileData
    {
    /* Class Fields:
     * fileName -> name of file
     * fileType -> type of file (either image/text file)
     * fileData -> the byte data of the file
     */
        public string fileName { get; }

        public string fileFormat { get; }

        public int fileType { get; }

        public byte[] fileData { get; }
        
        /* Constructor: File
         * Description: The constructor a new file in the database
         * Parameters: fName (string), fType (int), fData (byte[])
         */
        public FileData(string fName, string fFormat, int fType, byte[] fData)
        {
            fileName = fName;
            fileFormat = fFormat;
            fileType = fType;
            fileData = fData;
        }
    }
}
