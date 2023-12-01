namespace AOC2023.Puzzles;

internal class Puzzle1 : Puzzle<int>
{
    private static readonly List<string> Digits = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

    protected override void Solve(string[] lines)
    {
        One = lines
            .Sum(line => int.Parse($"{line.FirstOrDefault(char.IsNumber)}{line.LastOrDefault(char.IsNumber)}"));

        Two = lines
            .Sum(line =>
            {
                var firstNumber = Enumerable.Range(0, line.Length)
                    .Select(idx => char.IsNumber(line[idx])
                        ? line[idx] - '0'
                        : 1 + Digits.FindIndex(word => line[idx..].StartsWith(word)))
                    .FirstOrDefault(x => x > 0);

                var lastNumber = Enumerable.Range(0, line.Length)
                    .Select(idx => char.IsNumber(line[^(idx + 1)])
                        ? line[^(idx + 1)] - '0'
                        : 1 + Digits.FindIndex(word => line[..^idx].EndsWith(word)))
                    .FirstOrDefault(x => x > 0);

                return int.Parse($"{firstNumber}{lastNumber}");
            });
    }
}
