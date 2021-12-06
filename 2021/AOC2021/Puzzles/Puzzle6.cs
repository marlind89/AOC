namespace AOC2021.Puzzles
{
    internal class Puzzle6 : Puzzle<long>
    {
        protected override void Solve(string[] lines)
        {
            var initialFishes = Enumerable.Range(0, 9).ToDictionary(day => day, _ => 0L);

            foreach (var fish in lines[0].Split(",").Select(int.Parse))
            {
                initialFishes[fish] += 1;
            }

            One = CountFishes(initialFishes, 80);
            Two = CountFishes(initialFishes, 256);
        }

        private static long CountFishes(IDictionary<int, long> fishDict, int days) => Enumerable
            .Range(0, days)
            .Aggregate(fishDict, (a, _) => a = SimulateDay(a))
            .Values
            .Sum();

        private static IDictionary<int, long> SimulateDay(IDictionary<int, long> fishDict)
        {
            var result = fishDict.ToDictionary(x => x.Key, _ => 0L);
            foreach (var age in Enumerable.Range(0, 9).Reverse())
            {
                if (age > 0)
                {
                    result[age - 1] = fishDict[age];
                }
                else
                {
                    var fishCount = fishDict[0];
                    result[6] += fishCount;
                    result[8] += fishCount; 
                }
                
            }

            return result;
        }
    }
}
