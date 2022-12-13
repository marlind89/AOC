namespace AOC2022.Puzzles;

internal class Puzzle12 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var grid = Grid.CreateGrid<char>(lines, x => x);
        
        One = GetSteps(grid, 'S');
        Two = GetSteps(grid, 'S', 'a');
    }

    private static int GetSteps(char[,] grid, params char[] endLocations)
    {
        var from = Grid.Iterate(grid).First(x => grid[x.x, x.y] == 'E');
        var tos = Grid.Iterate(grid).Where(x => endLocations.Contains(grid[x.x, x.y])).ToHashSet();

        return GetSteps(grid, from, tos);
    }

    private static int GetSteps(char[,] grid, (int x, int y) from, IReadOnlyCollection<(int x, int y)> targets)
    {
        var queue = new Queue<(int x, int y)>();
        var totalSteps = new int[grid.GetLength(0), grid.GetLength(1)];

        totalSteps[from.x, from.y] = 0;
        queue.Enqueue(from);

        while (queue.TryDequeue(out var p))
        {
            foreach (var n in Grid.GetNeighbours(grid, p, false))
            {
                if (IsMovePossible(grid[p.x, p.y], grid[n.x, n.y]) && totalSteps[n.x, n.y] == 0)
                {
                    totalSteps[n.x, n.y] = totalSteps[p.x, p.y] + 1;
                    if (targets.Contains(n))
                    {
                        return totalSteps[n.x, n.y];
                    }
                    queue.Enqueue(n);
                }
            }
        }

        return 0;
    }

    private static bool IsMovePossible(char from, char to) => GetElevation(to) >= GetElevation(from) - 1;

    private static char GetElevation(char elev) => elev switch
    {
        'S' => 'a',
        'E' => 'z',
        _ => elev
    };
}
