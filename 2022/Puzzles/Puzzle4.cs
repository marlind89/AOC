namespace AOC2022.Puzzles;

internal class Puzzle4 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var overlaps = lines
            .Select(x => x.Split(',')
                .Select(range => range.Split('-').Select(int.Parse).ToList())
                .ToList())
            .Where(ranges => ranges[0][0] <= ranges[1][1] && ranges[1][0] <= ranges[0][1])
            .ToList();

        One = overlaps.Count(ranges => (ranges[0][0] <= ranges[1][0] && ranges[0][1] >= ranges[1][1]) ||
            (ranges[1][0] <= ranges[0][0] && ranges[1][1] >= ranges[0][1]));
        Two = overlaps.Count;
    }
}
