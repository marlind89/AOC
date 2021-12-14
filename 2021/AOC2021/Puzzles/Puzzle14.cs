namespace AOC2021.Puzzles
{
    internal class Puzzle14 : Puzzle<long>
    {
        protected override void Solve(string[] lines)
        {
            var rules = lines[2..].Select(x => x.Split(" -> ")).ToDictionary(x => x[0], x => x[1]);
            One = CountElems(rules, lines[0], 10);
            Two = CountElems(rules, lines[0], 40);
        }

        private static long CountElems(IDictionary<string, string> rules, string initTemplate, int times)
        {
            var pairCounts = initTemplate.Zip(initTemplate.Skip(1))
                .Select(x => string.Concat(x.First, x.Second))
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => (long) x.Count());

            var letterCounts = initTemplate
                .GroupBy(x => x)
                .ToDictionary(x => x.Key.ToString(), x => (long) x.Count());

            foreach (var _ in Enumerable.Range(0, times))
            {
                var newPairs = new Dictionary<string, long>();
                foreach (var (pair, count) in pairCounts)
                {
                    var elemToInsert = rules[pair];
                    var leftPair = pair[0] + elemToInsert;
                    var rightPair = elemToInsert + pair[1];

                    newPairs[leftPair] = newPairs.GetValueOrDefault(leftPair, 0) + count;
                    newPairs[rightPair] = newPairs.GetValueOrDefault(rightPair, 0) +  count;
                    letterCounts[elemToInsert] = letterCounts.GetValueOrDefault(elemToInsert, 0) + count;
                }
                pairCounts = newPairs.ToDictionary(x => x.Key, x => x.Value);
            }

            return letterCounts.Max(x => x.Value) - letterCounts.Min(x => x.Value);
        }
    }
}
