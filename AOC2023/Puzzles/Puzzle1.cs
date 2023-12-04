namespace AOC2023.Puzzles;

internal class Puzzle1 : Puzzle<int>
{
    private static readonly List<string> Digits = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

    private static int FindNumber(ReadOnlySpan<char> ls, bool fromStart)
    {
        int startIndex = fromStart ? 0 : ls.Length - 1;
        int increment = fromStart ? 1 : -1;
        for (int i = startIndex; fromStart ? i < ls.Length : i >= 0; i += increment)
        {
            var currentChar = ls[i];
            if (char.IsNumber(currentChar))
            {
                return currentChar - '0';
            }

            for (var j = 0; j < Digits.Count; j++)
            {
                var digit = Digits[j];
                if (fromStart ? ls[i..].StartsWith(digit) : ls[..(i + 1)].EndsWith(digit))
                {
                    return j + 1;
                }
            }
        }
        return -1;
    }

    protected override void Solve(string[] lines)
    {
        One = lines
            .Sum(line => int.Parse($"{line.FirstOrDefault(char.IsNumber)}{line.LastOrDefault(char.IsNumber)}"));

        Two = lines
            .Sum(line =>
            {
                var ls = line.AsSpan();
                return int.Parse($"{FindNumber(ls, true)}{FindNumber(ls, false)}");
            });
    }
}
