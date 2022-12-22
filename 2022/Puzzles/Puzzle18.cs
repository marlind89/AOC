namespace AOC2022.Puzzles;

internal class Puzzle18 : Puzzle<int>
{
    private static readonly Point[] Neighbours = new Point[]
    {
        new(-1 ,  0,  0),
        new(0  , -1,  0),
        new(0  ,  0, -1),
        new(1  ,  0,  0),
        new(0  ,  1,  0),
        new(0  ,  0,  1)
    };

    record struct Point(int X, int Y, int Z)
    {
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    protected override void Solve(string[] lines)
    {
        var points = lines
            .Select(line =>
            {
                var l = line.Split(',').Select(int.Parse).ToArray();
                return new Point(l[0], l[1], l[2]);
            })
            .ToHashSet();

        One = GetSurfaceArea(points);

        var xMin = points.Min(x => x.X) + 1;
        var xMax = points.Max(x => x.X);
        var yMin = points.Min(x => x.Y) + 1;
        var yMax = points.Max(x => x.Y);
        var zMin = points.Min(x => x.Z) + 1;
        var zMax = points.Max(x => x.Z);

        var airPockets = Enumerable.Range(xMin, xMax - xMin)
            .SelectMany(x => Enumerable.Range(yMin, yMax - yMin)
                .SelectMany(y => Enumerable.Range(zMin, zMax - zMin)
                    .Select(z => new Point(x, y, z))))
            .Where(a => !points.Contains(a) 
                && points.Any(b => a.Y == b.Y && a.Z == b.Z && a.X > b.X)
                && points.Any(b => a.Y == b.Y && a.Z == b.Z && a.X < b.X)
                && points.Any(b => a.X == b.X && a.Y == b.Y && a.Z < b.Z)
                && points.Any(b => a.X == b.X && a.Y == b.Y && a.Z > b.Z)
                && points.Any(b => a.X == b.X && a.Z == b.Z && a.Y < b.Y)
                && points.Any(b => a.X == b.X && a.Z == b.Z && a.Y > b.Y))
            .ToHashSet();

        
        Two = One - GetSurfaceArea(airPockets);
    }

    private static int GetSurfaceArea(IReadOnlyCollection<Point> points) => points
        .Sum(p => Neighbours.Select(n => n + p).Count(n => !points.Contains(n)));
}
