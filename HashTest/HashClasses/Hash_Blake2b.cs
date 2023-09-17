using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HasherTest.Interfaces;
using System.IO;

namespace HasherTest.HashClasses
{
    public class Hash_Blake2b : IHash
    {
        public EventHandler<double>? ProgressUpdater { get; set; }
        public double CurrentProgress { get; set; }

        public string HashFile(FileData file, long bufferSize)
        {
            IDigest blake2bDigest = new Blake2bDigest(256);

            using (var fileStream = File.OpenRead(file.Path))
            using (var digestStream = new DigestStream(fileStream, blake2bDigest, null))
            {
                byte[] buffer = new byte[bufferSize];
                int bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
                    ProgressUpdater?.Invoke(this, CurrentProgress);
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[blake2bDigest.GetDigestSize()];
            blake2bDigest.DoFinal(hashBytes, 0);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

    }
}
