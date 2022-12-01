using System.Linq;

namespace AOC2022.Puzzles;

internal class Puzzle1 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var elves = lines.Aggregate(new List<int>() { 0 }, (cals, line) =>
        {
            if (string.IsNullOrWhiteSpace(line)) cals.Add(0);
            else cals[^1] += int.Parse(line);
            return cals;
        }).OrderByDescending(x => x);

        One = elves.First();
        Two = elves.Take(3).Sum();
    }
}
