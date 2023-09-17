using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HasherTest.HashClasses;
using Microsoft.Win32;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HasherTest.Interfaces;
using System.Collections.ObjectModel;

namespace HasherTest.ViewModels
{
    public partial class HashCreateViewModel : ObservableObject
    {
        public HashCreateViewModel()
        {

        }

        [ObservableProperty]
        private double overallProgress;

        [ObservableProperty]
        private double currentProgress;

        [ObservableProperty]
        private string currentFileName = "Current Progress";

        [ObservableProperty]
        private List<FileData> files = new List<FileData>();

        [ObservableProperty]
        private string fileNames = string.Empty;

        /// <summary>
        /// Files with their hash verification status.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FileStatus> fileStatuses = new ObservableCollection<FileStatus>();


        private List<string> fileHashWithNames = new List<string>();

        private string DirectoryPath = string.Empty;

        [RelayCommand]
        private void GetFileNames()
        {
            fileHashWithNames.Clear();
            FileNames = string.Empty;
            CurrentFileName = "Current Progress";
            Files.Clear();

            //DirectoryPath = "C:\\PS2";
            //GetListOfFilesInDirectory(DirectoryPath);
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                Task.Run(async () =>
                {
                    await CreateHashForListOfFiles(dialog.FileNames, "", HashFunction.Blake3MultiThreaded);
                });
            }
        }

        private bool SaveHashesToFile(string fileName)
        {
            if (File.Exists(fileName)) File.Delete(fileName);

            File.WriteAllText(fileName, FileNames);
            //var str = File.ReadAllText(fileName);
            return true;
        }

        private async Task CreateHashForListOfFiles(string[] filePaths, string extension, HashFunction hashingAlgorithm)
        {
            double counter = 0;
            foreach (string filePath in filePaths)
                Files.Add(new FileData(filePath));

            foreach(FileData file in Files)
            {
                //hashingAlgorithm = HashFunction.Blake3MultiThreaded;

                string hash = CreateHashForFile(file, hashingAlgorithm);


                fileHashWithNames.Add(hash + "\t" + file.RelativePath);
                FileNames += fileHashWithNames[^1] + "\n";

                Double totalSize = Files.Sum(f => f.SizeInKBs);
                counter += file.SizeInKBs;
                //OverallProgress = ((double)counter / filePaths.Length) * 100;
                OverallProgress = ((double)counter / totalSize) * 100;

            }

            SaveHashesToFile(Files[0].DirectoryPath + "\\" + "Hash.blake3");
        }

        private string CreateHashForFile(FileData fileData, HashFunction hashingAlgorithm)
        {
            CurrentFileName = "Current Progress = " + fileData.RelativePath;
            if (fileData.DoesFileExist() == false) return "";
            switch (hashingAlgorithm)
            {
                case HashFunction.MD5:
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateMD5HashForFile(fileData);
                        }).Result;
                    }
                case HashFunction.SHA256:
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateSHA256HashForFile(fileData);
                        }).Result;
                    }
                case HashFunction.Blake2b:
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateBlake2bHashForFile(fileData);
                        }).Result;
                    }
                case HashFunction.Blake3:
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateBlake3HashForFile(fileData);
                        }).Result;
                    }
                case HashFunction.Blake3MultiThreaded:
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateBlake3MTHashForFile(fileData);
                        }).Result;
                    }
            }
            return "";
        }

        private void GetListOfFilesInDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory);

                foreach (string file in files)
                    Files.Add(new FileData(file,DirectoryPath));

                string[] subdirectories = Directory.GetDirectories(directory);
                foreach(string subdirectory in subdirectories)
                {
                    GetListOfFilesInDirectory(subdirectory);
                }
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


        public Task<string> CalculateMD5HashForFile(FileData file)
        {
            IDigest md5Digest = new MD5Digest();
            using (var fileStream = File.OpenRead(file.Path))
            using (var digestStream = new DigestStream(fileStream, md5Digest, null))
            {

                byte[] buffer = new byte[GetBufferSize(file.SizeInMBs)];
                long bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[md5Digest.GetDigestSize()];
            md5Digest.DoFinal(hashBytes, 0);

            return Task.FromResult(BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
        }

        public Task<string> CalculateSHA256HashForFile(FileData file)
        {
            IDigest sha256Digest = new Sha256Digest();

            using (var fileStream = File.OpenRead(file.Path))
            using (var digestStream = new DigestStream(fileStream, sha256Digest, null))
            {
                byte[] buffer = new byte[GetBufferSize(file.SizeInMBs)];
                int bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[sha256Digest.GetDigestSize()];
            sha256Digest.DoFinal(hashBytes, 0);
            return Task.FromResult(BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
        }

        public Task<string> CalculateBlake2bHashForFile(FileData file)
        {
            IDigest blake2bDigest = new Blake2bDigest(256);

            using (var fileStream = File.OpenRead(file.Path))
            using (var digestStream = new DigestStream(fileStream, blake2bDigest, null))
            {
                byte[] buffer = new byte[GetBufferSize(file.SizeInMBs)];
                int bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[blake2bDigest.GetDigestSize()];
            blake2bDigest.DoFinal(hashBytes, 0);
            return Task.FromResult(BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
        }

        public Task<string> CalculateBlake3HashForFile(FileData file)
        {
            using Blake3.Hasher blake3 = Blake3.Hasher.New();
            using FileStream fileStream = File.OpenRead(file.Path);
            byte[] buffer = new byte[GetBufferSize(file.SizeInMBs)];
            int bytesRead;
            do
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                blake3.Update(buffer.AsSpan(0, bytesRead));
            } while (bytesRead > 0);

            return Task.FromResult(blake3.Finalize().ToString());
        }

        public Task<string> CalculateBlake3MTHashForFile(FileData file)
        {
            using Blake3.Hasher blake3 = Blake3.Hasher.New();
            using FileStream fileStream = File.OpenRead(file.Path);
            byte[] buffer = new byte[GetBufferSize(file.SizeInMBs)];
            int bytesRead;
            do
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                blake3.UpdateWithJoin(buffer.AsSpan(0, bytesRead));
                CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
            } while (bytesRead > 0);

            return Task.FromResult(blake3.Finalize().ToString());
        }
    }
}
