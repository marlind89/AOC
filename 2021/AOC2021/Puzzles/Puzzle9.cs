namespace AOC2021.Puzzles
{
    internal class Puzzle9 : Puzzle<int>
    {
        private static readonly (int x, int y)[] NeighborOffsets = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

        private int _xLength;
        private int _yLength;
        private int[,] _map = default!;

        protected override void Solve(string[] lines)
        {
            _xLength = lines[0].Length;
            _yLength = lines.Length;
            _map = new int[_xLength, _yLength];

            foreach (var (x, y) in Iterate(_map))
            {
                _map[x, y] = lines[y][x] - '0';
            }

            var lowPoints = Iterate(_map).Where(IsLowPoint).ToArray();

            One = lowPoints.Sum(coord => 1 + _map[coord.x, coord.y]);
            Two = lowPoints
                .Select(c => GetBasinSize(c, new HashSet<(int x, int y)>()))
                .OrderByDescending(x => x)
                .Take(3)
                .Aggregate(1, (a,b) => a * b);
        }

        private bool IsLowPoint((int x, int y) coord) => NeighborOffsets
            .Select(offset => GetDepth(coord.x + offset.x, coord.y + offset.y, int.MaxValue))
            .All(n => n > _map[coord.x, coord.y]);

        private int GetBasinSize((int x, int y) coord, ISet<(int x, int y)> visitedNodes)
        {
            if (!visitedNodes.Add(coord))
            {
                return 0;
            }

            var depth = _map[coord.x, coord.y];
            if (depth == 9)
            {
                return 0;
            }

            return 1 + NeighborOffsets
                .Select(offset => new
                {
                    coord = (coord.x + offset.x, coord.y + offset.y),
                    depth = GetDepth(coord.x + offset.x, coord.y + offset.y, -1)
                })
                .Where(x => x.depth > depth)
                .Sum(x => GetBasinSize(x.coord, visitedNodes));
        }

        private int GetDepth(int x, int y, int defaultValue) => IsOutOfRange(x, y) ? defaultValue : _map[x, y];

        private bool IsOutOfRange(int x, int y) => x < 0 || x >= _xLength || y < 0 || y >= _yLength;

        private static IEnumerable<(int x, int y)> Iterate(int[,] arr)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                for (int y = 0; y < arr.GetLength(1); y++)
                {
                    yield return (x, y);
                }
            }
        }
    }
}
