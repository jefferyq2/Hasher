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
    //[Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 3, iterationCount: 5)]
    [RankColumn]
    public class HasherBenchmark
    {
        [Params(256,512,1024,2048,4096)]
        public int BufferSize;
        //[Params(1,100)]
        //public int N;
        private const string FILE_NAME = "D:\\Games\\Dragon Age series\\Dragon Age 2 Ultimate Edition - [DODI Repack]\\data1.dd";
        private static readonly Hasher hasher = new Hasher();

        [Benchmark]
        public byte[] MD5()
        {
            hasher.BufferSize = BufferSize * 1024;
            return hasher.CalculateMD5HashForFile(FILE_NAME);
        }

        //[Benchmark]
        //public void SHA256()
        //{
        //    hasher.BufferSize = 1024 * 1024 * 1;
        //    hasher.CalculateSHA256HashForFile(FILE_NAME);
        //}

        //[Benchmark]
        //public void Blake2()
        //{
        //    hasher.BufferSize = 1024 * 1024 * 1;
        //    hasher.CalculateBlake2bHashForFile(FILE_NAME);
        //}

        [Benchmark]
        public Hash Blake3()
        {
            hasher.BufferSize = BufferSize * 1024;
            return hasher.CalculateBlake3HashForFile(FILE_NAME);
        }

        [Benchmark]
        public Hash Blake3MT()
        {
            hasher.BufferSize = BufferSize * 1024;
            return hasher.CalculateBlake3MTHashForFile(FILE_NAME);
        }
    }

    public class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            // Configure the job to run a fixed number of iterations (5 in this example)
            Add(Job.Default.WithIterationCount(5));

            // Or configure the job to run for a specific amount of time (5 minutes in this example)
            // Add(Job.Default.WithMaxDuration(TimeSpan.FromMinutes(5)));
        }
    }
}
