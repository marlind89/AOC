namespace AOC2022.Puzzles;

internal class Puzzle14 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        var rockPaths = lines.Select(GetPath).ToList();
        var cave = GenerateCave(rockPaths);
        One = DropSands(cave);

        rockPaths.Add(GetPath($"0,{cave.GetLength(1)} -> 666,{cave.GetLength(1)}"));
        cave = GenerateCave(rockPaths);
        Two = DropSands(cave) + 1;
    }

    private static IEnumerable<(int x, int y)> GetPath(string line) => line.Split(" -> ")
        .Select(path =>
        {
            var coords = path.Split(",").Select(int.Parse).ToList();
            return (x: coords[0], y: coords[1]);
        });

    private static bool[,] GenerateCave(List<IEnumerable<(int x, int y)>> rockPaths)
    {
        var (width, depth) = rockPaths
            .SelectMany(x => x)
            .Aggregate((w: 0, d: 0), (max, cur) => 
                (Math.Max(cur.x, max.w), Math.Max(cur.y, max.d)), x => (x.w + 1, x.d + 2));

        return rockPaths
            .SelectMany(paths => paths.Zip(paths.Skip(1)))
            .SelectMany(x => Grid.IterateBetween(x.First, x.Second))
            .Aggregate(new bool[width, depth], (a, b) => 
            {
                a[b.x, b.y] = true;
                return a;
            });
    }

    private static int DropSands(bool[,] cave)
    {
        var startPos = (x: 500, y: 0);
        var sandAmount = 0;
        while (!DropSand(cave, startPos)) sandAmount++;
        return sandAmount;
    }

    private static bool DropSand(bool[,] cave, (int x, int y) startPos)
    {
        var sandPos = startPos;
        while (true)
        {
            if (Grid.IsOutOfRange(cave, (sandPos.x, sandPos.y + 1)))
            {
                return true;
            }

            var newPos = GetNextPosition(cave, sandPos.x, sandPos.y);
            if (newPos == sandPos)
            {
                cave[newPos.x, newPos.y] = true;
                return newPos == startPos;
            }

            sandPos = newPos;
        }
    }

    private static (int x, int y) GetNextPosition(bool[,] cave, int sx, int sy) => cave switch
    {
        _ when !cave[sx, sy + 1]     => (sx, sy + 1),
        _ when !cave[sx - 1, sy +1]  => (sx - 1, sy + 1),
        _ when !cave[sx + 1, sy + 1] => (sx + 1, sy + 1),
        _                            => (sx, sy)
    };
}
