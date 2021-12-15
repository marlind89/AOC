// See https://aka.ms/new-console-template for more information
using AOC2021.Puzzles;
using System.Diagnostics;

var type = typeof(IPuzzle);
var puzzles = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
    .Select(Activator.CreateInstance)
    .OfType<IPuzzle>()
    .OrderBy(x => x.PuzzleNumber)
    .ToList();

var sw = new Stopwatch();
Console.WriteLine("Advent Of Code 2021");
Console.WriteLine(new string('-', 30));
sw.Start();
foreach (var puzzle in puzzles)
{
    puzzle.Solve();
}
sw.Stop();

foreach (var puzzle in puzzles)
{
    Console.WriteLine(puzzle);
}

Console.WriteLine(new string('-', 30));
Console.WriteLine($"Total time (ms): {sw.ElapsedMilliseconds}");

Console.ReadLine();