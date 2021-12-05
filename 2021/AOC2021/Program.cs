// See https://aka.ms/new-console-template for more information
using AOC2021.Puzzles;

var type = typeof(IPuzzle);
var puzzles = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
    .Select(Activator.CreateInstance)
    .OfType<IPuzzle>();

Console.WriteLine("Advent Of Code 2021");
Console.WriteLine(new string('-', 30));
foreach (var puzzle in puzzles)
{
    Console.WriteLine(puzzle);
}

Console.ReadLine();