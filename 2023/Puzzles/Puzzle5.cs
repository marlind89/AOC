namespace AOC2023.Puzzles;

internal class Puzzle5 : Puzzle<long>
{
    record SeedMap(List<SeedRange> Ranges)
    {
        public long GetSeedDestination(long seed)
        {
            var range = Ranges.FirstOrDefault(x => x.InRangeDest(seed));
            return seed + (range?.Delta ?? 0);
        }

        public IEnumerable<(long From, long Length)> GetSeedDestinations((long From, long Length) seedRange)
        {
            var (sFrom, sLength, sTo) = (seedRange.From, seedRange.Length, seedRange.From + seedRange.Length - 1);
            var seenRanges = new List<(long From, long To)>();
            foreach (var range in Ranges)
            {
                var (dFrom, dLength, dTo) = (range.Source, range.Length, range.Source + range.Length - 1);
                if (dFrom <= sFrom && dTo >= sTo)
                {
                    yield return (sFrom + range.Delta, sLength);
                    yield break;
                }
                if (dFrom >= sFrom && dTo <= sTo)
                {
                    seenRanges.Add((dFrom, dTo));
                    yield return (dFrom + range.Delta, dLength);
                }
                else if (dFrom <= sFrom && sFrom <= dTo)
                {
                    seenRanges.Add((sFrom, dTo));
                    yield return (sFrom + range.Delta, 1 + dTo - sFrom);
                }
                else if (dFrom >= sFrom && dFrom <= sTo)
                {
                    seenRanges.Add((dFrom, sTo));
                    yield return (dFrom + range.Delta, 1 +  sTo - dFrom);
                }
            }

            long currentStart = sFrom;
            foreach (var (From, To) in seenRanges.OrderBy(x => x.From))
            {
                if (From > currentStart)
                {
                    yield return (currentStart, 1 + To - currentStart);
                }
                currentStart = To + 1;
            }
            if (currentStart < sTo)
            {
                yield return (currentStart, 1 + sTo - currentStart);
            }
        }
    }

    record SeedRange(long Source, long Destination, long Length)
    {
        public bool InRangeDest(long seed) => seed >= Source && seed <= (Source + Length);
        public long Delta => Destination - Source;
    }

    protected override void Solve(string[] lines)
    {
        lines = lines.Where(x => x != "").ToArray();

        var seeds = lines[0][(1 + lines[0].IndexOf(':'))..]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList();

        var seedMapGroups = ParseSeedMaps(lines);

        One = GetLowestLocation(seeds, seedMapGroups);
        
        var seedRanges = seeds.Chunk(2).Select(x => (From: x[0], Length: x[1]));
        Two = GetLowestLocation(seedRanges, seedMapGroups);
    }

    private static List<SeedMap> ParseSeedMaps(string[] lines)
    {
        var mapIdxes = lines
            .Append(" map:")
            .Select((x, i) => x.EndsWith(" map:") ? i : -1)
            .Where(x => x != -1)
            .ToArray();

        return mapIdxes.Zip(mapIdxes.Skip(1))
            .Select(x => new SeedMap(Enumerable.Range(x.First + 1, x.Second - x.First - 1)
                .Select(idx =>
                {
                    var values = lines[idx].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                    return new SeedRange(values[1], values[0], values[2]);
                })
                .ToList()
            ))
            .ToList();
    }

    private static long GetLowestLocation(List<long> seeds, List<SeedMap> seedMapGroups) => 
        seedMapGroups.Aggregate(seeds, 
            (seeds, seedMapGroup) => seeds.Select(seedMapGroup.GetSeedDestination).ToList(),
            seeds => seeds.Min());

    private static long GetLowestLocation(IEnumerable<(long From, long Length)> seedRanges, List<SeedMap> seedMapGroups) => 
        seedMapGroups.Aggregate(seedRanges,
            (seeds, seedMap) => seeds.SelectMany(seedMap.GetSeedDestinations).ToList(),
            seeds => seeds.Min(x => x.From));
}
