using System.Text.RegularExpressions;

namespace AOC2022.Puzzles;

internal partial class Puzzle11 : Puzzle<long, long>
{
    [GeneratedRegex(@"Monkey (\d+):")] private static partial Regex MonkeyRegex();
    [GeneratedRegex(@"(\*|\+{1})")] private static partial Regex OperationRegex();

    record Monkey(int Id, List<long> Items, Func<long, long> Operation, int DivisibleBy, int TrueMonkey, int FalseMonkey)
    {
        public int Inspection = 0;

        public IEnumerable<(int MonkeyId, long Item)> MakeTurn(decimal worryRelief, int superModulo)
        {
            foreach (var item in Items)
            {
                Inspection++;
                var newWorryLevel = (long) Math.Floor(Operation(item) / worryRelief) % superModulo;
                yield return (newWorryLevel % DivisibleBy == 0 ? TrueMonkey : FalseMonkey, newWorryLevel);
            }

            Items.Clear();
        }
    }
    
    protected override void Solve(string[] lines)
    {
        One = RunMonkeys(lines, 20);
        Two = RunMonkeys(lines, 10000);
    }

    private static long RunMonkeys(string[] lines, int rounds)
    {
        var monkeys = lines
            .Select((x, i) => x.StartsWith("Monkey") ? i : -1)
            .Where(x => x >= 0)
            .Select(x =>
            {
                var monkeyId = int.Parse(MonkeyRegex().Match(lines[x]).Groups[1].Value);
                var startingItems = lines[x + 1].Split(": ")[1].Split(',', StringSplitOptions.TrimEntries).Select(long.Parse);
                var operation = ParseOperation(lines[x + 2].Split("= ")[1]);
                var divisibleBy = int.Parse(lines[x + 3].Split(" ")[^1]);
                var trueMonkey = int.Parse(lines[x + 4].Split(" ")[^1]);
                var falseMonkey = int.Parse(lines[x + 5].Split(" ")[^1]);
                return new Monkey(monkeyId, startingItems.ToList(), operation, divisibleBy, trueMonkey, falseMonkey);
            })
            .ToDictionary(x => x.Id);

        var superModulo = monkeys.Values.Aggregate(1, (current, monkey) => current * monkey.DivisibleBy);

        foreach (var (monkeyId, item) in Enumerable.Range(1, rounds)
            .SelectMany(x => monkeys.Values)
            .SelectMany(x => x.MakeTurn(rounds == 20 ? 3 : 1, superModulo)))
        {
            monkeys[monkeyId].Items.Add(item);
        }

        return monkeys.Values.OrderByDescending(x => x.Inspection)
            .Take(2).Aggregate(1L, (a, b) => a * b.Inspection);
    }

    private static Func<long, long> ParseOperation(string op)
    {
        var test = OperationRegex().Split(op).Select(x => x.Trim()).ToList();

        var useMultiply = test[1] == "*";
        return worryLevel =>
        {
            var parts = test
                .Where((x, i) => i != 1)
                .Select(x =>x switch
                {
                    "old" => worryLevel,
                    _ => long.Parse(x)
                })
                .ToList();

            return useMultiply ? parts[0] * parts[1] : parts[0] + parts[1];
        };
    }
}
