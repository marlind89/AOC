using System.Text;

namespace AOC2021.Puzzles
{
    internal abstract class Puzzle<T> : IPuzzle
    {
        public int PuzzleNumber { get; }

        protected T? One { get; set; }
        protected T? Two { get; set; }

        private readonly string[] _lines;

        public Puzzle() 
        {
            PuzzleNumber = int.Parse(GetType().Name.Replace("Puzzle", ""));
             _lines = File.ReadAllLines($"Inputs/Puzzle{PuzzleNumber}.txt");
        } 

        public void Solve() => Solve(_lines);
        protected abstract void Solve(string[] lines);

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Puzzle {PuzzleNumber}, Part One: {One}");
            builder.AppendLine($"Puzzle {PuzzleNumber}, Part Two: {Two}");
            return builder.ToString();
        }
    }
}
