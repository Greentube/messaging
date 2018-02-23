using BenchmarkDotNet.Running;

namespace Greentube.Messaging.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MessagehandlerInfoProviderBenchmarks>();
        }
    }
}
