namespace AOC2022.Puzzles;

internal partial class Puzzle8 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var trees = GetTreeViews(lines).ToList();

        One = trees.Count(tree => tree.Visible);
        Two = trees.Max(tree => tree.Score);
    }

    private static IEnumerable<(bool Visible, int Score)> GetTreeViews(string[] lines)
    {
        var grid = Grid.CreateGrid(lines);
        return Grid.Iterate(grid)
            .Select(cell =>
            {
                var (treeHeight, treeVisible, totalScore) = (grid[cell.x, cell.y], false, 1);
                foreach (var (dx, dy) in Grid.NeighborOffsets)
                {
                    var (clearSight, score, neighbour) = (true, 0, cell);
                    while (!Grid.IsOutOfRange(grid, neighbour = (neighbour.x + dx, neighbour.y + dy)))
                    {
                        score++;
                        if (grid[neighbour.x, neighbour.y] >= treeHeight)
                        {
                            clearSight = false;
                            break;
                        }
                    }

                    totalScore *= score;
                    treeVisible |= clearSight;
                }

                return (treeVisible, totalScore);
            });
    }
}
