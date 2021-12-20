namespace AOC2021.Helpers
{
    internal class Grid
    {
        public static readonly (int x, int y)[] NeighborOffsets = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
        public static readonly (int x, int y)[] NeighborOffsetsWithDiags = new[] { (-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1) };
        public static readonly (int x, int y)[] NeighborOffsetsWithDiagsIncludeSelf = new[] { (-1, -1), (0, -1), (1, -1), (-1, 0), (0, 0), (1, 0), (-1, 1), (0, 1), (1, 1) };

        public static void Fill<T>(T[,] grid, T val)
        {
            foreach (var (x,y) in Iterate(grid))
            {
                grid[x, y] = val;
            }
        }

        public static IEnumerable<(int x, int y)> Iterate<T>(T[,] arr)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                for (int y = 0; y < arr.GetLength(1); y++)
                {
                    yield return (x, y);
                }
            }
        }

        public static int[,] CreateGrid(string[] lines)
        {
            var xLength = lines[0].Length;
            var yLength = lines.Length;

            var grid = new int[xLength, yLength];

            foreach (var (x, y) in Iterate(grid))
            {
                grid[x, y] = lines[y][x] - '0';
            }

            return grid;
        }

        public static T[,] CreateGrid<T>(string[] lines, Func<char, T> transform)
        {
            var xLength = lines[0].Length;
            var yLength = lines.Length;

            var grid = new T[xLength, yLength];

            foreach (var (x, y) in Iterate(grid))
            {
                grid[x, y] = transform(lines[y][x]);
            }

            return grid;
        }

        public static bool IsOutOfRange<T>(T[,] grid, (int x, int y) coord) =>
            coord.x < 0 || coord.x >= grid.GetLength(0) || coord.y < 0 || coord.y >= grid.GetLength(1);

        public static IEnumerable<(int x, int y)> GetNeighbours(int[,] grid, (int x, int y) coord, bool includeDiags) => 
            (includeDiags ? NeighborOffsetsWithDiags : NeighborOffsets)
               .Select(c => (coord.x + c.x, coord.y + c.y))
               .Where(c => !IsOutOfRange(grid, c));
    }
}
