namespace AOC2021.Puzzles
{
    internal class Puzzle3 : Puzzle<int>
    {
        private readonly string[] _diagnosticReport = File.ReadAllLines("Inputs/Puzzle3.txt");
        private const int BitLength = 12;

        public override int One()
        {
            var gamma = Enumerable.Range(0, BitLength)
                .Aggregate("", (str, x) => str + FindCommonBit(_diagnosticReport, x, true));

            var gammaNumber = Convert.ToInt32(gamma, 2);
            var epsilon = ~gammaNumber & 0xfff;

            return gammaNumber * epsilon;
        }

        public override int Two() 
        {
            var oxygenGeneratorRating = CalculateRating(true);
            var co2ScrubberRating = CalculateRating(false);
            return oxygenGeneratorRating * co2ScrubberRating;
        }

        private int CalculateRating(bool mostCommonValue)
        {
            var lines = _diagnosticReport.ToList();
            foreach (var idx in Enumerable.Range(0, BitLength))
            {
                var commonBit = FindCommonBit(lines, idx, mostCommonValue);
                lines = lines.Where(line => line[idx] == commonBit).ToList();

                if (lines.Count == 1)
                {
                    return Convert.ToInt32(lines[0], 2);
                }
            }
            return 0;
        }

        private static char FindCommonBit(IReadOnlyCollection<string> lines, int bitPosition, bool mostCommonValue)
        {
            var bitCount = lines.Count(line => line[bitPosition] == '1');
            return (mostCommonValue == 2*bitCount >= lines.Count) ? '1' : '0';
        }
    }
}
