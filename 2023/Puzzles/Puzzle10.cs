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

        grid[ax, ay] = GetStartPipe((ax, ay), front.Current, back.Current);

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
        Two = GetTilesEnclosedByLoop(grid, visitedPipes);
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

    private static char GetStartPipe((int x, int y) start, (int x, int y) first, (int x, int y) second)
    {
        var pipeDirs = new List<Direction>
        {
            GetPipeDirection(start, first),
            GetPipeDirection(start, second)
        }.OrderBy(x => x).ToArray();

        return (pipeDirs[0], pipeDirs[1]) switch
        {
            (Direction.Up, Direction.Right) => 'L',
            (Direction.Up, Direction.Down) => '|',
            (Direction.Up, Direction.Left) => 'J',
            (Direction.Right, Direction.Down) => 'F',
            (Direction.Right, Direction.Left) => '-',
            (Direction.Down, Direction.Left) => '7',
            _ => throw new Exception("Failed to find start pipe")
        };
    }

    private static Direction GetPipeDirection((int x, int y) start, (int x, int y) pipeLoc)
    {
        var (dx, dy) = (pipeLoc.x - start.x, pipeLoc.y - start.y);
        return (dx, dy) switch
        {
            (-1, 0) => Direction.Left,
            (1, 0) => Direction.Right,
            (0, -1) => Direction.Up,
            (0, 1) => Direction.Down,
            _ => throw new Exception("Unknown pipe direction")
        }; ;
    }

    private static int GetTilesEnclosedByLoop(char[,] grid, HashSet<(int x, int y)> visitedPipes)
    {
        var insideCount = 0;
        var isInside = false;
        char? startCorner = null;
        
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                var tile = grid[x, y];
                if (!visitedPipes.Contains((x, y)))
                {
                    if (isInside)
                    {
                        insideCount++;
                    }
                }
                else if (tile == '|' || (startCorner == 'L' && tile == '7') || (startCorner == 'F' && tile == 'J'))
                {
                    isInside = !isInside;
                    startCorner = null;
                }
                else if ("L7FJ".Contains(tile))
                {
                    startCorner = startCorner == null ? tile : null;
                }
            }
        }

        return insideCount;
    }
}

