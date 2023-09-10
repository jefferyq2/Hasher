using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Blake3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasherBenchmark
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 3, iterationCount: 5)]
    [RankColumn]
    public class HasherBenchmark
    {
        [Params(256,512,1024,2048,4096)]
        public int BufferSizeInKBs;
        //[Params(1,100)]
        //public int N;
        private const string FILE_NAME = "D:\\Games\\Dragon Age series\\Dragon Age 2 Ultimate Edition - [DODI Repack]\\data1.dd";
        private static readonly Hasher hasher = new Hasher();

        [Benchmark]
        public byte[] MD5()
        {
            hasher.BufferSize = getBufferSizeInBytes(BufferSizeInKBs);
            return hasher.CalculateMD5HashForFile(FILE_NAME);
        }

        [Benchmark]
        public void SHA256()
        {
            hasher.BufferSize = getBufferSizeInBytes(BufferSizeInKBs);
            hasher.CalculateSHA256HashForFile(FILE_NAME);
        }

        [Benchmark]
        public void Blake2()
        {
            hasher.BufferSize = getBufferSizeInBytes(BufferSizeInKBs);
            hasher.CalculateBlake2bHashForFile(FILE_NAME);
        }

        [Benchmark]
        public Hash Blake3()
        {
            hasher.BufferSize = getBufferSizeInBytes(BufferSizeInKBs);
            return hasher.CalculateBlake3HashForFile(FILE_NAME);
        }

        [Benchmark]
        public Hash Blake3MT()
        {
            hasher.BufferSize = getBufferSizeInBytes(BufferSizeInKBs);
            return hasher.CalculateBlake3MTHashForFile(FILE_NAME);
        }

        private int getBufferSizeInBytes(int bufferSize)
        {
            return bufferSize * 1024;
        }
    }
}
