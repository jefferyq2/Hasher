using Blake3;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasherBenchmark
{
    internal class Hasher
    {
        private const int NUMBER_OF_ITERATIONS = 3;
        //public Hasher()
        //{
        //    //DataColumn Run1 = new DataColumn("Run1", typeof(double));
        //    //DataColumn Run2 = new DataColumn("Run2", typeof(double));
        //    //DataColumn Run3 = new DataColumn("Run3", typeof(double));
        //    //DataColumn Run4 = new DataColumn("Run4", typeof(double));
        //    //DataColumn Run5 = new DataColumn("Run5", typeof(double));

        //    //dataTable.Columns.Add(Run1);
        //    //dataTable.Columns.Add(Run2);
        //    //dataTable.Columns.Add(Run3);
        //    //dataTable.Columns.Add(Run4);
        //    //dataTable.Columns.Add(Run5);
        //}

        //DataTable dataTable = new DataTable();

        public string FileName { get; set; } = string.Empty;

        public int BufferSizeInKBs { get; set; } = 0;

        public double FileSize { get; set; } = 0;

        public int BufferSize { get; set; } = 0;

        public string Md5HashValue { get; set; } = string.Empty;

        public string Blake3HashValue { get; set; } = string.Empty;

        public string SHA256HashValue { get; set; } = string.Empty;

        public string Blake2bHashValue { get; set; } = string.Empty;

        public string Blake3MTHashValue { get; set; } = string.Empty;

        public double MD5Progress { get; set; } = 0;

        public double SHA256Progress { get; set; } = 0;

        public double Blake2bProgress { get; set; } = 0;

        public double Blake3Progress { get; set; } = 0;

        public double Blake3MTProgress { get; set; } = 0;

        public void HashFile(string fileName)
        {
            //var dialog = new Microsoft.Win32.OpenFileDialog();
            //dialog.FileName = "Document"; // Default file name
                                          //dialog.DefaultExt = ".txt"; // Default file extension
                                          //dialog.Filter = "Text documents (.txt)|*.txt";

            //bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            //if (result == true)
            //{
                // Open document
                FileName = fileName;

                FileInfo fileInfo = new FileInfo(FileName);
                if (fileInfo.Exists == true) FileSize = (double)fileInfo.Length / (1024 * 1024);

                BufferSize = GetBufferSize(FileSize);
                BufferSizeInKBs = BufferSize / 1024;

                Task.Run(async () =>
                {
                    await GetMd5Hash();
                    await GetSHA256Hash();
                    await GetBlake2bHash();
                    await GetBlake3Hash();
                    await GetBlake3MTHash();
                });
           // }
        }

        private async Task GetMd5Hash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            byte[] hash = null;

            for (int i = 0; i < NUMBER_OF_ITERATIONS; i++)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                hash = CalculateMD5HashForFile(FileName);
                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
            }

            Md5HashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Md5HashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetSHA256Hash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            byte[] hash = null;

            for (int i = 0; i < NUMBER_OF_ITERATIONS; i++)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                hash = CalculateSHA256HashForFile(FileName);
                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
            }


            SHA256HashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            SHA256HashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetBlake2bHash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            byte[] hash = null;

            for (int i = 0; i < NUMBER_OF_ITERATIONS; i++)
            {

                watch = System.Diagnostics.Stopwatch.StartNew();
                hash = CalculateBlake2bHashForFile(FileName);
                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
            }


            Blake2bHashValue = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Blake2bHashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetBlake3Hash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            Hash blake3hash = new Hash();

            for (int i = 0; i < NUMBER_OF_ITERATIONS; i++)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                blake3hash = CalculateBlake3HashForFile(FileName);
                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
            }


            Blake3HashValue = blake3hash.ToString();
            Blake3HashValue += "\n Time elapsed : " + ((double)elapsedMs / 1000).ToString() + "s.";
        }
        private async Task GetBlake3MTHash()
        {
            Stopwatch? watch;
            long elapsedMs = 0;
            Hash blake3hash = new Hash();

            for (int i = 0; i < NUMBER_OF_ITERATIONS; i++)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                blake3hash = CalculateBlake3MTHashForFile(FileName);
                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
            }

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
                    //System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    //{
                    MD5Progress = (double)fileStream.Position / fileStream.Length * 100;
                    //});
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
