using BenchmarkDotNet.Running;

namespace HasherBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<HasherBenchmark>();
        }
    }
}