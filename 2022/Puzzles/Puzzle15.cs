using System.Text.RegularExpressions;

namespace AOC2022.Puzzles;

internal partial class Puzzle15 : Puzzle<int, long>
{
    [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
    private static partial Regex Regex();

    record struct Pair((int x, int y) Sensor, (int x, int y) Beacon)
    {
        public int Distance { get; } = ManhattanDistance(Sensor, Beacon);
    }

    protected override void Solve(string[] lines)
    {
        var beacons = lines
            .Select(x => Regex().Match(x).Groups.Values.Skip(1)
                .Select(x => int.Parse(x.Value)).ToList())
            .Select(x => new Pair((x: x[0], y: x[1]), (x: x[2], y: x[3])))
            .ToList();

        var (from, to) = beacons
            .Aggregate((0, 0), (a, b) =>
            {
                var from = Math.Min(a.Item1, b.Beacon.x - b.Distance);
                from = Math.Min(from, b.Sensor.x - b.Distance);

                var to = Math.Max(a.Item2, b.Beacon.x + b.Distance);
                to = Math.Max(to, b.Sensor.x + b.Distance);

                return (from, to);
            });

        One = ParallelEnumerable.Range(from, to - from)
            .Count(x => CheckForBeacon(beacons, (x, 2000000)));

        Two = FindBeacon(beacons);
    }

    private static long FindBeacon(IEnumerable<Pair> beacons)
    {
        var (x, y) = beacons
            .SelectMany(x => Circle(x.Sensor, x.Distance))
            .FirstOrDefault(x => x is { x: >= 0 and <= 4000000, y: >= 0 and <= 4000000 } &&
                !CheckForBeacon(beacons, x));

        return x * 4000000L + y;
    }

    private static bool CheckForBeacon(IEnumerable<Pair> pairs, (int x, int y) location) => 
        pairs.Any(p => p.Beacon != location && ManhattanDistance(location, p.Sensor) <= p.Distance);

    private static IEnumerable<(int x, int y)> Circle((int x, int y) sensor, int distance)
    {
        var fromY = sensor.y - distance - 1;
        var toY = sensor.y + distance + 2;

        foreach (var y in Enumerable.Range(fromY, Math.Abs(fromY - toY)))
        {
            var remainder = distance - Math.Abs(sensor.y - y) + 1;
            yield return (sensor.x - remainder, y);
            if (remainder > 0)
            {
                yield return (sensor.x + remainder, y);
            }
        }
    }

    private static int ManhattanDistance((int x, int y) first, (int x, int y) second) =>
        Math.Abs(first.x - second.x) + Math.Abs(first.y - second.y);
}
