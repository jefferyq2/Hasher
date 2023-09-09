using BenchmarkDotNet.Running;

namespace HasherBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            BenchmarkRunner.Run<HasherBenchmark>();
        }
    }
}