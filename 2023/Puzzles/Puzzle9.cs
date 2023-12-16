namespace AOC2023.Puzzles;

internal class Puzzle9 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var histories = lines
            .Select(x => x.Split(' ', StringSplitOptions.TrimEntries).Select(int.Parse).ToList())
            .ToList();

        var results = histories.Select(GetPrediction).ToArray();
        One = results.Sum(x => x.Next);
        Two = results.Sum(x => x.Previous);
    }

    private (int Previous, int Next) GetPrediction(List<int> list)
    {
        var differences = new List<List<int>>() { list };
        var currentDifference = list;
        while (currentDifference.Any(x => x != 0))
        {
            currentDifference = currentDifference.Zip(currentDifference.Skip(1))
                .Select(x => x.Second - x.First)
                .ToList();
            differences.Insert(0, currentDifference);
        }

        foreach (var (first, second) in differences.Zip(differences.Skip(1)))
        {
            second.Add(first.Last() + second.Last());
            second.Insert(0, second.First() - first.First());
        }

        var history = differences.Last();
        return (history.First(), history.Last());
    }
}

