using System.Text.RegularExpressions;

namespace AOC2022.Puzzles;

internal partial class Puzzle5 : Puzzle<string>
{
    [GeneratedRegex(@"move (\d+) from (\d+) to (\d+)")] private static partial Regex InstructionRegex();
    [GeneratedRegex(@"\[(.)\]")] private static partial Regex CrateRegex();

    protected override void Solve(string[] lines)
    {
        One = SetupAndMoveCrates(lines, false);
        Two = SetupAndMoveCrates(lines, true);
    }

    private static string SetupAndMoveCrates(string[] lines, bool moveAsGroup)
    {
        var idx = lines.ToList().FindIndex(string.IsNullOrWhiteSpace);
        var crateLines = lines[..idx];
        var instructions = lines[(idx + 1)..];

        var stacks = Enumerable.Range(0, crateLines.Last().Last(char.IsDigit) - '0')
            .Select((x, stackIdx) => new Stack<char>(crateLines.Reverse().Skip(1)
                .Select(crateLine => crateLine
                    .Chunk(4)
                    .Skip(stackIdx)
                    .Select(crateStr => CrateRegex().Match(string.Concat(crateStr))
                        .Groups.Values.Last().Value.FirstOrDefault())
                    .FirstOrDefault())
                .Where(x => x > 0)))
            .ToArray();
        
        MoveCrates(instructions, stacks, moveAsGroup);
        return string.Concat(stacks.Select(x => x.Peek()));
    }

    private static void MoveCrates(IEnumerable<string> instructions, Stack<char>[] stacks, bool moveAsGroup)
    {
        foreach (var instruction in instructions)
        {
            var values = InstructionRegex().Match(instruction).Groups.Values
                .Skip(1).Select(x => int.Parse(x.Value)).ToList();
            var (amount, from, to) = (values[0], values[1], values[2]);
            var crates = Enumerable.Range(0, amount).Select(_ => stacks[from - 1].Pop());
            if (moveAsGroup) crates = crates.Reverse();
            stacks[to - 1].PushRange(crates);
        }
    }
}
