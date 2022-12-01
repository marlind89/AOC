namespace AOC2021.Puzzles;

internal class Puzzle7 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var positions = lines[0].Split(',').Select(int.Parse).ToArray();
        One = GetCheapestPosition(positions, true);
        Two = GetCheapestPosition(positions, false);
    }

    private static int GetCheapestPosition(IReadOnlyCollection<int> positions, bool burnAtConstantRate) => 
        Enumerable.Range(0, positions.Max())
            .Aggregate(int.MaxValue, (lowestFuel, pos) => Math
                .Min(lowestFuel, positions
                    .Where(x => x != pos)
                    .Aggregate(0, (a, b) =>
                    {
                        var distance = Math.Abs(pos - b);
                        return a + (burnAtConstantRate
                            ? distance
                            : (distance * (distance + 1)) / 2);
                    }))
            );
}
