using System.Diagnostics;
using System.Reflection;

namespace AOC.Helpers;

public static class ProjectRunner
{
    public static void Run(int year)
    {
        var type = typeof(IPuzzle);
        var puzzles = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .Select(Activator.CreateInstance)
            .OfType<IPuzzle>()
            .OrderBy(x => x.PuzzleNumber)
            .ToList();

        var focusPuzzle = puzzles.FirstOrDefault(x => x.GetType().GetCustomAttribute<FocusAttribute>() != null);
        if (focusPuzzle != null)
        {
            puzzles = puzzles.Where(x => x == focusPuzzle).ToList();
        }

        var sw = new Stopwatch();
        Console.WriteLine($"Advent Of Code {year}");
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

        if (focusPuzzle == null)
        {
            var errors = PuzzleValidator.ValidatePuzzles(puzzles);
            if (!string.IsNullOrWhiteSpace(errors))
            {
                Console.WriteLine();
                Console.WriteLine("!!!!!!!!!!!! Some puzzles are not valid anymore !!!!!!!!!!!!");
                Console.WriteLine();
                Console.WriteLine(errors);
            }
        }

        Console.ReadLine();
    }
}