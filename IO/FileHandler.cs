using System;
using System.IO;
using System.Collections.Generic;

namespace DFCommonLib.IO
{
    public interface IFileHandler
    {
        void SetFolder(string folderName);
        FileHandler.ErrorCode SaveFile(string filename, string fileData);
        byte[] ReadFile(string filename);
        string ReadBase64File(string filename);
        FileHandler.ErrorCode DeleteFile(string filename);
        string CreateDiskFilename(string filename);        
    }

    public class FileHandler : IFileHandler
    {
        public enum ErrorCode
        {
            OK = 0,
            FILE_TO_LARGE = 1,
            FAILED_TO_WRITE_FILE = 2,
            FILE_ALREADY_EXIST = 3,
            FILE_DOES_NOT_EXIST = 4,
            FAILED_TO_DELETE_FILE = 5,
        }

        private const uint MAX_FILE_SIZE = 10000000; // 10.000.000 bytes

        private string _folderName;

        public FileHandler()
        {
        }

        public void SetFolder(string folderName)
        {
            _folderName = folderName;
        }

        public ErrorCode SaveFile(string diskFilename, string base64data)
        {
            // Do not allow large files
            int len = base64data.Length;
            if ( len > MAX_FILE_SIZE || len <= 0 )
            {
                return ErrorCode.FILE_TO_LARGE;
            }

            // Convert data
            byte[] decodedBytes = Convert.FromBase64String (base64data);

            // Todo : Get path from config
            var currentFolder = Directory.GetCurrentDirectory();

            // Check if we have to create the folder
            Directory.CreateDirectory(_folderName);
            var fullFilePath = _folderName + "/" + diskFilename;

            if ( File.Exists(fullFilePath) )
            {
                return ErrorCode.FILE_ALREADY_EXIST;
            }

            // Try to write file
            File.WriteAllBytes(fullFilePath, decodedBytes );

            // Did we actually write file
            if ( !File.Exists(fullFilePath) )
            {
                return ErrorCode.FAILED_TO_WRITE_FILE;
            }

            return ErrorCode.OK;
        }

        public byte[] ReadFile(string filename)
        {
            var fullFilePath = _folderName + "/" + filename;
            return File.ReadAllBytes(fullFilePath);
        }

        public string ReadBase64File(string filename)
        {
            var data = ReadFile(filename);
            if ( data != null )
            {
                var base64data = Convert.ToBase64String(data);
                return base64data;
            }
            return null;
        }

        public ErrorCode DeleteFile(string filename)
        {
            var fullFilePath = _folderName + "/" + filename;

            if ( !File.Exists(fullFilePath) )
            {
                return ErrorCode.FILE_DOES_NOT_EXIST;
            }

            File.Delete(fullFilePath);

            if ( File.Exists(fullFilePath) )
            {
                return ErrorCode.FAILED_TO_DELETE_FILE;
            }

            return ErrorCode.OK;
        }

        public string CreateDiskFilename(string filename)
        {
            var ext = GetExtension(filename);
            string token = Guid.NewGuid().ToString();
            var diskFilename = token + "." + ext;
            return diskFilename;
        }

        private string GetExtension(string filename)
        {
            return "png";
        }
    }    
}