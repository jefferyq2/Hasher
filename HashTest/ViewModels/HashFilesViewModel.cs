using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hasher.HashClasses;
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

namespace Hasher.ViewModels
{
    public partial class HashFilesViewModel : ObservableObject
    {
        public HashFilesViewModel()
        {

        }

        [ObservableProperty]
        private double overallProgress;

        [ObservableProperty]
        private double currentProgress;

        [ObservableProperty]
        private string currentFileName = "Current Progress";

        [ObservableProperty]
        private string fileNames = string.Empty;

        private List<string> fileHashWithNames = new List<string>();

        [RelayCommand]
        private void GetFileNames()
        {
            fileHashWithNames.Clear();
            FileNames = string.Empty;
            CurrentFileName = "Current Progress";

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            dialog.FileName = "Document"; // Default file name
                                          //dialog.DefaultExt = ".txt"; // Default file extension
                                          //dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                Task.Run(async () =>
                {
                    await CreateHashForListOfFiles(dialog.FileNames, "", "Blake3MultiThreaded");
                });
                
            }
        }

        private async Task CreateHashForListOfFiles(string[] filePaths, string extension, string hashingAlgorithm)
        {
            int counter = 0;
            foreach (var filePath in filePaths)
            {
                //TODO : Append hash and filename to the .hash file.
                FileData fileData = new FileData(filePath);

                hashingAlgorithm = "Blake3MultiThreaded";

                string hash = CreateHashForFile(fileData, hashingAlgorithm);


                fileHashWithNames.Add(hash + "\t" + fileData.Name);
                FileNames += fileHashWithNames[^1] + "\n";
                counter++;
                OverallProgress = ((double)counter/filePaths.Length) * 100;
            }
        }

        private string CreateHashForFile(FileData fileData, string hashingAlgorithm)
        {
            CurrentFileName = "Current Progress = " + fileData.Name;
            switch (hashingAlgorithm)
            {
                case "MD5":
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateMD5HashForFile(fileData.Path);
                        }).Result;
                    }
                case "SHA256":
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateSHA256HashForFile(fileData.Path);
                        }).Result;
                    }
                case "Blake2b":
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateBlake2bHashForFile(fileData.Path);
                        }).Result;
                    }
                case "Blake3":
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateBlake3HashForFile(fileData.Path);
                        }).Result;
                    }
                case "Blake3MultiThreaded":
                    {
                        return Task<string>.Run(async () =>
                        {
                            return await CalculateBlake3MTHashForFile(fileData.Path);
                        }).Result;
                    }
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSize">Gets fileSize in MBs</param>
        /// <returns></returns>
        private int GetBufferSize(long fileSize)
        {
            if (fileSize < 1)
            {
                return 512 * 512;
            }
            else if (fileSize < 1024)
            {
                return 1024 * 1024;
            }
            else if (fileSize >= 1024)
            {
                return 1024 * 1024 * 8;
            }
            return 0;
        }


        public Task<string> CalculateMD5HashForFile(string filePath)
        {
            IDigest md5Digest = new MD5Digest();
            //MD5Progress = 0;
            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, md5Digest, null))
            {

                byte[] buffer = new byte[GetBufferSize(fileStream.Length / (1024 * 1024))]; // Adjust buffer size as needed
                long bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    //System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    MD5Progress = (double)fileStream.Position / fileStream.Length * 100;
                    //});
                    CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[md5Digest.GetDigestSize()];
            md5Digest.DoFinal(hashBytes, 0);

            return Task.FromResult(BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
        }

        public Task<string> CalculateSHA256HashForFile(string filePath)
        {
            IDigest sha256Digest = new Sha256Digest();

            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, sha256Digest, null))
            {
                byte[] buffer = new byte[GetBufferSize(fileStream.Length / (1024 * 1024))]; // Adjust buffer size as needed
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

        public Task<string> CalculateBlake2bHashForFile(string filePath)
        {
            IDigest blake2bDigest = new Blake2bDigest(256);

            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, blake2bDigest, null))
            {
                byte[] buffer = new byte[GetBufferSize(fileStream.Length / (1024 * 1024))]; // Adjust buffer size as needed
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

        public Task<string> CalculateBlake3HashForFile(string filePath)
        {
            using Blake3.Hasher blake3 = Blake3.Hasher.New();
            using FileStream fileStream = File.OpenRead(filePath);
            byte[] buffer = new byte[GetBufferSize(fileStream.Length / (1024 * 1024))]; // Adjust buffer size as needed
            int bytesRead;
            do
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                blake3.Update(buffer.AsSpan(0, bytesRead));
            } while (bytesRead > 0);

            return Task.FromResult(blake3.Finalize().ToString());
        }

        public Task<string> CalculateBlake3MTHashForFile(string filePath)
        {
            using Blake3.Hasher blake3 = Blake3.Hasher.New();
            using FileStream fileStream = File.OpenRead(filePath);
            byte[] buffer = new byte[GetBufferSize(fileStream.Length / (1024 * 1024))]; // Adjust buffer size as needed
            int bytesRead;
            do
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                blake3.UpdateWithJoin(buffer.AsSpan(0, bytesRead));
                CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
            } while (bytesRead > 0);

            return Task.FromResult(blake3.Finalize().ToString());
        }

        partial void OnOverallProgressChanged(double value)
        {
            //System.Diagnostics.Debug.WriteLine(OverallProgress.ToString());
        }
    }
}
