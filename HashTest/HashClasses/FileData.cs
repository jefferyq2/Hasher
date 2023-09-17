using HasherTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace HasherTest.HashClasses
{
    public class FileData : IFile
    {
        public FileData(string path)
        {
            SetFileAttributes(path);
            if (string.IsNullOrEmpty(RelativePath)) RelativePath = Name;
        }

        public FileData(string path, string CurrentDirectory)
        {
            Path = path;
            RelativePath = Path.Substring(CurrentDirectory.Length);
            SetFileAttributes(Path);
        }

        /// <summary>
        /// Name of the file.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _Name = string.Empty;

        /// <summary>
        /// Size of the file in bytes.
        /// </summary>
        public long Size
        {
            get { return _Size; }
            set 
            { 
                _Size = value; 
                SizeInKBs = (double)Size/1024;
                SizeInMBs = (double)Size/(1024*1024);
            }
        }
        private long _Size;

        /// <summary>
        /// Size of the file in Kilobytes.
        /// </summary>
        public double SizeInKBs
        {
            get { return _SizeInKBs; }
            set { _SizeInKBs = value; }
        }
        private double _SizeInKBs;

        /// <summary>
        /// Size of the file in Megabytes.
        /// </summary>
        public double SizeInMBs
        {
            get { return _SizeInMBs; }
            set { _SizeInMBs = value; }
        }
        private double _SizeInMBs;

        /// <summary>
        /// Absolute path of the file.
        /// </summary>
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }
        private string _Path = string.Empty;

        /// <summary>
        /// Relative path of the file wrt current directory.
        /// </summary>
        public string RelativePath
        {
            get { return _RelativePath; }
            set { _RelativePath = value; }
        }
        private string _RelativePath = string.Empty;

        /// <summary>
        /// Path of the parent directory.
        /// </summary>
        public string? DirectoryPath
        {
            get { return _DirectoryPath; }
            set { _DirectoryPath = value; }
        }
        private string? _DirectoryPath = string.Empty;

        /// <summary>
        /// Date on which the file was created.
        /// </summary>
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set { _CreatedOn = value; }
        }
        private DateTime _CreatedOn;

        /// <summary>
        /// Hash of the file.
        /// </summary>
        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }
        private string hash = string.Empty;

        /// <summary>
        /// Type of hash for the file.
        /// </summary>
        public HashFunction HashType
        {
            get { return hashType; }
            set { hashType = value; }
        }
        private HashFunction hashType;

        /// <summary>
        /// Status of file.
        /// </summary>
        public SymbolRegular Status
        {
            get { return status; }
            set { status = value; }
        }
        private SymbolRegular status = SymbolRegular.Empty;

        /// <summary>
        /// Checks for file existence.
        /// </summary>
        /// <returns><see langword="true"/> if file exists, <see langword="false"/> otherwise</returns>
        public bool DoesFileExist()
        {
            FileInfo fileInfo = new FileInfo(Path);
            if (fileInfo.Exists)
                return true;
            return false;
        }

        private void SetFileAttributes(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            Name = fileInfo.Name;
            DirectoryPath = fileInfo.DirectoryName;
            Path = fileInfo.FullName;
            if (fileInfo.Exists)
            {
                Size = fileInfo.Length;
                CreatedOn = fileInfo.CreationTime;
            }
            else
            {

            }
        }

    }
}
