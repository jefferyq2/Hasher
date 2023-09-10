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

namespace HasherTest.ViewModels
{
    public partial class HashVerifyViewModel : ObservableObject
    {
        public HashVerifyViewModel()
        {
            FileStatuses.Add(new FileStatus { FileName = "Hey",Status= SymbolRegular.CheckmarkCircle24 }); 
            FileStatuses.Add(new FileStatus { FileName = "Hi", Status = SymbolRegular.DismissCircle24 });
        }

        [ObservableProperty]
        private double overallProgress;

        [ObservableProperty]
        private double currentProgress;

        [ObservableProperty]
        private string currentFileName = "Current Progress";

        [ObservableProperty]
        private ObservableCollection<FileStatus> fileStatuses = new ObservableCollection<FileStatus>();

        private Dictionary<string,string> FileHashes = new Dictionary<string,string>();

        private List<FileData> Files = new List<FileData>();

        [RelayCommand]
        public void VerifyHashes()
        {
            FileStatuses.Clear();

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = false;
            dialog.FileName = "Document"; // Default file name
                                          //dialog.DefaultExt = ".txt"; // Default file extension
                                          //dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                Task.Run(async () =>
                {
                    await Verify(dialog.FileName, "", HashFunction.Blake3MultiThreaded);
                });
            }
        }

        public async Task Verify(string filePath, string extension, HashFunction hashingAlgorithm)
        {
            FileData fileData = new FileData(filePath);
            var file = File.ReadLines(filePath).ToList();
            foreach(var f in file.Where(fi => fi[0] != '#'))
            {
                var fileAndHash = f.Split('\t');
                FileHashes.Add(fileAndHash[1], fileAndHash[0]);
            }

            foreach (var fileNames in FileHashes.Keys)
                Files.Add(new FileData(fileData.DirectoryPath + "\\" + fileNames));

            double counter = 0;
            Double totalSize = Files.Sum(f => f.SizeInKBs);

            //go over the dictionary and perform stuff
            foreach (var fh in FileHashes)
            {
                FileData fd = new FileData(fileData.DirectoryPath +"\\"+ fh.Key);
                var hash = Task<string>.Run(async () =>
                {
                    return await CalculateBlake3MTHashForFile(fd);
                }).Result;
                if (fh.Value == hash)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FileStatuses.Add(new FileStatus { FileName = fd.RelativePath, Status = SymbolRegular.CheckmarkCircle24 });
                    });
                else
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FileStatuses.Add(new FileStatus { FileName = fd.RelativePath, Status = SymbolRegular.DismissCircle24 });
                    });
                counter += fd.SizeInKBs;
                //OverallProgress = ((double)counter / filePaths.Length) * 100;
                OverallProgress = ((double)counter / totalSize) * 100;

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


        public class FileStatus
        {
            public required string FileName { get; set; }
            public SymbolRegular Status { get; set; }
        }
    }
}
