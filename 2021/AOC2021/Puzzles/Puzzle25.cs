using AOC2021.Helpers;

namespace AOC2021.Puzzles
{
    internal class Puzzle25 : Puzzle<int>
    {
        protected override void Solve(string[] lines)
        {
            var grid = Grid.CreateGrid(lines, c => c);

            bool isMoving = true;
            var i = 1;
            while (isMoving = Move(grid))
            {
                i++;
            }

            One = i;
        }

        private static bool Move(char[,] grid)
        {
            return Move(grid, true) | Move(grid, false);
        }

        private static bool Move(char[,] grid, bool right)
        {
            var didMove = false;

            foreach (var (x,y) in Grid.Iterate(grid))
            {
                if (grid[x, y] == (right ? '>' : 'v'))
                {
                    var (nx, ny) = right
                        ? ((x + 1) % grid.GetLength(0), y)
                        : (x, (y + 1) % grid.GetLength(1));

                    if (grid[nx, ny] == '.')
                    {
                        grid[nx, ny] = 'x';
                        grid[x, y] = 'o';
                        didMove = true;
                    }
                }
            }

            if (didMove)
            {
                foreach (var (x, y) in Grid.Iterate(grid))
                {
                    if (grid[x, y] == 'o')
                    {
                        grid[x, y] = '.';
                    }
                    if (grid[x, y] == 'x')
                    {
                        grid[x, y] = right ? '>' : 'v';
                    }
                }
            }

            return didMove;
        }
    }
}
