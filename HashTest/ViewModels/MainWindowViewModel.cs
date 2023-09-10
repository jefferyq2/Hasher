using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle;
using System.IO;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto;
using System.Diagnostics;
using Blake3;
using System.Data;

namespace HasherTest.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {

        [ObservableProperty]
        private string fileName = string.Empty;

        [ObservableProperty]
        private double fileSize = 0;

        [ObservableProperty]
        private int bufferSizeInKBs = 0;

        [ObservableProperty]
        private int bufferSize = 0;

        [ObservableProperty]
        private string md5HashValue = string.Empty;

        [ObservableProperty]
        private string blake3HashValue = string.Empty;

        [ObservableProperty]
        private string sHA256HashValue = string.Empty;

        [ObservableProperty]
        private string blake2bHashValue = string.Empty;

        [ObservableProperty]
        private string blake3MTHashValue = string.Empty;

        [ObservableProperty]
        private double mD5Progress = 0;

        [ObservableProperty]
        private double sHA256Progress = 0;

        [ObservableProperty]
        private double blake2bProgress = 0;

        [ObservableProperty]
        private double blake3Progress = 0;

        [ObservableProperty]
        private double blake3MTProgress = 0;


        [RelayCommand]
        private void GetBenchmark()
        {
            //var x = dataTable.Rows;
        }

        [RelayCommand]
        private void HashFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
                                          //dialog.DefaultExt = ".txt"; // Default file extension
                                          //dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                FileName = dialog.FileName;

                FileInfo fileInfo = new FileInfo(FileName);
                if (fileInfo.Exists == true) FileSize = (double)fileInfo.Length / (1024 * 1024);

                BufferSize = GetBufferSize(FileSize);
                BufferSizeInKBs = BufferSize / 1024;

                Task.Run(async () =>
                {
                    await GetMd5Hash();
                    //await GetSHA256Hash();
                    //await GetBlake2bHash();
                    await GetBlake3Hash();
                    await GetBlake3MTHash();
                });
            }
        }

        private async Task GetMd5Hash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            byte[] hash = null;

            watch = System.Diagnostics.Stopwatch.StartNew();
            hash = CalculateMD5HashForFile(FileName);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;

            Md5HashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Md5HashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetSHA256Hash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            byte[] hash = null;

            watch = System.Diagnostics.Stopwatch.StartNew();
            hash = CalculateSHA256HashForFile(FileName);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;


            SHA256HashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            SHA256HashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetBlake2bHash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            byte[] hash = null;

            watch = System.Diagnostics.Stopwatch.StartNew();
            hash = CalculateBlake2bHashForFile(FileName);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;


            Blake2bHashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Blake2bHashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetBlake3Hash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            Hash blake3hash = new Hash();

            watch = System.Diagnostics.Stopwatch.StartNew();
            blake3hash = CalculateBlake3HashForFile(FileName);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;


            Blake3HashValue = blake3hash.ToString();
            Blake3HashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetBlake3MTHash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            Hash blake3hash = new Hash();

            watch = System.Diagnostics.Stopwatch.StartNew();
            blake3hash = CalculateBlake3MTHashForFile(FileName);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;

            Blake3MTHashValue = blake3hash.ToString();
            Blake3MTHashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }

        public byte[] CalculateMD5HashForFile(string filePath)
        {
            IDigest md5Digest = new MD5Digest();
            MD5Progress = 0;
            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, md5Digest, null))
            {

                byte[] buffer = new byte[BufferSize]; // Adjust buffer size as needed
                long bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    MD5Progress = (double)fileStream.Position / fileStream.Length * 100;
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[md5Digest.GetDigestSize()];
            md5Digest.DoFinal(hashBytes, 0);
            return hashBytes;
        }
        public byte[] CalculateSHA256HashForFile(string filePath)
        {
            IDigest sha256Digest = new Sha256Digest();
            SHA256Progress = 0;
            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, sha256Digest, null))
            {
                byte[] buffer = new byte[BufferSize]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    SHA256Progress = (double)fileStream.Position / fileStream.Length * 100;
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[sha256Digest.GetDigestSize()];
            sha256Digest.DoFinal(hashBytes, 0);
            return hashBytes;
        }
        public byte[] CalculateBlake2bHashForFile(string filePath)
        {
            IDigest blake2bDigest = new Blake2bDigest(256);
            Blake2bProgress = 0;
            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, blake2bDigest, null))
            {
                byte[] buffer = new byte[BufferSize]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    Blake2bProgress = (double)fileStream.Position / fileStream.Length * 100;
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[blake2bDigest.GetDigestSize()];
            blake2bDigest.DoFinal(hashBytes, 0);
            return hashBytes;
        }
        public Hash CalculateBlake3HashForFile(string filePath)
        {
            Blake3Progress = 0;
            using (var blake3 = Blake3.Hasher.New())
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[BufferSize]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    blake3.Update(buffer.AsSpan(0, bytesRead));
                    Blake3Progress = (double)fileStream.Position / fileStream.Length * 100;
                } while (bytesRead > 0);

                return blake3.Finalize();
            }
        }
        public Hash CalculateBlake3MTHashForFile(string filePath)
        {
            Blake3MTProgress = 0;
            using (var blake3 = Blake3.Hasher.New())
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[BufferSize]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    blake3.UpdateWithJoin(buffer.AsSpan(0, bytesRead));
                    Blake3MTProgress = (double)fileStream.Position / fileStream.Length * 100;
                } while (bytesRead > 0);

                return blake3.Finalize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSize">Gets fileSize in MBs</param>
        /// <returns></returns>
        private int GetBufferSize(double fileSize)
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
