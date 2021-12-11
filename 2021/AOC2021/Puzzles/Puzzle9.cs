using AOC2021.Helpers;

namespace AOC2021.Puzzles
{
    internal class Puzzle9 : Puzzle<int>
    {
        private int[,] _map = default!;

        protected override void Solve(string[] lines)
        {
            _map = Grid.CreateGrid(lines);

            var lowPoints = Grid.Iterate(_map).Where(IsLowPoint).ToArray();

            One = lowPoints.Sum(coord => 1 + _map[coord.x, coord.y]);
            Two = lowPoints
                .Select(c => GetBasinSize(c, new HashSet<(int x, int y)>()))
                .OrderByDescending(x => x)
                .Take(3)
                .Aggregate(1, (a,b) => a * b);
        }

        private bool IsLowPoint((int x, int y) coord) => Grid.NeighborOffsets
            .Select(offset => GetDepth((coord.x + offset.x, coord.y + offset.y), int.MaxValue))
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

            return 1 + Grid.NeighborOffsets
                .Select(offset => new
                {
                    coord = (coord.x + offset.x, coord.y + offset.y),
                    depth = GetDepth((coord.x + offset.x, coord.y + offset.y), -1)
                })
                .Where(x => x.depth > depth)
                .Sum(x => GetBasinSize(x.coord, visitedNodes));
        }

        private int GetDepth((int x, int y) coord, int defaultValue) =>
            Grid.IsOutOfRange(_map, coord) ? defaultValue : _map[coord.x, coord.y];
    }
}
