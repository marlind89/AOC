namespace AOC2021.Puzzles;

internal class Puzzle19 : Puzzle<int>
{
    public record Position(int X, int Y, int Z)
    {
        public int Distance(Position other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);

        public Position Vector(Position other) => new(other.X - X, other.Y - Y, other.Z - Z);

        internal Position Translate(Position translation) => new(X + translation.X, Y + translation.Y, Z + translation.Z);
    }
    
    private Dictionary<int, Position> _scanners = default!;

    protected override void Solve(string[] lines)
    {
        var scannerReadings = new Dictionary<int, IReadOnlyCollection<Position>>();

        List<Position>? readings = null;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("---"))
            {
                readings = new List<Position>();
                scannerReadings[int.Parse(line.Split(' ')[2])] = readings;
            }
            else
            {
                var coords = line.Split(",").Select(int.Parse).ToArray();
                readings?.Add(new Position(coords[0], coords[1], coords[2]));
            }
        }

        var beacons = new HashSet<Position>(scannerReadings[0]);
        _scanners = new()
        {
            [0] = new Position(0, 0, 0)
        };

        One = FindBeacons(scannerReadings, beacons);
        Two = GetMaxDistance();
    }

    private int FindBeacons(IDictionary<int, IReadOnlyCollection<Position>> scannerPositions,
        HashSet<Position> beacons)
    {
        var vectors = GetDistances(beacons);

        var scannersToCheck = new Queue<int>();
        for (int i = 1; i < scannerPositions.Count; i++)
        {
            scannersToCheck.Enqueue(i);
        }

        while (scannersToCheck.Count > 0)
        {
            int scanner = scannersToCheck.Dequeue();
            var readings = scannerPositions[scanner];

            Func<Position, Position>? scannerRotation = null;
            Position? translation = null;
            foreach (var rotateFunc in GetRotations())
            {
                if (TryRotation(vectors, readings, rotateFunc, out translation))
                {
                    scannerRotation = rotateFunc;
                    break;
                }
            }

            if (scannerRotation != null && translation != null)
            {
                foreach (var beacon in readings
                    .Select(scannerRotation)
                    .Select(b => b.Translate(translation)))
                {
                    beacons.Add(beacon);
                }

                vectors = GetDistances(beacons);

                _scanners.Add(scanner, translation);
            }
            else
            {
                scannersToCheck.Enqueue(scanner);
            }
        }

        return beacons.Count;
    }

    private int GetMaxDistance()
    {
        var tested = new HashSet<(int, int)>();
        int maxDistance = 0;

        foreach (var (scannerId, scannerFrom) in _scanners)
        {
            foreach (var (scannerToId, scannerTo) in _scanners)
            {
                if (scannerId == scannerToId)
                {
                    continue;
                }

                var key1 = (scannerId, scannerToId);
                var key2 = (scannerToId, scannerId);

                if (tested.Contains(key1) || tested.Contains(key2))
                {
                    continue;
                }

                maxDistance = Math.Max(maxDistance, scannerFrom.Distance(scannerTo));

                tested.Add(key1);
            }
        }

        return maxDistance;
    }

    private static Dictionary<Position, Position> GetDistances(IReadOnlyCollection<Position> beacons) => beacons
        .SelectMany(a => beacons.Select(b => new[] { a, b }))
        .Where(a => a[0] != a[1])
        .ToDictionary(x => x[1].Vector(x[0]), x => x[1]);

    private static bool TryRotation(Dictionary<Position, Position> vectors, 
        IReadOnlyCollection<Position> beacons, Func<Position, Position> rotate, out Position? translation)
    {
        int matchCount = 0;
        foreach (var p1 in beacons)
        {
            var p1Rotated = rotate(p1);
            foreach (var p2 in beacons)
            {
                if (p1 == p2)
                {
                    continue;
                }

                var p2Rotated = rotate(p2);
                var vector = p1Rotated.Vector(p2Rotated);

                if (vectors.ContainsKey(vector) && ++matchCount == 11)
                {
                    translation = p1Rotated.Vector(vectors[vector]);
                    return true;
                }
            }
        }

        translation = null;
        return false;
    }

    private static IEnumerable<Func<Position, Position>> GetRotations()
    {
        yield return v => new(v.X, v.Y, v.Z);
        yield return v => new(v.X, -v.Z, v.Y);
        yield return v => new(v.X, -v.Y, -v.Z);
        yield return v => new(v.X, v.Z, -v.Y);

        yield return v => new(-v.Y, v.X, v.Z);
        yield return v => new(v.Z, v.X, v.Y);
        yield return v => new(v.Y, v.X, -v.Z);
        yield return v => new(-v.Z, v.X, -v.Y);

        yield return v => new(-v.X, -v.Y, v.Z);
        yield return v => new(-v.X, -v.Z, -v.Y);
        yield return v => new(-v.X, v.Y, -v.Z);
        yield return v => new(-v.X, v.Z, v.Y);

        yield return v => new(v.Y, -v.X, v.Z);
        yield return v => new(v.Z, -v.X, -v.Y);
        yield return v => new(-v.Y, -v.X, -v.Z);
        yield return v => new(-v.Z, -v.X, v.Y);

        yield return v => new(-v.Z, v.Y, v.X);
        yield return v => new(v.Y, v.Z, v.X);
        yield return v => new(v.Z, -v.Y, v.X);
        yield return v => new(-v.Y, -v.Z, v.X);

        yield return v => new(-v.Z, -v.Y, -v.X);
        yield return v => new(-v.Y, v.Z, -v.X);
        yield return v => new(v.Z, v.Y, -v.X);
        yield return v => new(v.Y, -v.Z, -v.X);
    }
}

