namespace AOC2022.Puzzles;

internal class Puzzle23 : Puzzle<int>
{
    static readonly (int x, int y) North = new(0, -1);
    static readonly (int x, int y) South = new(0, 1);
    static readonly (int x, int y) West = new(-1, 0);
    static readonly (int x, int y) East = new(1, 0);

    record struct Move((int x, int y) From, (int x, int y) To);

    protected override void Solve(string[] lines)
    {
        var grid = Grid.CreateGrid(AddSpacingsToInput(lines), c => c);
        var directions = new List<(int x, int y)> { North, South, West, East };

        foreach (var _ in Enumerable.Range(0, 10))
        {
            MoveElves(grid, directions);
        }

        One = IterateBounds(grid).Count(p => grid[p.x, p.y] == '.');

        var i = 10;
        while (MoveElves(grid, directions)) i++;
        Two = i + 1;
    }

    private static string[] AddSpacingsToInput(string[] lines)
    {
        var emptySpace = new string('.', lines[0].Length);
        var padded = lines.Select(l => $"{emptySpace}{l}{emptySpace}").ToArray();
        emptySpace = new string('.', padded[0].Length);
        var emptyLines = Enumerable.Range(0, lines.Length).Select(_ => emptySpace);
        return emptyLines.Concat(padded).Concat(emptyLines).ToArray();
    }

    private static bool MoveElves(char[,] grid, List<(int x, int y)> directions)
    {
        var moveProposals = Grid.Iterate(grid)
            .Where(p => grid[p.x, p.y] == '#')
            .Select(elf =>
            {
                var neighbours = Grid.GetNeighbours(grid, elf, true)
                    .Where(n => grid[n.x, n.y] == '#')
                    .ToList();

                if (neighbours.Count == 0)
                {
                    return default;
                }

                var newPos = directions
                    .Where(d =>
                    {
                        var newPos = (x: elf.x + d.x, y: elf.y + d.y);

                        var cellsToCheck = new List<(int x, int y)>(d.y != 0
                            ? new[] { (newPos.x - 1, newPos.y), (newPos.x + 1, newPos.y) }
                            : new[] { (newPos.x, newPos.y - 1), (newPos.x, newPos.y + 1) }) { newPos };

                        return cellsToCheck.All(p => grid[p.x, p.y] == '.');
                    })
                    .FirstOrDefault();

                return newPos == default
                    ? default
                    : new Move((elf.x, elf.y), (elf.x + newPos.x, elf.y + newPos.y));
            })
            .Where(x => x != default)
            .GroupBy(x => x.To)
            .Where(x => x.Count() == 1)
            .Select(x => x.First())
            .ToList();

        foreach (var (from, to) in moveProposals)
        {
            grid[from.x, from.y] = '.';
            grid[to.x, to.y] = '#';
        }

        var dir = directions[0];
        directions.Remove(dir);
        directions.Add(dir);

        return moveProposals.Count > 0;
    }

    private static IEnumerable<(int x, int y)> IterateBounds(char[,] grid)
    {
        var elves = Grid.Iterate(grid).Where(p => grid[p.x, p.y] == '#');

        var (xMin, xMax, yMin, yMax) = (elves.Min(p => p.x), elves.Max(p => p.x) + 1,
            elves.Min(p => p.y), elves.Max(p => p.y) + 1);

        return Enumerable.Range(xMin, xMax - xMin)
            .SelectMany(x => Enumerable.Range(yMin, yMax - yMin).Select(y => (x, y)));
    }
}
