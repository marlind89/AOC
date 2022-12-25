namespace AOC2022.Puzzles;

internal class Puzzle21 : Puzzle<long>
{
    interface IMonkey
    {
        long GetValue();
    }

    class MathMonkey : IMonkey
    {
        public MathMonkey(string operation) => Operation = operation;
        
        public IMonkey First { get; set; } = null!;
        public IMonkey Second { get; set; } = null!;
        public string Operation { get; }

        public long GetValue()
        {
            var first = First.GetValue();
            var second = Second.GetValue();
            return Operation switch
            {
                "+" => first + second,
                "-" => first - second,
                "*" => first * second,
                "/" => first / second,
                _ => throw new InvalidOperationException()
            };
        }
    }

    class ValueMonkey : IMonkey
    {
        public ValueMonkey(long value) => _value = value;

        public long _value;
        public long GetValue() => _value;
    }

    protected override void Solve(string[] lines)
    {
        var monkeys = CreateMonkeys(lines);

        var root = (MathMonkey)monkeys["root"];
        One = root.GetValue();

        var humn = (ValueMonkey)monkeys["humn"];
        var next = root;
        var expectedValue = (HasMonkey(next.First, humn) ? next.Second : next.First).GetValue();
        while ((next = GetNextTowards(next, humn) as MathMonkey) != null)
        {
            var goLeft = HasMonkey(next.First, humn);
            expectedValue = SolveEquation(goLeft, expectedValue, (goLeft ? next.Second : next.First).GetValue(), next.Operation);
        }

        Two = expectedValue;
    }


    private static IMonkey? GetNextTowards(IMonkey root, IMonkey target)
    {
        if (root == target || root is not MathMonkey mathMonkey)
        {
            return null;
        }

        return HasMonkey(mathMonkey.First, target)
            ? mathMonkey.First
            : mathMonkey.Second;;
    }

    private static bool HasMonkey(IMonkey root, IMonkey target)
    {
        if (root == target)
        {
            return true;
        }

        if (root is not MathMonkey mathMonkey)
        {
            return false;
        }

        return HasMonkey(mathMonkey.First, target) || HasMonkey(mathMonkey.Second, target);
    }

    private static long SolveEquation(bool goingLeft, long expectedValue, long frozenValue, string operation) => (goingLeft, operation) switch
    {
        (_,     "*") => expectedValue / frozenValue,
        (_,     "+") => expectedValue - frozenValue,
        (true,  "/") => expectedValue * frozenValue,
        (true,  "-") => expectedValue + frozenValue,
        (false, "/") => frozenValue   / expectedValue,
        (false, "-") => frozenValue   - expectedValue,
        _ => throw new InvalidOperationException(),
    };

    private static IDictionary<string, IMonkey> CreateMonkeys(string[] lines)
    {
        var monkeys = lines
            .Select(line =>
            {
                var nameIdx = line.IndexOf(':');
                var name = line[0..nameIdx];

                var operation = line[(nameIdx + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                IMonkey monkey = operation.Length == 1
                    ? new ValueMonkey(long.Parse(operation[0]))
                    : new MathMonkey(operation[1]);

                return (name, monkey);
            })
            .ToDictionary(x => x.name, x => x.monkey);

        ConnectMonkeys(lines, monkeys);
        return monkeys;
    }

    private static void ConnectMonkeys(string[] lines, IDictionary<string, IMonkey> monkeys)
    {
        foreach (var line in lines)
        {
            var nameIdx = line.IndexOf(':');
            var name = line[0..nameIdx];

            var monkey = monkeys[name];
            if (monkey is not MathMonkey mathMonkey)
            {
                continue;
            }

            var operation = line[(nameIdx + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            mathMonkey.First = monkeys[operation[0]];
            mathMonkey.Second = monkeys[operation[2]];
        }
    }
}
