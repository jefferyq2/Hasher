using HasherTest.HashClasses;
using HasherTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasherTest.Helpers
{
    public static class HashHelper
    {
        /// <summary>
        /// Returns the appropriate buffer size depending on the size of the file.
        /// </summary>
        /// <param name="fileSize">FileSize in MBs.</param>
        /// <returns>BufferSize in bytes.</returns>
        public static int GetBufferSize(double fileSize)
        {
            if (fileSize <= 1)
            {
                return 8192 * 8;
            }
            else if (fileSize <= 101) //100mb
            {
                return 1024 * 1024 * 1 / 2;
            }
            else if (fileSize <= 512) //500mb
            {
                return 1024 * 512;
            }
            else if (fileSize <= 1024) //1GB
            {
                return 1024 * 1024 * 1;
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

        /// <summary>
        /// Get HashType from file extension.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static HashFunction IdentifyHashType(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension)
            {
                case ".md5":
                    return HashFunction.MD5;
                case ".blake3":
                    return HashFunction.Blake3;
                default:
                    return HashFunction.None;
            }
        }

        public static string GetFileExtensionFilter(HashFunction hashFunction)
        {
            switch(hashFunction)
            {
                case HashFunction.MD5:
                    return "MD5 Hash file (*.md5)|*.md5";
                case HashFunction.Blake2b:
                    return "Blake2 Hash file (*.blake2)|*.blake2";
                case HashFunction.Blake3MultiThreaded:
                case HashFunction.Blake3:
                    return "Blake3 Hash file (*.blake3)|*.blake3";
                default:
                    return "";
            }
        }

        public static IHash? getHashType(HashFunction hashFunction)
        {
            switch (hashFunction)
            {
                case HashFunction.SHA256:
                    return new Hash_SHA256();
                case HashFunction.MD5:
                    return new Hash_MD5();
                case HashFunction.Blake3:
                    return new Hash_Blake3();
                default:
                    return null;
            }
        }



    }
}
