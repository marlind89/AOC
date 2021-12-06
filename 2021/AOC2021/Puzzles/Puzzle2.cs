namespace AOC2021.Puzzles
{
    internal class Puzzle2 : Puzzle<int>
    {
        private record Instruction(string Command, int Amount);
        private record Position(int HorPos = 0, int Depth = 0, int Aim = 0)
        {
            internal int FinalPosition { get => HorPos * Depth; }
        }

        protected override void Solve(string[] lines)
        {
            var instructions = lines
                .Select(x =>
                {
                    var parts = x.Split(' ');
                    return new Instruction(parts[0], int.Parse(parts[1]));
                })
                .ToList();

            One = instructions
                .Aggregate(new Position(), (c, i) => i.Command switch
                {
                    "forward" => c with { HorPos = c.HorPos + i.Amount },
                    "down" => c with { Depth = c.Depth + i.Amount },
                    "up" => c with { Depth = c.Depth - i.Amount },
                    _ => c
                }).FinalPosition;

            Two = instructions
                .Aggregate(new Position(), (c, i) => i.Command switch
                {
                    "forward" => c with { HorPos = c.HorPos + i.Amount, Depth = c.Depth + c.Aim * i.Amount },
                    "down" => c with { Aim = c.Aim + i.Amount },
                    "up" => c with { Aim = c.Aim - i.Amount },
                    _ => c
                }).FinalPosition;
        }
    }
}
