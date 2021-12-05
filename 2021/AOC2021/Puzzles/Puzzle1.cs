namespace AOC2021.Puzzles
{
    internal class Puzzle1 : Puzzle<int>
    {
        private readonly List<int> _depths = File.ReadAllLines("Inputs/Puzzle1.txt")
            .Select(int.Parse)
            .ToList();

        public override int One() => GetMeasurementCounts(_depths);

        public override int Two()
        {
            const int windowSize = 3;

            var windowSums = _depths
                .Where((_, idx) => idx <= (_depths.Count - windowSize))
                .Select((_, idx) =>  _depths.GetRange(idx, windowSize).Sum())
                .ToList();

            return GetMeasurementCounts(windowSums);
        }

        private static int GetMeasurementCounts(IReadOnlyCollection<int> depths)
        {
            return depths
                .Zip(depths.Skip(1))
                .Aggregate(0, (c, t) => t.Second > t.First ? c + 1 : c);
        }
    }
}
