using AOC2021.Helpers;

namespace AOC2021.Puzzles
{
    internal class Puzzle11 : Puzzle<int>
    {
        private int[,] _grid = default!;

        protected override void Solve(string[] lines)
        {
            _grid = Grid.CreateGrid(lines);

            int step = 0, totalFlashCount = 0;
            while (true)
            {
                step++;
                var flashedSquids = new HashSet<(int x, int y)>();
                var flashCount = Grid.Iterate(_grid).Sum(c => Flash(c, flashedSquids));
                if (step <= 100)
                {
                    totalFlashCount += flashCount;
                }
                if (flashCount == _grid.Length)
                {
                    break;
                }
            }

            One = totalFlashCount;
            Two = step;
        }

        private int Flash((int x, int y) coord, ISet<(int x, int y)> flashedSquids)
        {
            if (flashedSquids.Contains(coord))
            {
                return 0;
            }

            if (_grid[coord.x, coord.y] < 9)
            {
                _grid[coord.x, coord.y]++;
                return 0;
            }

            _grid[coord.x, coord.y] = 0;
            flashedSquids.Add(coord);

            return 1 + Grid.GetNeighbours(_grid, coord, true)
                .Sum(n => Flash(n, flashedSquids));
        }
    }
}
