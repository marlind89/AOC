namespace AOC2021.Puzzles
{
    internal class Puzzle10 : Puzzle<long>
    {
        private static readonly string[] Chunks = new string[] { "()", "[]", "{}", "<>" };

        protected override void Solve(string[] lines)
        {
            var corruptedLines = lines
                .Select((x, idx) => new { score = FindIllegalCharacter(x), idx })
                .Where(x => x.score > 0)
                .ToList();
                
            One = corruptedLines.Select(x => x.score).Sum();

            var corruptedLineIndexes = corruptedLines.Select(x => x.idx).ToList();

            var fixedLines = lines
                .Where((x, idx) => !corruptedLineIndexes.Contains(idx))
                .Select(x => FixIncompleteLines(x))
                .OrderBy(x => x)
                .ToList();

            Two = fixedLines[fixedLines.Count / 2];
        }

        private static long FixIncompleteLines(string line) => GetOpenChars(line, false, out _)
            .Reverse()
            .Select(GetClosedChar)
            .Aggregate(0L, (totalScore, b) => totalScore * 5 + b switch
            {
                ')' => 1,
                ']' => 2,
                '}' => 3,
                '>' => 4,
                _ => 0
            });

        private static int FindIllegalCharacter(string line)
        {
            _ = GetOpenChars(line, true, out var point);
            return point;
        }

        private static char? GetOpenChar(char @char) => Chunks.FirstOrDefault(x => x[1] == @char)?[0];
        private static char GetClosedChar(char @char) => Chunks.Single(x => x[0] == @char)[1];

        private static IEnumerable<char> GetOpenChars(string line, bool checkIllegal, out int illegalCharPoint)
        {
            illegalCharPoint = 0;

            var openChars = new List<char>();
            foreach (var @char in line)
            {
                var openChar = GetOpenChar(@char);
                if (openChar == null)
                {
                    openChars.Add(@char);
                }
                else if (checkIllegal && openChars[^1] != openChar)
                {
                    illegalCharPoint = @char switch
                    {
                        ')' => 3,
                        ']' => 57,
                        '}' => 1197,
                        '>' => 25137,
                        _ => 0
                    };
                    break;
                }
                else
                {
                    openChars.RemoveAt(openChars.Count - 1);
                }
            }

            return openChars;
        }
    }
}
