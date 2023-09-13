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
            Path = path;
            SetFileAttributes(Path);
            if (RelativePath == "") RelativePath = Name;
        }

        public FileData(string path, string CurrentDirectory)
        {
            Path = path;
            RelativePath = Path.Substring(CurrentDirectory.Length);
            SetFileAttributes(Path);
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _Name = string.Empty;

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
        public double SizeInKBs
        {
            get { return _SizeInKBs; }
            set { _SizeInKBs = value; }
        }
        private double _SizeInKBs;

        public double SizeInMBs
        {
            get { return _SizeInMBs; }
            set { _SizeInMBs = value; }
        }
        private double _SizeInMBs;

        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }
        private string _Path = string.Empty;

        public string RelativePath
        {
            get { return _RelativePath; }
            set { _RelativePath = value; }
        }
        private string _RelativePath = string.Empty;

        public string? DirectoryPath
        {
            get { return _DirectoryPath; }
            set { _DirectoryPath = value; }
        }
        private string? _DirectoryPath = string.Empty;

        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set { _CreatedOn = value; }
        }
        private DateTime _CreatedOn;

        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }
        private string hash = string.Empty;

        public HashFunction HashType
        {
            get { return hashType; }
            set { hashType = value; }
        }
        private HashFunction hashType;

        public SymbolRegular Status
        {
            get { return status; }
            set { status = value; }
        }
        private SymbolRegular status = SymbolRegular.Empty;


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
            if (fileInfo.Exists)
            {
                Name = fileInfo.Name;
                Size = fileInfo.Length;
                CreatedOn = fileInfo.CreationTime;
                DirectoryPath = fileInfo.DirectoryName;
            }
        }
    }
}
