namespace AOC2022.Puzzles;

internal class Puzzle3 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        One = lines
            .Sum(line => line.Substring(0, line.Length / 2)
                .Intersect(line.Substring(line.Length / 2))
                .Select(GetPriority)
                .Single());
        Two = lines
            .Chunk(3)
            .Sum(group => group
                .Skip(1)
                .Aggregate(group.First(), (a, b) => string.Concat(a.Intersect(b)), x => GetPriority(x.Single())));
    }

    private static int GetPriority(char type) => type - (char.IsUpper(type) ? 38 : 96);
}
