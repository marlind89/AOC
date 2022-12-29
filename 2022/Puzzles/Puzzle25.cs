namespace AOC2022.Puzzles;

internal class Puzzle25 : Puzzle<string>
{
    protected override void Solve(string[] lines)
    {
        One = ToSnafu(lines
            .Select(line => Enumerable.Range(1, line.Length)
                .Sum(c => ToDecimal(line[^c]) * (long)Math.Pow(5, c - 1)))
            .Sum());
    }

    private static int ToDecimal(char snafu) => snafu switch
    {
        '=' => -2,
        '-' => -1,
        _ => snafu - '0',
    };

    private static string ToSnafu(long value)
    {
        var (carry, rem, values) = (0, 0, new List<int>());

        while (value > 0)
        {
            rem = (int) (value % 5) + (rem < 0 ? 1 : 0);
            if (rem > 2)
            {
                rem -= 5;
            }
            values.Add(rem);
            value /= 5;
        }

        return values
            .Reverse<int>()
            .Aggregate("", (acc, cur) =>
                acc + cur switch
                {
                    -2 => "=",
                    -1 => "-",
                    _ => cur
                });
        
    }
}
