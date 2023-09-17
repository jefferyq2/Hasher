using HasherTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasherTest.HashClasses
{
    public class Hash_Blake3 : IHash
    {
        public double CurrentProgress {  get; set; }
        
        public EventHandler<double>? ProgressUpdater { get; set; }
        
        public string HashFile(FileData file, long bufferSize)
        {
            using Blake3.Hasher blake3 = Blake3.Hasher.New();
            using FileStream fileStream = File.OpenRead(file.Path);
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            do
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                blake3.Update(buffer.AsSpan(0, bytesRead));
                CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
                ProgressUpdater?.Invoke(this, CurrentProgress);
            } while (bytesRead > 0);

            return blake3.Finalize().ToString();
        }

        public string HashFile(FileData file, long bufferSize, bool isMultiThreaded = true)
        {
            using Blake3.Hasher blake3 = Blake3.Hasher.New();
            using FileStream fileStream = File.OpenRead(file.Path);
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            do
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                blake3.UpdateWithJoin(buffer.AsSpan(0, bytesRead));
                CurrentProgress = (double)fileStream.Position / fileStream.Length * 100;
                ProgressUpdater?.Invoke(this, CurrentProgress);
            } while (bytesRead > 0);

            return blake3.Finalize().ToString();
        }
    }
}
