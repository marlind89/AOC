using System.Text;

namespace AOC2021.Puzzles
{
    internal abstract class Puzzle<T> : IPuzzle<T>
    {
        public abstract T One();
        public abstract T Two();

        public override string ToString()
        {
            var puzzleNumber = int.Parse(GetType().Name.Replace("Puzzle", ""));
            var builder = new StringBuilder();
            builder.AppendLine($"Puzzle {puzzleNumber}, Part One: {One()}");
            builder.AppendLine($"Puzzle {puzzleNumber}, Part Two: {Two()}");
            return builder.ToString();
        }
    }
}
