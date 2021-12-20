using AOC2021.Helpers;
using System.Text;

namespace AOC2021.Puzzles
{
    internal class Puzzle20 : Puzzle<int>
    {
        private int _flip = 0;

        protected override void Solve(string[] l)
        {
            var algo = l[0];
            var input = l[2..];

            One = DecodeImage(algo, input, 2);
            Two = DecodeImage(algo, input, 50);
        }

        private int DecodeImage(string algo, string[] input, int times)
        {
            return Enumerable.Range(0, times)
                .Aggregate(input, (a, _) => DecodeImage(algo, a))
                .SelectMany(s => s)
                .Count(x => x == '#');
        }

        private string[] DecodeImage(string algo, string[] lines)
        {
            var inputImage = Grid.CreateGrid(lines, c => c == '#');
            var outputImage = new char[lines[0].Length + 2, lines.Length + 2];

            foreach (var (x, y) in Grid.Iterate(outputImage))
            {
                var str = Grid.NeighborOffsetsWithDiagsIncludeSelf
                    .Select(c => (x: x + c.x, y: y + c.y))
                    .Aggregate(new StringBuilder(), (sb, n) =>
                     {
                         bool pixel = Grid.IsOutOfRange(inputImage, (n.x - 1, n.y - 1))
                            ? (algo[0] == '#' && _flip % 2 == 1)
                            : inputImage[n.x - 1, n.y - 1];

                         return sb.Append(pixel ? '1' : '0');
                     })
                    .ToString();
                
                var idx = Convert.ToInt32(str, 2);
                var outputChar = algo[idx];

                outputImage[x, y] = outputChar;
            }

            _flip++;

            return Enumerable.Range(0, outputImage.GetLength(1))
                .Select(y => string.Concat(Enumerable
                    .Range(0, outputImage.GetLength(0))
                    .Select(x => outputImage[x, y])
                ))
                .ToArray();
        }
    }
}
