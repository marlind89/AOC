namespace AOC2023.Puzzles;

internal class Puzzle11 : Puzzle<long, long>
{
    protected override void Solve(string[] lines)
    {
        var emptyRows = lines
            .Select((line, idx) => (line, idx))
            .Where(x => x.line.All(l => l == '.'))
            .Select(x => x.idx)
            .ToList();

        var emptyCols = Enumerable.Range(0, lines[0].Length - 1)
            .Where(x => lines.All(line => line[x] == '.'))
            .ToList();

        var space = Grid.CreateGrid(lines, c => c);
        var galaxyPairs = Grid.Iterate(space)
            .Where(p => space[p.x, p.y] == '#')
            .AsPairs()
            .ToList();

        long GetGalaxyDistanceSum(int galaxyAge)
        {
            return galaxyPairs.Sum(g =>
            {
                var sx = GetEmptySpaceCount(g.First.y, g.Second.y, emptyRows);
                var sy = GetEmptySpaceCount(g.First.x, g.Second.x, emptyCols);
                return galaxyAge * (sx + sy) + Grid.ManhattanDistance(g.First, g.Second);
            });
        }

        One = GetGalaxyDistanceSum(1);
        Two = GetGalaxyDistanceSum(1_000_000 - 1);
    }

    private static long GetEmptySpaceCount(int first, int second, List<int> emptyRows) => 
        GetIndex(Math.Max(first, second), emptyRows) - GetIndex(Math.Min(first, second), emptyRows);

    private static long GetIndex(int value, List<int> emptyRows)
    {
        var index = emptyRows.BinarySearch(value);
        return index < 0 ? ~index : index;
    }
}

