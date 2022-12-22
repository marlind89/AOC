namespace AOC2022.Puzzles;

internal class Puzzle19 : Puzzle<int>
{
    private static readonly Materials Ore = new (1, 0, 0, 0);
    private static readonly Materials Clay = new (0, 1, 0, 0);
    private static readonly Materials Obsidian = new (0, 0, 1, 0);
    private static readonly Materials Geode = new (0, 0, 0, 1);

    enum Resource
    {
        Ore,
        Clay,
        Obsidian,
        Geode
    }

    record Cost(Resource Resource, int Amount)
    {
        public Materials ToMaterials() => Resource switch
        {
            Resource.Ore => new Materials(Amount, 0, 0, 0),
            Resource.Clay => new Materials(0, Amount, 0, 0),
            Resource.Obsidian => new Materials(0, 0, Amount, 0),
            Resource.Geode => new Materials(0, 0, 0, Amount),
            _ => throw new InvalidOperationException()
        };
    }

    record struct Blueprint(int Id, Materials OreCost, Materials ClayCost, Materials ObsidianCost, Materials GeodeCost)
    {
        public (Materials Robot, Materials Cost)[] Robots = new[]
        {
            (Geode, GeodeCost),
            (Obsidian, ObsidianCost),
            (Clay, ClayCost),
            (Ore, OreCost)
        };
    }
    
    record struct Materials(int Ore, int Clay, int Obsidian, int Geode)
    {
        public static bool operator >=(Materials a, Materials b) => a.Ore >= b.Ore && a.Clay >= b.Clay && a.Obsidian >= b.Obsidian && a.Geode >= b.Geode;
        public static bool operator <=(Materials a, Materials b) => a.Ore <= b.Ore && a.Clay <= b.Clay && a.Obsidian <= b.Obsidian && a.Geode <= b.Geode;
        public static Materials operator +(Materials a, Materials b) => new(a.Ore + b.Ore, a.Clay + b.Clay, a.Obsidian + b.Obsidian, a.Geode + b.Geode);
        public static Materials operator -(Materials a, Materials b) => new(a.Ore - b.Ore, a.Clay - b.Clay, a.Obsidian - b.Obsidian, a.Geode - b.Geode);
    }
    record struct Fortress(int MinutesRemaining, Materials Robots, Materials Inventory, (Materials Robot, Materials Cost)[]? Ignore)
    {
        public IEnumerable<Fortress> GetNextPossibleStates(Blueprint blueprint, int maxGeodes)
        {
            if (!(Inventory.Geode > (maxGeodes - 2)))
            {
                yield break;
            }

            if (!WorthBuilding(blueprint))
            {
                yield break;
            }

            var inv = Inventory;
            var buildableRobots = blueprint.Robots
                .Where(x => x.Cost <= inv)
                .ToArray();

            yield return this with
            {
                Inventory = Robots + Inventory,
                MinutesRemaining = MinutesRemaining - 1,
                Ignore = buildableRobots
            };

            foreach (var robot in buildableRobots)
            {
                if (Ignore == null || !Ignore.Contains(robot))
                {
                    yield return this with
                    {
                        Robots = Robots + robot.Robot,
                        Inventory = Robots + Inventory - robot.Cost,
                        MinutesRemaining = MinutesRemaining - 1,
                        Ignore = null
                    };

                    if (robot.Robot == Geode)
                    {
                        yield break;
                    }
                }
            }
        }

        public int PotentialGeodeCount()
        {
            var future = (Robots.Geode + Robots.Geode + MinutesRemaining) * MinutesRemaining / 2;
            return Inventory.Geode + future;
        }

        public bool WorthBuilding(Blueprint blueprint)
        {
            return blueprint.GeodeCost.Ore > Robots.Ore ||
                blueprint.GeodeCost.Clay > Robots.Clay ||
                blueprint.GeodeCost.Obsidian > Robots.Obsidian;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MinutesRemaining, Robots, Inventory);
        }
    }

    protected override void Solve(string[] lines)
    {
        var blueprints = lines
            .Select(lines =>
            {
                var blueprintIdx = lines.IndexOf(':');
                var id = int.Parse(lines[10..blueprintIdx]);

                var costs = lines[blueprintIdx..]
                    .Split('.', StringSplitOptions.TrimEntries)
                    .Select(robot => ParseCost(robot).Aggregate(new Materials(0, 0, 0, 0), (a, b) => a + b.ToMaterials()))
                    .ToList();

                return new Blueprint(id, costs[0], costs[1], costs[2], costs[3]);
            })
            .ToList();

        One = blueprints
            .AsParallel()
            .Sum(x => CrackGeodes(x, 24) * x.Id);

        Two = blueprints
            .Where(x => x.Id <= 3)
            .AsParallel()
            .Select(x => CrackGeodes(x, 32))
            .Aggregate(1, (a, b) => a * b);
    }


    private static int CrackGeodes(Blueprint blueprint, int minutes)
    {
        var fortress = new PriorityQueue<Fortress, int>();
        var start = new Fortress(minutes, new Materials(1,0,0,0), new Materials(0,0,0,0), null);
        var seen = new HashSet<Fortress>();
        var max = 0;

        fortress.Enqueue(start, -start.PotentialGeodeCount());
        while (fortress.TryDequeue(out var arsenal, out var prio))
        {
            if (prio * -1 < max)
            {
                break;
            }

            if (seen.Contains(arsenal))
            {
                continue;
            }
            seen.Add(arsenal);

            if (arsenal.MinutesRemaining == 0)
            {
                max = Math.Max(max, arsenal.Inventory.Geode);
            }
            else
            {
                fortress.EnqueueRange(arsenal.GetNextPossibleStates(blueprint, max)
                    .Select(x => (x, -x.PotentialGeodeCount())));
            }
        }
        
        return max;
    }

    private static IEnumerable<Cost> ParseCost(string costStr)
    {
        var parts = costStr.Split(' ').Skip(2).ToList();

        return Enum.GetValues<Resource>()
            .Select(x =>
            {
                var idx = parts.LastIndexOf(x.ToString().ToLower());
                return idx == -1
                    ? null
                    : new Cost(x, int.Parse(parts[idx - 1]));
            })
            .Where(x => x != null)!;
    }
}
