// See https://aka.ms/new-console-template for more information
using System.Reflection;
using MetricUtils;

Console.WriteLine("Hello, World!");

var a = Assembly.LoadFrom("C:/Users/igorm/Documents/MetricUtils/TestProject/bin/Debug/net8.0/TestProject.dll");

var treesBuilder = new TreesBuilder(a);

var trees = treesBuilder.BuildTrees();

foreach (var dependencyTree in trees)
{
    var printer = new TreePrinter(dependencyTree);
    printer.PrintMaxDepth();
    printer.PrintDepthOfEachNode();
    Console.WriteLine("===============");
}

Console.WriteLine("MIF=" + new MifCalculator(trees).Calculate());
Console.WriteLine("MHF=" + new MhfCalculator(trees).Calculate());
Console.WriteLine("AIF=" + new AifCalculator(trees).Calculate());
Console.WriteLine("AHF=" + new AhfCalculator(trees).Calculate());
Console.WriteLine("POF=" + new PofCalculator(trees).Calculate());
