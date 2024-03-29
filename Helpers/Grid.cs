﻿namespace AOC.Helpers;

public class Grid
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

    public static IEnumerable<(int x, int y)> Iterate<T>(T[,] arr, int startY = 0)
    {
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = startY; y < arr.GetLength(1); y++)
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

    public static T[,] CreateGrid<T>(string[] lines, Func<char, T> transform, char? oobDefaultValue = null)
    {
        var xLength = lines[0].Length;
        var yLength = lines.Length;

        var grid = new T[xLength, yLength];

        foreach (var (x, y) in Iterate(grid))
        {
            var line = lines[y];
            var cellVal = oobDefaultValue == null || line.Length > x ? line[x] : oobDefaultValue.Value;
            grid[x, y] = transform(cellVal);
        }

        return grid;
    }

    public static bool IsOutOfRange<T>(T[,] grid, (int x, int y) coord) =>
        coord.x < 0 || coord.x >= grid.GetLength(0) || coord.y < 0 || coord.y >= grid.GetLength(1);

    public static IEnumerable<(int x, int y)> GetNeighbours<T>(T[,] grid, (int x, int y) coord, bool includeDiags) => 
        (includeDiags ? NeighborOffsetsWithDiags : NeighborOffsets)
           .Select(c => (coord.x + c.x, coord.y + c.y))
           .Where(c => !IsOutOfRange(grid, c));

    public static IEnumerable<(int x, int y)> IterateBetween((int x, int y) first, (int x, int y) second)
    {
        return IterateBetween(first.x, second.x)
            .SelectMany(x => IterateBetween(first.y, second.y).Select(y => (x, y)));
    }

    public static void Print<T>(T[,] arr, Func<T, string>? transform = null)
    {
        transform ??= x => x?.ToString() ?? "";
        for (int y = 0; y < arr.GetLength(1); y++)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                Console.Write(transform(arr[x, y]));
            }
            Console.WriteLine();
        }
    }

    public static (int x, int y) FindFirstLocation<T>(T[,] arr, T val)
    {
        for (int y = 0; y < arr.GetLength(1); y++)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                if (arr[x, y]?.Equals(val) ?? false)
                {
                    return (x, y);
                }
            }
        }

        throw new Exception("Failed to find location");
    }

    public static T[,] Slice<T>(T[,] arr, (int x, int y) first, (int x, int y) second)
    {
        var grid = new T[Math.Abs(second.x - first.x) + 1, Math.Abs(second.y - first.y) + 1];

        var xMin = Math.Min(first.x, second.x);
        var yMin = Math.Min(first.y, second.y);
        foreach (var (x, y) in IterateBetween(first, second))
        {
            grid[x - xMin, y - yMin] = arr[x, y];
        }
        return grid;
    }

    private static IEnumerable<int> IterateBetween(int first, int second)
    {
        return Enumerable.Range(Math.Min(first, second), Math.Abs(first - second) + 1);
    }

    public static int ManhattanDistance((int x, int y) first, (int x, int y) second) =>
       Math.Abs(first.x - second.x) + Math.Abs(first.y - second.y);
}
