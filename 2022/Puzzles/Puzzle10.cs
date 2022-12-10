using System.Text;

namespace AOC2022.Puzzles;

internal class Puzzle10 : Puzzle<int, string>
{
    private const int CrtPixels = 40;

    protected override void Solve(string[] lines)
    {
        var (x, totalCycles, signalStrengths, drawer) = (1, 0, new Dictionary<int, int>(), new StringBuilder());
        foreach (var line in lines)
        {
            var (cycles, addx) = ParseInstruction(line);
            foreach (var currCycle in Enumerable.Range(totalCycles + 1, cycles))
            {
                signalStrengths[currCycle] = x * currCycle;
                var crtIndex = (currCycle - 1) % CrtPixels;
                var pixel = crtIndex >= x - 1 && crtIndex <= x + 1 ? '#' : '.';

                drawer.Append(pixel);
                if (currCycle / CrtPixels > ((currCycle - 1) / CrtPixels)) 
                {
                    drawer.AppendLine();
                }
            }

            totalCycles += cycles;
            x += addx;
        }

        One = Enumerable.Range(0, 6).Select(x => signalStrengths[20 + 40 * x]).Sum();
        Two = Environment.NewLine + Environment.NewLine + drawer.ToString();
    }

    private static (int cycles, int addx) ParseInstruction(string instruction) => instruction switch
    {
        "noop" => (1, 0),
        _ => (2, int.Parse(instruction.Split(' ')[1]))
    };
}
