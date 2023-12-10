using System;

namespace AOC2023.Puzzles;

internal class Puzzle6 : Puzzle<long>
{
    protected override void Solve(string[] lines)
    {
        var parse = (int idx) => lines[idx][(1 + lines[idx].IndexOf(':'))..]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var races = parse(0).Zip(parse(1))
            .Select(x => (Time: int.Parse(x.First), Distance: int.Parse(x.Second)))
            .ToList();

        One = races
            .Select(x => GetRaceWins(x.Time, x.Distance))
            .Aggregate((a, b) => a * b);

        var (time, dist) = (long.Parse(string.Concat(parse(0))), long.Parse(string.Concat(parse(1))));
        Two = GetRaceWins(time, dist);
    }

    private static long GetRaceWins(long time, long dist)
    {
        var b = time / 2d;
        var sqrt = Math.Sqrt(b*b - dist);
        var min = (long)Math.Ceiling(b - sqrt);
        var max = (long)Math.Floor(b + sqrt);
        return max - min + 1;
    }
}
