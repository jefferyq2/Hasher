using CommunityToolkit.Mvvm.ComponentModel;
using HasherTest.HashClasses;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using CommunityToolkit.Mvvm.Input;
using HasherTest.Interfaces;
using System.Windows;
using Microsoft.Win32;

namespace HasherTest.ViewModels
{
    public partial class HashVerifyViewModel : ObservableObject
    {
        public HashVerifyViewModel()
        {
            HashType = HashFunction.Blake3MultiThreaded;
            Separator = '*';
            CommentChar = ';';
        }

        [ObservableProperty]
        private double overallProgress;

        [ObservableProperty]
        private double currentProgress;

        /// <summary>
        /// File name to be displayed for the currently processing file.
        /// </summary>
        [ObservableProperty]
        private string currentFileName;

        /// <summary>
        /// Files with their hash verification status.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FileStatus> fileStatuses = new ObservableCollection<FileStatus>();

        /// <summary>
        /// List of files obtained from the hash file.
        /// </summary>
        private ObservableCollection<FileData> Files = new ObservableCollection<FileData>();

        /// <summary>
        /// Separator used for separating the hash and filename in the hash file.
        /// </summary>
        [ObservableProperty]
        private char separator;

        /// <summary>
        /// Character with which any commented lines could begin in the hash files.
        /// </summary>
        [ObservableProperty] 
        private char commentChar;

        /// <summary>
        /// Type of hash to check against.
        /// </summary>
        [ObservableProperty]
        private HashFunction hashType;

        [ObservableProperty]
        private System.Windows.Media.Brush statusIconColor;

        [RelayCommand]
        public void VerifyHashes()
        {
            //FileStatuses = new ObservableCollection<FileStatus>();
            Files.Clear();
            FileStatuses.Clear();
            OverallProgress = 0;
            CurrentProgress = 0;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            //dialog.DefaultExt = ".txt"; // Default file extension
            //dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                Task.Run(async () =>
                {
                    await Verify(dialog.FileName, HashType);
                });
            }
        }

        public async Task Verify(string hashFilePath, HashFunction hashingAlgorithm)
        {
            GetDataFromHashFile(hashFilePath, hashingAlgorithm);

            double runningTotalOfFileSize = 0;
            double totalSizeOfFiles = Files.Sum(f => f.SizeInKBs);

            //go over the dictionary and perform stuff
            foreach (FileData file in Files)
            {

                CurrentFileName = file.Name;
                if(file.DoesFileExist() == false)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        file.Status = SymbolRegular.DismissCircle24;
                        FileStatuses.Add(new FileStatus(fileName: file.RelativePath, SymbolRegular.DismissCircle24, "File not found.") { StatusIconColor = System.Windows.Media.Brushes.Red });
                    });
                    continue;
                }
                string calculatedHash = Task<string>.Run(async () =>
                {
                    IHash hash_MD5 = new Hash_MD5();
                    hash_MD5.ProgressUpdater += UpdateProgress;
                    return hash_MD5.HashFile(file, GetBufferSize(file.SizeInMBs));
                    //return await CalculateBlake3MTHashForFile(file);
                }).Result;
                if (file.Hash == calculatedHash)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        file.Status = SymbolRegular.CheckmarkCircle24;
                        FileStatuses.Add(new FileStatus(file.RelativePath, SymbolRegular.CheckmarkCircle24, "OK") { StatusIconColor = System.Windows.Media.Brushes.Green } );
                        StatusIconColor = System.Windows.Media.Brushes.Green;
                    });
                else
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        file.Status = SymbolRegular.DismissCircle24;
                        FileStatuses.Add(new FileStatus(file.RelativePath, SymbolRegular.DismissCircle24, "Hash Mismatch") { StatusIconColor = System.Windows.Media.Brushes.Red });
                        StatusIconColor = System.Windows.Media.Brushes.Red;

                    });

                runningTotalOfFileSize += file.SizeInKBs;
                OverallProgress = (runningTotalOfFileSize / totalSizeOfFiles) * 100;

            }
        }

        public void UpdateProgress(object? sender,double progress)
        {
            CurrentProgress = progress;
        }

        private void GetDataFromHashFile(string hashFilePath, HashFunction hashingAlgorithm)
        {
            FileData hashFile = new FileData(hashFilePath);

            List<string> fileLines = new List<string>();
            try
            {
                //get the list of files without commented lines.
                fileLines = File.ReadLines(hashFilePath).Where(fi => fi[0] != CommentChar).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            foreach (string line in fileLines)
            {
                string[] fileAndHash = line.Split(Separator, 2, StringSplitOptions.TrimEntries);
                Files.Add(new FileData(hashFile.DirectoryPath + "\\" + fileAndHash[1]) { HashType = hashingAlgorithm, Hash = fileAndHash[0] });
            }
        }

        /// <summary>
        /// Returns the appropriate buffer size depending on the size of the file.
        /// </summary>
        /// <param name="fileSize">FileSize in MBs.</param>
        /// <returns>BufferSize in bytes.</returns>
        public int GetBufferSize(double fileSize)
        {
            if (fileSize <= 1)
            {
                return 8192 * 8;
            }
            else if (fileSize <= 101) //100mb
            {
                return 1024 * 1024 * 2;
            }
            else if (fileSize <= 512) //500mb
            {
                return 1024 * 512;
            }
            else if (fileSize <= 1024) //1GB
            {
                return 1024 * 1024 * 16;
            }
            else if (fileSize <= 1024 * 8) //8GB
            {
                return 1024 * 1024 * 2;
            }
            else
            {
                return 1024 * 1024 * 1 / 2;
            }
        }

        

    }
}
