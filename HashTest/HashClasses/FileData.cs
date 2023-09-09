using Hasher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Hasher.HashClasses
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
                SizeInKBs = Size/1024;
                SizeInMBs = Size/(1024*1024);
            }
        }
        private long _Size;

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

        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set { _CreatedOn = value; }
        }
        private DateTime _CreatedOn;

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
            }
        }
    }
}
