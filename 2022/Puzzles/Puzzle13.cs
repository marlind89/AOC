namespace AOC2022.Puzzles;

internal class Puzzle13 : Puzzle<int>
{
    private static readonly string[] DividerPackets = new string[] { "[[2]]", "[[6]]"};

    record Signal(int Value, List<Signal> Children)
    {
        public override string ToString() => Value >= 0 ? Value.ToString() : $"[{string.Join(",", Children)}]";
    }

    protected override void Solve(string[] lines)
    {
        var comparer = new SignalComparer();
        var signals = lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(Parse).ToList();

        One = signals
            .Chunk(2)
            .Select((x, idx) => (first: x[0], second: x[1], idx: idx + 1))
            .Sum(x => comparer.Compare(x.first, x.second) == -1 ? x.idx : 0);

        Two = signals
            .Concat(DividerPackets.Select(Parse))
            .OrderBy(x => x, comparer)
            .Select((signal, idx) => (signal, idx: idx + 1))
            .Where(x => DividerPackets.Contains(x.signal.ToString()))
            .Aggregate(1, (a, b) => a * b.idx);
    }

    private static Signal Parse(string signal)
    {
        var stack = new Stack<Signal>();
        var brackets = new char[] { '[', ']' };
        for (var i = 0; i < signal.Length; i++)
        {
            var s = signal[i];
            if (s == '[')
            {
                stack.Push(new(-1, new()));
            }
            else if (s == ']')
            {
                var currSignal = stack.Pop();
                if (!stack.TryPeek(out var parent))
                {
                    return currSignal;
                }
                parent.Children.Add(currSignal);
            }
            else
            {
                var remainingSignal = signal[i..];
                var toIdx = remainingSignal.IndexOfAny(brackets);
                var values = remainingSignal[..toIdx]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new Signal(int.Parse(s), new()))
                    .ToList();

                if (values.Count > 0)
                {
                    stack.Peek().Children.AddRange(values);
                    i += toIdx - 1;
                }
            }
        }

        throw new Exception("Invalid signal");
    }

    private static (Signal first, Signal second) Normalize(Signal first, Signal second) => (first.Value, second.Value) switch
    {
        (-1, >= 0) => (first, second with { Children = new() { new(second.Value, new()) }, Value = -1 }),
        (>= 0, -1) => (first with { Children = new() { new(first.Value, new()) }, Value = -1 }, second),
        _          => (first, second)
    };

    class SignalComparer : IComparer<Signal>
    {
        public int Compare(Signal? x, Signal? y)
        {
            var (first, second) = Normalize(x!, y!);

            return first.Value >= 0 && second.Value >= 0
                ? first.Value.CompareTo(second.Value)
                : first.Children.Zip(second.Children)
                    .Select(x => Compare(x.First, x.Second))
                    .FirstOrDefault(x => x != 0, first.Children.Count.CompareTo(second.Children.Count));
        }
    }
}
