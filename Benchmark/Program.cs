using System;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Importing data");
            var runner = new Runner();
            var it = runner.ImportAsync();
            it.Wait();
            Console.WriteLine("Imported {0}", it.Result);

            Console.WriteLine("running benchmark");
            var t = runner.RunAsync();
            t.Wait();

            Console.WriteLine("It tooks {0:0.00} milliseconds to query", t.Result.TotalMilliseconds);

            runner.FlushDatabase();
        }
    }
}
