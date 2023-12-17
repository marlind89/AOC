namespace AOC2023.Puzzles;

internal class Puzzle10 : Puzzle<int>
{
    record Pipe((int x, int y) Current, (int x, int y) Previous)
    {
        public Pipe GetNext(char[,] grid)
        {
            List<(int x, int y)> nextPositions = grid[Current.x, Current.y] switch
            {
                '|' => [(0, 1), (0, -1)],
                '-' => [(1, 0), (-1, 0)],
                'L' => [(0, -1), (1, 0)],
                'J' => [(0, -1), (-1, 0)],
                '7' => [(0, 1), (-1, 0)],
                'F' => [(0, 1), (1, 0)],
                _ => []
            };

            (int x, int y) nextPos = nextPositions
                .Select(p => (Current.x + p.x, Current.y + p.y))
                .First(x => x != Previous);

            return new Pipe(nextPos, Current);
        }
    }

    protected override void Solve(string[] lines)
    {
        var grid = Grid.CreateGrid(lines, x => x);

        var (ax, ay) = Grid.FindFirstLocation(grid, 'S');
        var connectedNodes = GetStartNodes(grid, ax, ay).Take(2).ToList();

        var front = connectedNodes[0];
        var back = connectedNodes[1];
        HashSet<(int x, int y)> visitedPipes = [(ax, ay), front.Current, back.Current];

        var pipeFarthestAway = 1;
        while (true)
        {
            pipeFarthestAway++;
            front = front.GetNext(grid);
            back = back.GetNext(grid);
            if (!visitedPipes.Add(front.Current) || !visitedPipes.Add(back.Current))
            {
                break;
            }
        }

        One = pipeFarthestAway;
    }

    private static IEnumerable<Pipe> GetStartNodes(char[,] grid, int ax, int ay)
    {
        foreach (var (nx, ny) in Grid.GetNeighbours(grid, (ax, ay), false))
        {
            var isConnected = (grid[nx, ny], (ax - nx, ay - ny)) switch
            {
                ('|', (0, 1) or (0, -1)) => true,
                ('-', (1, 0) or (-1, 0)) => true,
                ('L', (0, -1) or (1, 0)) => true,
                ('J', (0, -1) or (-1, 0)) => true,
                ('7', (0, 1) or (-1, 0)) => true,
                ('F', (0, 1) or (1, 0)) => true,
                _ => false
            };

            if (isConnected)
            {
                yield return new Pipe((nx, ny), (ax, ay));
            }
        }
    }
}

