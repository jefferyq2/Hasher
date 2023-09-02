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

namespace HashTest.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            
        }

        private const int BUFFER_SIZE = 524288+524288;

        [ObservableProperty]
        private string fileName = string.Empty;


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
        private double mD5Progress= 0;


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
                MD5Progress = 0.0;
                Task.Run(async () =>
                {
                    await GetMd5Hash();
                });
                //GetMd5Hash();
                //GetSHA256Hash();
                //GetBlake2bHash();
                //GetBlake3Hash();
                //GetBlake3MTHash();
            }
        }

        private async Task GetMd5Hash()
        {
            Stopwatch? watch = System.Diagnostics.Stopwatch.StartNew();
            var hash = CalculateMD5HashForFile(FileName);

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Md5HashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Md5HashValue += "\n Time elapsed : " + elapsedMs.ToString() + "ms.";
        }
        private void GetSHA256Hash()
        {
            Stopwatch? watch = System.Diagnostics.Stopwatch.StartNew();
            var hash = CalculateSHA256HashForFile(FileName);

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            SHA256HashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            SHA256HashValue += "\n Time elapsed : " + elapsedMs.ToString() + "ms.";
        }
        private void GetBlake2bHash()
        {
            Stopwatch? watch = System.Diagnostics.Stopwatch.StartNew();
            var hash = CalculateBlake2bHashForFile(FileName);

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Blake2bHashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Blake2bHashValue += "\n Time elapsed : " + elapsedMs.ToString() + "ms.";
        }
        private void GetBlake3Hash()
        {
            Stopwatch? watch = System.Diagnostics.Stopwatch.StartNew();

            var blake3hash = CalculateBlake3HashForFile(FileName);

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Blake3HashValue = blake3hash.ToString();
            Blake3HashValue += "\n Time elapsed : " + elapsedMs.ToString() + "ms.";
        }
        private void GetBlake3MTHash()
        {
            Stopwatch? watch = System.Diagnostics.Stopwatch.StartNew();

            var blake3hash = CalculateBlake3MTHashForFile(FileName);

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Blake3MTHashValue = blake3hash.ToString();
            Blake3MTHashValue += "\n Time elapsed : " + elapsedMs.ToString() + "ms.";
        }

        public byte[] CalculateMD5HashForFile(string filePath)
        {
            IDigest md5Digest = new MD5Digest();
            MD5Progress = 0;
            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, md5Digest, null))
            {

                byte[] buffer = new byte[BUFFER_SIZE]; // Adjust buffer size as needed
                //long totalBytesRead = 0;
                long bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    //totalBytesRead += bytesRead;
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        MD5Progress = (double)fileStream.Position / fileStream.Length * 100;
                    });
                    //MD5Progress = (double)fileStream.Position/fileStream.Length*100;
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[md5Digest.GetDigestSize()];
            md5Digest.DoFinal(hashBytes, 0);
            return hashBytes;
        }
        public byte[] CalculateSHA256HashForFile(string filePath)
        {
            IDigest sha256Digest = new Sha256Digest();

            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, sha256Digest, null))
            {
                byte[] buffer = new byte[BUFFER_SIZE]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[sha256Digest.GetDigestSize()];
            sha256Digest.DoFinal(hashBytes, 0);
            return hashBytes;
        }
        public byte[] CalculateBlake2bHashForFile(string filePath)
        {
            IDigest blake2bDigest = new Blake2bDigest(256);

            using (var fileStream = File.OpenRead(filePath))
            using (var digestStream = new DigestStream(fileStream, blake2bDigest, null))
            {
                byte[] buffer = new byte[BUFFER_SIZE]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[blake2bDigest.GetDigestSize()];
            blake2bDigest.DoFinal(hashBytes, 0);
            return hashBytes;
        }
        public Hash CalculateBlake3HashForFile(string filePath)
        {
            using (var blake3 = Blake3.Hasher.New())
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[BUFFER_SIZE]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    blake3.Update(buffer.AsSpan(0, bytesRead));
                } while (bytesRead > 0);

                return blake3.Finalize();
            }
        }
        public Hash CalculateBlake3MTHashForFile(string filePath)
        {
            using (var blake3 = Blake3.Hasher.New())
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[BUFFER_SIZE]; // Adjust buffer size as needed
                int bytesRead;
                do
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    blake3.UpdateWithJoin(buffer.AsSpan(0, bytesRead));
                } while (bytesRead > 0);

                return blake3.Finalize();
            }
        }

        partial void OnMD5ProgressChanged(double value)
        {
            System.Diagnostics.Debug.WriteLine(MD5Progress.ToString());
        }
    }
}
