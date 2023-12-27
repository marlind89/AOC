using System.Collections.Concurrent;

namespace AOC2023.Puzzles;

internal partial class Puzzle12 : Puzzle<long>
{
    private readonly ConcurrentDictionary<string, long> _cache = [];

    record SpringRecord(string Springs, int[] Groups)
    {
        public SpringRecord Unfold()
        {
            return new SpringRecord(
                string.Join('?', Enumerable.Repeat(Springs, 5)),
                Enumerable.Repeat(Groups, 5).SelectMany(x => x).ToArray());
        }
    }

    protected override void Solve(string[] lines)
    {
        var springs = lines
            .Select(x =>
            {
                var split = x.Split(' ');
                return new SpringRecord(split[0], split[1].Split(',').Select(int.Parse).ToArray());
            })
            .ToList();

        One = springs.AsParallel().Sum(x => GetCombinations(x.Springs, x.Groups));
        Two = springs.AsParallel().Select(x => x.Unfold()).Sum(x => GetCombinations(x.Springs, x.Groups));
    }


    private long GetCombinations(string springs, int[] groups)
    {
        var cacheKey = $"{springs}, {string.Join(',', groups)}";

        if (_cache.TryGetValue(cacheKey, out var combinations))
        {
            return combinations;
        }

        combinations = CalculateCombinations(springs, groups);
        _cache[cacheKey] = combinations;

        return combinations;
    }

    private long CalculateCombinations(string springs, int[] groups)
    {
        while (true)
        {
            if (groups.Length == 0)
            {
                return springs.Contains('#') ? 0 : 1;
            }

            var currentGroup = groups[0];

            if (string.IsNullOrEmpty(springs))
            {
                return 0;
            }
            else if (springs.StartsWith('.'))
            {
                springs = springs.Trim('.');
            }
            else if (springs.StartsWith('?'))
            {
                return GetCombinations("." + springs[1..], groups) + GetCombinations("#" + springs[1..], groups);
            }
            else if (springs.StartsWith('#'))
            {
                if (springs.Length < currentGroup || springs[..currentGroup].Contains('.'))
                {
                    return 0;
                }

                if (groups.Length > 1)
                {
                    if (springs.Length < currentGroup + 1 || springs[currentGroup] == '#')
                    {
                        return 0;
                    }

                    springs = springs[(currentGroup + 1)..];
                    groups = groups[1..];
                }
                else
                {
                    springs = springs[currentGroup..];
                    groups = groups[1..];
                }
            }
        }
    }
}

