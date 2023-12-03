namespace AOC2023.Puzzles;

internal class Puzzle3 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var grid = Grid.CreateGrid(lines, x => x);

        var (partsSum, gearRatio) = (0, 0);
        foreach (var (x,y) in Grid.Iterate(grid))
        {
            var val = grid[x, y];
            if (!char.IsNumber(val) && val != '.')
            {
                var parts = new HashSet<int>();
                foreach (var (nx, ny) in Grid.GetNeighbours(grid, (x,y), true))
                {
                    var nVal = grid[nx, ny];

                    if (char.IsNumber(nVal))
                    {
                        var (nStart, nEnd) = (nx, nx);
                        while (nStart > 0 && char.IsDigit(grid[nStart - 1, ny]))
                        {
                            nStart--;
                        }
                        while (nEnd < (grid.GetLength(0) - 1) && char.IsDigit(grid[nEnd + 1, ny]))
                        {
                            nEnd++;
                        }

                        parts.Add(int.Parse(string.Concat(Enumerable.Range(
                            nStart, nEnd - nStart + 1).Select(p => grid[p, ny]))));
                    }
                }
                partsSum += parts.Sum();
                if (val == '*' && parts.Count == 2)
                {
                    gearRatio += parts.Aggregate((x, y) => x * y);
                }
            }
        }

        One = partsSum;
        Two = gearRatio;
    }
}
