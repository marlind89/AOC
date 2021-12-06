namespace AOC2021.Puzzles
{
    internal class Puzzle1 : Puzzle<int>
    {
        private const int WindowSize = 3;

        protected override void Solve(string[] lines)
        {
            var depths = lines.Select(int.Parse).ToList();

            One = GetMeasurementCounts(depths);

            var windowSums = depths
                .Where((_, idx) => idx <= (depths.Count - WindowSize))
                .Select((_, idx) => depths.GetRange(idx, WindowSize).Sum())
                .ToList();
            
            Two = GetMeasurementCounts(windowSums);
        }

        private static int GetMeasurementCounts(IReadOnlyCollection<int> depths)
        {
            return depths
                .Zip(depths.Skip(1))
                .Aggregate(0, (c, t) => t.Second > t.First ? c + 1 : c);
        }
    }
}
