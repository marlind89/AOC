using System.Text.RegularExpressions;

namespace AOC2021.Puzzles;

internal class Puzzle8 : Puzzle<int>
{
    
    protected override void Solve(string[] lines)
    {
        var sums = lines
            .Select(x => x.Split(" | "))
            .Aggregate(new List<string>(), (sums, signalPatterns) =>
            {
                var decoder = CreateDecoder(signalPatterns[0].Split(" "));
                sums.Add(string.Concat(signalPatterns[1].Split(" ").Select(x => Decode(x, decoder))));
                return sums;
            });

        One = sums.Sum(sum => Regex.Matches(sum, "1|4|7|8").Count);
        Two = sums.Sum(int.Parse);
    }

    private static IDictionary<char, char> CreateDecoder(string[] signals)
    {
        var translatedSegments = new Dictionary<char, char>();

        var nums = signals
            .Select(signal => new { signal, digit = GetDigitByLength(signal) })
            .Where(x => x.digit != null)
            .ToDictionary(x => x.digit!.Value, x => x.signal);

        nums[6] = signals.First(x => x.Length == 6 && x.Contains(nums[1][0]) != x.Contains(nums[1][1]));

        translatedSegments['a'] = nums[7].First(c => !nums[1].Contains(c));
        translatedSegments['c'] = nums[6].Contains(nums[1][0]) ? nums[1][1] : nums[1][0];
        translatedSegments['f'] = nums[6].Contains(nums[1][0]) ? nums[1][0] : nums[1][1];

        nums[3] = signals.First(x => x.Length == 5 &&
            x.Contains(translatedSegments['c']) && x.Contains(translatedSegments['f']));

        translatedSegments['b'] = nums[4].First(c => !nums[3].Contains(c));
        translatedSegments['g'] = nums[3].First(c => c != translatedSegments['a'] && !nums[4].Contains(c));
        translatedSegments['d'] = nums[3].First(c => !translatedSegments.ContainsValue(c));
        translatedSegments['e'] = "abcdefg".First(c => !translatedSegments.ContainsValue(c));

        return translatedSegments.ToDictionary(x => x.Value, x => x.Key);
    }

    private static int? GetDigitByLength(string segments) => segments.Length switch
    {
        2 => 1,
        3 => 7,
        4 => 4,
        7 => 8,
        _ => null
    };

    private static int Decode(string segment, IDictionary<char, char> decoder) => 
        string.Concat(segment.Select(x => decoder[x]).OrderBy(c => c)) switch
        {
            "abcefg" => 0,
            "cf" => 1,
            "acdeg" => 2,
            "acdfg" => 3,
            "bcdf" => 4,
            "abdfg" => 5,
            "abdefg" => 6,
            "acf" => 7,
            "abcdefg" => 8,
            "abcdfg" => 9,
            _ => -1
        };
}
