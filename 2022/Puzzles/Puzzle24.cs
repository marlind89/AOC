namespace AOC2022.Puzzles;

internal class Puzzle24 : Puzzle<int>
{
    record struct Move(char Direction, (int x, int y) Pos);
    record struct State((int x, int y) MyPos, int Tick, int Steps)
    {
        public IEnumerable<State> GetNextStates(byte[][,] grids)
        {
            var tick = (Tick + 1) % grids.Length;
            var valley = grids[tick];

            if (valley[MyPos.x, MyPos.y] == 0)
            {
                yield return new State(MyPos, tick, Steps + 1);
            }

            foreach (var n in Grid.GetNeighbours(valley, MyPos, false))
            {
                if (valley[n.x, n.y] == 0)
                {
                    yield return new State(n, tick, Steps + 1);
                }
            }
        }

        public int GetPotentialBestSteps((int x, int y) target) => 
            Steps + ManhattanDistance(MyPos, target) + 1;
    }

    protected override void Solve(string[] lines)
    {
        var grids = CreateGrids(lines);
        var startGrid = grids[0];

        var start = (x: 1, y: 0);
        var target = (x: startGrid.GetLength(0) - 2, y: startGrid.GetLength(1) - 1);

        var first  = WalkThroughValley(grids, 0,      start, target);
        var second = WalkThroughValley(grids, first,  target, start);
        var third  = WalkThroughValley(grids, first + second, start, target);
        One = first;
        Two = first + second + third;
    }

    private static int WalkThroughValley(byte[][,] grids, int tick, (int x, int y) start, (int x, int y) target)
    {
        var queue = new PriorityQueue<State, int>();
        queue.Enqueue(new State(start, tick, 0), 0);
        var seen = new HashSet<((int x, int y) pos, int tick)>();

        while (queue.TryDequeue(out var curr, out var steps))
        {
            if (!seen.Add((curr.MyPos, curr.Tick)))
            {
                continue;
            }

            if (curr.MyPos == target)
            {
                return curr.Steps;
            }
            
            queue.EnqueueRange(curr.GetNextStates(grids)
                .Select(curr => (curr, curr.GetPotentialBestSteps(target))));
        }

        return 0;
    }

    private static byte[][,] CreateGrids(string[] lines)
    {
        var valley = Grid.CreateGrid(lines, c => (byte)(c switch
        {
            '>' => 64,
            'v' => 32,
            '<' => 16,
            '^' => 8,
            '#' => 4,
            '.' => 0,
            _ => 0,
        }));

        var currValley = valley;
        var res = Enumerable.Range(1, Lcm(valley.GetLength(0) - 2, valley.GetLength(1) - 2) - 1)
            .Select(tick =>
            {
                currValley = (byte[,])currValley.Clone();
                var newPositions = Grid.Iterate(currValley)
                    .SelectMany(p =>
                    {
                        var val = currValley[p.x, p.y];

                        var moves = new List<Move>();
                        if ((val & (1 << 6)) != 0)
                        {
                            var x = p.x + 1;
                            if ((currValley[x, p.y] & (1 << 2)) != 0)
                            {
                                x = 1;
                            }
                            moves.Add(new Move('>', (x, p.y)));
                        }
                        if ((val & (1 << 5)) != 0)
                        {
                            var y = p.y + 1;
                            if ((currValley[p.x, y] & (1 << 2)) != 0)
                            {
                                y = 1;
                            }
                            moves.Add(new Move('v', (p.x, y)));
                        }
                        if ((val & (1 << 4)) != 0)
                        {
                            var x = p.x - 1;
                            if ((currValley[x, p.y] & (1 << 2)) != 0)
                            {
                                x = currValley.GetLength(0) - 2;
                            }

                            moves.Add(new Move('<', (x, p.y)));
                        }
                        if ((val & (1 << 3)) != 0)
                        {
                            var y = p.y - 1;
                            if ((currValley[p.x, y] & (1 << 2)) != 0)
                            {
                                y = currValley.GetLength(1) - 2;
                            }
                            moves.Add(new Move('^', (p.x, y)));
                        }

                        return moves;
                    })
                    .ToList();

                foreach (var p in Grid.Iterate(currValley)
                    .Where(p => currValley[p.x, p.y] is > 4 and < 128))
                {
                    currValley[p.x, p.y] = 0;
                }

                foreach (var move in newPositions)
                {
                    currValley[move.Pos.x, move.Pos.y] |= move.Direction switch
                    {
                        '>' => 64,
                        'v' => 32,
                        '<' => 16,
                        '^' => 8,
                        _ => 0,
                    };
                }
                
                return currValley;
            })
            .Prepend(valley) // <-- Tick 0
            .ToArray();

        return res;
    }

    private static int ManhattanDistance((int x, int y) first, (int x, int y) second) =>
       Math.Abs(first.x - second.x) + Math.Abs(first.y - second.y);

    private static int Lcm(int a, int b)
    {
        static int Gfc(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        return (a / Gfc(a, b)) * b;
    }
}
