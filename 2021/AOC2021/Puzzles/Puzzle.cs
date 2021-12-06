using System.Text;

namespace AOC2021.Puzzles
{
    internal abstract class Puzzle<T> : IPuzzle
    {
        protected T? One { get; set; }
        protected T? Two { get; set; }

        private readonly int _puzzleNumber;
        private readonly string[] _lines;

        public Puzzle() 
        {
            _puzzleNumber = int.Parse(GetType().Name.Replace("Puzzle", ""));
             _lines = File.ReadAllLines($"Inputs/Puzzle{_puzzleNumber}.txt");
        } 

        public void Solve() => Solve(_lines);
        protected abstract void Solve(string[] lines);

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Puzzle {_puzzleNumber}, Part One: {One}");
            builder.AppendLine($"Puzzle {_puzzleNumber}, Part Two: {Two}");
            return builder.ToString();
        }
    }
}
