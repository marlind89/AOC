using System.Text.RegularExpressions;

namespace AOC2023.Puzzles;

internal partial class Puzzle2 : Puzzle<int>
{
    [GeneratedRegex(@"Game (\d+):")] private static partial Regex GameRegex();
    private record struct Game(int Id, IReadOnlyCollection<Set> Sets);
    private record struct Set(IDictionary<string, int> Cubes);
    private readonly IDictionary<string, int> AvailableCubes = new Dictionary<string, int>
    {
        ["red"] = 12,
        ["green"] = 13,
        ["blue"] = 14
    };

    protected override void Solve(string[] lines)
    {
        var games = lines.Select(line =>
        {
            var gameId = int.Parse(GameRegex().Match(line).Groups[1].Value);
            var sets = line.Substring(line.IndexOf(": ") + 2).Split("; ")
                .Select(set => new Set(set.Split(", ")
                    .Select(cubeStr => cubeStr.Split(" "))
                    .ToDictionary(x => x[1], x => int.Parse(x[0]))))
                .ToList();

            return new Game(gameId, sets);
        });

        One = games
            .Where(x => x.Sets.All(set => !AvailableCubes.Any(c => set.Cubes.TryGetValue(c.Key, out var amount) && amount > c.Value)))
            .Sum(x => x.Id);

        Two = games
            .Select(game => game.Sets
                .SelectMany(x => x.Cubes)
                .GroupBy(x => x.Key, x => x.Value, (_, vals) => vals.Max())
                .Aggregate((val, acc) => val * acc))
            .Sum();
    }
}
