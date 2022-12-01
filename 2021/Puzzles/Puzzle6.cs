namespace AOC2021.Puzzles;

internal class Puzzle6 : Puzzle<long>
{
    protected override void Solve(string[] lines)
    {
        var initialFishes = Enumerable.Range(0, 9).Select(_ => 0L).ToArray();

        foreach (var fish in lines[0].Split(",").Select(int.Parse))
        {
            initialFishes[fish] += 1;
        }

        One = CountFishes(initialFishes, 80);
        Two = CountFishes(initialFishes, 256);
    }

    private static long CountFishes(long[] fishes, int days)
    {
        fishes = fishes.ToArray();

        foreach (var _ in Enumerable.Range(0, days))
        {
            SimulateDay(fishes);
        }

        return fishes.Sum();
    }

    private static void SimulateDay(long[] fishes)
    {
        var newSpawns = fishes[0];

        foreach (var age in Enumerable.Range(1, 8))
        {
            fishes[age - 1] = fishes[age];
        }

        fishes[6] += newSpawns;
        fishes[8] = newSpawns;
    }
}
