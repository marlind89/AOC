using System.Text.RegularExpressions;

namespace AOC2022.Puzzles;

internal partial class Puzzle16 : Puzzle<int>
{
    [GeneratedRegex(@".*([A-Z]{2}).*=(\d+);.*valve(?:s?) (.*)")]
    private static partial Regex Regex();

    record struct Valve(string Name, int FlowRate, List<Valve> ConnectedValves);

    record struct Cave(string OpenedValves, string MyPosition, string ElephantPosition, int MinutesLeft, int TotalPressure)
    {
        public int CalcPressure(IDictionary<string, Valve> valves) => OpenedValves.Chunk(2).Sum(x => valves[string.Concat(x)].FlowRate);
    }

    protected override void Solve(string[] lines)
    {
        var valves = CreateValves(lines);
        One = GetPressureResult(valves, true, 30);
        Two = GetPressureResult(valves, false, 26);
    }

    private static IDictionary<string, Valve> CreateValves(string[] lines)
    {
        var valves = lines
          .Select(line =>
          {
              var groups = Regex().Match(line).Groups;
              return new Valve(groups[1].Value, int.Parse(groups[2].Value), new());
          })
          .ToDictionary(x => x.Name);

        foreach (var line in lines)
        {
            var match = Regex().Match(line);
            var valveId = match.Groups[1].Value;
            var connections = match.Groups[3].Value.Split(", ").ToList();
            valves[valveId].ConnectedValves.AddRange(connections.Select(x => valves[x]));
        }

        return valves;
    }

    private static int GetPressureResult(IDictionary<string, Valve> valves, bool soloPlay, int minutes)
    {
        IEnumerable<Cave> CreateNewCaves(Cave cave, Valve myPos, string myPosition, Func<Cave> open, Func<Valve, Cave> move) => 
            Enumerable.Range(0, 2)
                .Select(x => x % 2 == 0)
                .Where(shouldOpen => !shouldOpen || (myPos.FlowRate > 0 && !cave.OpenedValves.Contains(myPosition)))
                .SelectMany(shouldOpen => shouldOpen
                    ? new List<Cave>() { open() }
                    : valves.Values
                        .Where(valve => myPos.ConnectedValves.Contains(valve))
                        .Select(move));

        var caves = new PriorityQueue<Cave, Cave>(new CaveComparer());
        var start = new Cave("", "AA", "AA", minutes, 0);
        caves.Enqueue(start, start);
        var valvesWithFlowRate = valves.Values.Count(x => x.FlowRate > 0) * 2;

        while (true)
        {
            var cavesToVisit = Enumerable.Range(0, Math.Min(1000, caves.Count)).Select(_ => caves.Dequeue()).ToList();

            var next = cavesToVisit
                .AsParallel()
                .SelectMany(cave =>
                {
                    if (cave.OpenedValves.Length == valvesWithFlowRate)
                    {
                        return new List<Cave>
                        {
                            cave with
                            {
                                MinutesLeft = 0,
                                TotalPressure = cave.TotalPressure + cave.CalcPressure(valves) * cave.MinutesLeft
                            }
                        };
                    }

                    var result = CreateNewCaves(cave, valves[cave.MyPosition], cave.MyPosition,
                        () => cave with
                        {
                            MinutesLeft = cave.MinutesLeft - 1,
                            OpenedValves = cave.OpenedValves + cave.MyPosition,
                            TotalPressure = cave.TotalPressure + cave.CalcPressure(valves),
                        }, 
                        valve => cave with
                        {
                            MinutesLeft = cave.MinutesLeft - 1,
                            MyPosition = valve.Name,
                            TotalPressure = cave.TotalPressure + cave.CalcPressure(valves),
                        });

                    if (!soloPlay)
                    {
                        var elephant = valves[cave.ElephantPosition];
                        result = result.SelectMany(c => CreateNewCaves(c, elephant, cave.ElephantPosition,
                            () => c with { OpenedValves = c.OpenedValves + c.ElephantPosition },
                            valve => c with { ElephantPosition = valve.Name }));
                    }

                    return result;
                })
                .ToHashSet();

            var completed = next
                .Where(x => x.MinutesLeft <= 0)
                .ToList();

            if (completed.Count > 0)
            {
                return completed.Max(x => x.TotalPressure);
            }

            caves.EnqueueRange(next.Select(x => (x, x)));
        }
    }

    class CaveComparer : IComparer<Cave>
    {
        public int Compare(Cave x, Cave y)
        {
            return -1 * x.TotalPressure.CompareTo(y.TotalPressure);
        }
    }
}
