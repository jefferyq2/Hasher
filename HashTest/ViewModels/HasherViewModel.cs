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
using Microsoft.Diagnostics.Tracing.Parsers;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;

namespace HasherTest.ViewModels
{
    public partial class HasherViewModel : ObservableObject
    {
        public HasherViewModel()
        {
            CurrentFileName = "CurrentFileName";
            HashType = HashFunction.Blake3;
            SeparatorChar = '*';
            CommentChar = ';';
        }

        [ObservableProperty]
        private Dictionary<string, int> bufferSizes = new Dictionary<string, int>() { 
            ["32kb"] = 1024 * 32, 
            ["64kb"] = 1024 * 64,
            ["128kb"] = 1024 * 128,
            ["256kb"] = 1024 * 256,
            ["512kb"] = 1024 * 512,
            ["1MB"] = 1024 * 1024 * 1,
            ["2MB"] = 1024 * 1024 * 2,
            ["4MB"] = 1024 * 1024 * 4,
            ["8MB"] = 1024 * 1024 * 8,
        };

        [ObservableProperty]
        private string bufferSize;

        #region ProgressBars

        /// <summary>
        /// Percentage value for overall progress bar.
        /// </summary>
        [ObservableProperty]
        private double overallProgress;

        /// <summary>
        /// Percentage value for current file progress bar.
        /// </summary>
        [ObservableProperty]
        private double currentProgress;

        /// <summary>
        /// File name to be displayed for the currently processing file.
        /// </summary>
        [ObservableProperty]
        private string currentFileName;

        #endregion

        /// <summary>
        /// List of files obtained from the hash file.
        /// </summary>
        private List<FileData> Files = new List<FileData>();

        #region Settings

        /// <summary>
        /// Separator used for separating the hash and filename in the hash file.
        /// </summary>
        [ObservableProperty]
        private char separatorChar;

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


        #endregion

        #region FileStatusExpander

        /// <summary>
        /// Files with their hash verification status.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FileStatus> fileStatuses = new ObservableCollection<FileStatus>();

        /// <summary>
        /// Displays total file count.
        /// </summary>
        [ObservableProperty]
        private long fileCount;

        /// <summary>
        /// Displays the success file count.
        /// </summary>
        [ObservableProperty]
        private long successFileCount;

        /// <summary>
        /// Displays the failure file count.
        /// </summary>
        [ObservableProperty]
        private long failureFileCount;

        #endregion

        [RelayCommand]
        public void VerifyHashes()
        {
            ResetUI();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Hashes (.md5)|*.md5;*.blake3;*.xxh3";

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                HashType = Helpers.HashHelper.IdentifyHashType(dialog.FileName);
                Task.Run(async () =>
                {
                    await Verify(dialog.FileName, HashType);
                });
            }
        }

        public async Task Verify(string hashFilePath, HashFunction hashingAlgorithm)
        {
            ReadHashesFromFile(hashFilePath, hashingAlgorithm);

            ResetFileCounter();

            double runningTotalOfFileSize = 0;
            double totalSizeOfFiles = Files.Sum(f => f.SizeInKBs);

            //go over the dictionary and perform stuff
            foreach (FileData file in Files)
            {
                CurrentFileName = file.Name;

                if (file.DoesFileExist() == false)
                {
                    UpdateFileStatus(file.RelativePath, SymbolRegular.DismissCircle24, "File not found");
                    continue;
                }

                string calculatedHash = await Task<string>.Run(() => { return GetHashForFile(file, hashingAlgorithm); });

                if (file.Hash == calculatedHash)
                    UpdateFileStatus(file.RelativePath, SymbolRegular.CheckmarkCircle24, "OK");
                else
                    UpdateFileStatus(file.RelativePath, SymbolRegular.DismissCircle24, "Hash Mismatch");


                runningTotalOfFileSize += file.SizeInKBs;
                OverallProgress = (runningTotalOfFileSize / totalSizeOfFiles) * 100;

            }
        }

        private void ResetUI()
        {
            Files.Clear();
            FileStatuses.Clear();
            OverallProgress = 0;
            CurrentProgress = 0;
        }

        [RelayCommand]
        public void CreateHashes()
        {

            //TODO:Implement dropdowns for buffersize and hashtype.

            ResetUI();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            //dialog.Filter = "Hashes (.md5)|*.md5;*.blake3;*.xxh3";

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                //HashType = Helpers.HashHelper.IdentifyHashType(dialog.FileName);
                Task.Run(async () =>
                {
                    await Create(dialog.FileNames, HashType);
                });
            }
        }

        public async Task Create(string[] filePaths, HashFunction hashFunction)
        {
            foreach (var file in filePaths)
            {
                FileData fileData = new FileData(file) { HashType = hashFunction };
                if (fileData.DoesFileExist() == false) continue;
                Files.Add(fileData);
            }

            ResetFileCounter();

            double runningTotalOfFileSize = 0;
            double totalSizeOfFiles = Files.Sum(f => f.SizeInKBs);

            //go over the dictionary and perform stuff
            foreach (FileData file in Files)
            {
                CurrentFileName = file.Name;

                string calculatedHash = string.Empty;

                try
                {
                    calculatedHash = await Task<string>.Run(() => { return GetHashForFile(file, hashFunction); });
                }
                catch
                { }

                if (!string.IsNullOrEmpty(calculatedHash))
                    UpdateFileStatus(file.RelativePath, SymbolRegular.CheckmarkCircle24, "Hash created");
                else
                    UpdateFileStatus(file.RelativePath, SymbolRegular.DismissCircle24, "Could not create hash");
                
                file.Hash = calculatedHash;


                runningTotalOfFileSize += file.SizeInKBs;
                OverallProgress = (runningTotalOfFileSize / totalSizeOfFiles) * 100;
            }

            WriteHashesToFile(hashFunction);

        }

        #region Helpers

        private void GetListOfFilesInDirectory(string directory)
        {
            string DirectoryPath = string.Empty;
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory);

                foreach (string file in files)
                    Files.Add(new FileData(file, DirectoryPath));

                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    GetListOfFilesInDirectory(subdirectory);
                }
            }
        }


        /// <summary>
        /// Adds the file to the filestatus list 
        /// and handles the running file counts.
        /// </summary>
        /// <param name="fileName">Name of the file to add.</param>
        /// <param name="statusIcon">Status icon to display for file.</param>
        /// <param name="statusMessage">Status message to display for file.</param>
        private void UpdateFileStatus(string fileName, SymbolRegular statusIcon, string statusMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FileStatuses.Add(new FileStatus(fileName: fileName, statusIcon, statusMessage));

                if (statusIcon == SymbolRegular.CheckmarkCircle24)
                    SuccessFileCount++;
                else
                    FailureFileCount++;
            });
        }

        /// <summary>
        /// Resets the file counter values.
        /// </summary>
        private void ResetFileCounter()
        {
            FileCount = Files.Count;
            SuccessFileCount = 0;
            FailureFileCount = 0;
        }

        /// <summary>
        /// Gets the hash for the file in the form of string for the given file
        /// using the given hashing algorithm.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="hashFunction"></param>
        /// <returns></returns>
        public string GetHashForFile(FileData file, HashFunction hashFunction)
        {
            IHash hash = Helpers.HashHelper.getHashType(hashFunction)!;
            hash.ProgressUpdater += UpdateProgress;
            return hash.HashFile(file, Helpers.HashHelper.GetBufferSize(file.SizeInMBs));
        }

        /// <summary>
        /// Method to update the progress for current file as progress received
        /// from the hash function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progress">Progress received from Hash function.</param>
        private void UpdateProgress(object? sender, double progress)
        {
            CurrentProgress = progress;
        }

        private void ReadHashesFromFile(string hashFilePath, HashFunction hashingAlgorithm)
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
                string[] fileAndHash = line.Split(SeparatorChar, 2, StringSplitOptions.TrimEntries);
                if (fileAndHash.Length < 2) continue;
                Files.Add(new FileData(hashFile.DirectoryPath + "\\" + fileAndHash[1]) { HashType = hashingAlgorithm, Hash = fileAndHash[0] });
            }
        }

        public void WriteHashesToFile(HashFunction hashFunction)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Blake3 Hash file (*.blake3)|*.blake3";
            saveFileDialog.Filter = Helpers.HashHelper.GetFileExtensionFilter(hashFunction);
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, Files.Where(file => file.Hash != null).Select(file => file.Hash + "\t" + "*" + file.RelativePath).ToArray());
            }
        }

        #endregion
    }
}
