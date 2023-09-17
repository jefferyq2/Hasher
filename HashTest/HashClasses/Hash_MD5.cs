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
    public class Hash_MD5 : IHash
    {
        public EventHandler<double>? ProgressUpdater { get; set; }
        public double CurrentProgress { get; set; }

        public string HashFile(FileData file, long bufferSize)
        {
            IDigest md5Digest = new MD5Digest();
            using (var fileStream = File.OpenRead(file.Path))
            using (var digestStream = new DigestStream(fileStream, md5Digest, null))
            {
                byte[] buffer = new byte[bufferSize];
                long bytesRead;
                do
                {
                    bytesRead = digestStream.Read(buffer, 0, buffer.Length);
                    CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
                    ProgressUpdater?.Invoke(this, CurrentProgress);
                } while (bytesRead > 0);
            }

            byte[] hashBytes = new byte[md5Digest.GetDigestSize()];
            md5Digest.DoFinal(hashBytes, 0);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
