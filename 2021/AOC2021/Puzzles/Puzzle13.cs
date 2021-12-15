using AOC2021.Helpers;
using System.Text;

namespace AOC2021.Puzzles
{
    internal class Puzzle13 : Puzzle<int, string>
    {
        public record Instruction(int FoldAt, bool FoldUp);

        protected override void Solve(string[] lines)
        {
            var coords = lines
                .TakeWhile(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Split(',').Select(int.Parse).ToArray())
                .Select(c => (x: c[0], y: c[1]))
                .ToList();

            var grid = new bool[coords.Max(x => x.x) + 1, coords.Max(y => y.y) + 1];
            foreach (var (x, y) in coords)
            {
                grid[x, y] = true;
            }

            var instructions = lines[(coords.Count + 1)..]
                .Select(x => new Instruction(int.Parse(x[(x.IndexOf("=") + 1)..]), x.Contains('y')))
                .ToList();

            var newGrid = Fold(grid, instructions[0]);
            One = Grid.Iterate(newGrid).Count(x => newGrid[x.x, x.y]);

            var finalGrid = instructions.Aggregate(grid, (a, b) => Fold(a, b));
            var sb = new StringBuilder(Environment.NewLine);
            for (var y = 0; y < finalGrid.GetLength(1); y++)
            {
                sb.Append("    ");
                for (var x = 0; x < finalGrid.GetLength(0); x++)
                {
                    sb.Append(finalGrid[x, y] ? '#' : ' ');
                }
                sb.AppendLine();
            }
            Two = sb.ToString()[..^1];
        }

        private static bool[,] Fold(bool[,] grid, Instruction foldInstruction)
        {
            bool[,] newGrid;
            if (foldInstruction.FoldUp)
            {
                newGrid = new bool[grid.GetLength(0), grid.GetLength(1) / 2];
                foreach (var (x,y) in Grid.Iterate(grid).Where(c => grid[c.x, c.y]))
                {
                    newGrid[x, y < foldInstruction.FoldAt ? y : (2 * foldInstruction.FoldAt - y)] = true;
                }
            }
            else
            {
                newGrid = new bool[grid.GetLength(0) / 2, grid.GetLength(1)];
                foreach (var (x, y) in Grid.Iterate(grid).Where(c => grid[c.x, c.y]))
                {
                    newGrid[x < foldInstruction.FoldAt ? x : (2 * foldInstruction.FoldAt - x), y] = true;
                }
            }

            return newGrid;
        }
    }
}
