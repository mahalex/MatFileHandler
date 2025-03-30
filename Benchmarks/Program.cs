using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MatFileHandler;
using System;
using System.IO;

namespace Benchmarks;

[MemoryDiagnoser]
public class BigWriteBenchmark
{
    private IMatFile? matFile;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var m = 1000;
        var n = 10000;
        var builder = new DataBuilder();
        var array = builder.NewArray<double>(m, n);
        var random = new Random(1);
        for (var i = 0; i < m * n; i++)
        {
            array[i] = random.NextDouble();
        }

        var variable = builder.NewVariable("test", array);
        matFile = builder.NewFile(new[] { variable });
    }

    [Benchmark]
    public void V1()
    {
        using var stream = new MemoryStream();
        var writer = new MatFileWriter(stream);
        writer.Write(matFile!);
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        BenchmarkRunner.Run<BigWriteBenchmark>();
    }
}
