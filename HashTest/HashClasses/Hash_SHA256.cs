using HasherTest.Interfaces;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HasherTest.HashClasses
{
    public class Hash_SHA256 : IHash
    {
        public EventHandler<double>? ProgressUpdater { get; set; }
        public double CurrentProgress { get; set; }

        public string HashFile(FileData file, long bufferSize)
        {
            IDigest sha256Digest = new Sha256Digest();

            using (var fileStream = File.OpenRead(file.Path))
            using (var digestStream = new DigestStream(fileStream, sha256Digest, null))
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

            byte[] hashBytes = new byte[sha256Digest.GetDigestSize()];
            sha256Digest.DoFinal(hashBytes, 0);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

    }
}
