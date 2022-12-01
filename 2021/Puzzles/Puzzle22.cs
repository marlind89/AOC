namespace AOC2021.Puzzles;

internal class Puzzle22 : Puzzle<long>
{
    record struct Position(int X, int Y, int Z);
    record struct Cuboid(Position From, Position To)
    {
        public long Volume => (To.X - From.X + 1L) * Math.Abs(To.Y - From.Y + 1L) * Math.Abs(To.Z - From.Z + 1L);

        public Cuboid? Intersect(Cuboid other)
        {
            if (From.X > other.To.X || To.X < other.From.X ||
                From.Y > other.To.Y || To.Y < other.From.Y ||
                From.Z > other.To.Z || To.Z < other.From.Z)
            {
                return null;
            }

            return new Cuboid(
                new Position(Math.Max(From.X, other.From.X), Math.Max(From.Y, other.From.Y), Math.Max(From.Z, other.From.Z)),
                new Position(Math.Min(To.X, other.To.X), Math.Min(To.Y, other.To.Y), Math.Min(To.Z, other.To.Z))
            );
        }

    }

    record struct Instruction(bool On, Cuboid Cuboid)
    {
        public Instruction? Intersect(Cuboid other, bool on)
        {
            var intersection = Cuboid.Intersect(other);

            if (intersection == null)
            {
                return null;
            }

            return new Instruction(on, intersection.Value);
        }
    }

    protected override void Solve(string[] lines)
    {
        var instructions = lines.Select(l =>
        {
            int FirstPos(string part) => int.Parse(part[0..part.IndexOf("..")]);
            int LastPos(string part) 
            {
                var endIdx = part.IndexOf(",");
                if (endIdx == -1)
                {
                    endIdx = part.Length;
                }

                return int.Parse(part[(part.IndexOf("..") + 2)..endIdx]); ;
            }

            var splits = l.Split("=");
            var x = splits[1];
            var y = splits[2];
            var z = splits[3];
            var from = new Position(FirstPos(x), FirstPos(y), FirstPos(z));
            var to = new Position(LastPos(x), LastPos(y), LastPos(z));
            return new Instruction(splits[0].StartsWith("on"), new Cuboid(from, to));
        }).ToList();

        One = InitProcedure(instructions);
        Two = RebootProcedure(instructions);
    }


    private static int InitProcedure(IEnumerable<Instruction> instructions)
    {
        var loop = (int from, int to, Action<int> action) =>
        {
            if (to < -50 || from > 50)
            {
                return;
            }

            from = Math.Max(from, -50);
            to = Math.Min(to, 50);
            int length = Math.Abs(from - to);
            for (int i = 0; i <= length; i++)
            {
                int step = (from < to) ? from + i : to + (length - i);
                action(step);
            }
        };

        var cubes = new Dictionary<Position, bool>();

        foreach (var instruction in instructions)
        {
            loop(instruction.Cuboid.From.X, instruction.Cuboid.To.X, x =>
            {
                loop(instruction.Cuboid.From.Y, instruction.Cuboid.To.Y, y =>
                {
                    loop(instruction.Cuboid.From.Z, instruction.Cuboid.To.Z, z =>
                    {
                        var pos = new Position(x, y, z);
                        if (instruction.On)
                        {
                            cubes[pos] = instruction.On;
                        }
                        else
                        {
                            cubes.Remove(pos);
                        }
                    });
                });
            });
        }

        return cubes.Count;
    }


    private static long RebootProcedure(List<Instruction> instructions)
    {
        var resultInstructions = new List<Instruction>();
        foreach (var instruction in instructions)
        {
            var instructionsToAdd = resultInstructions
                .Select(x => instruction.Intersect(x.Cuboid, !x.On))
                .Where(x => x != null)
                .Select(x => x!.Value)
                .ToList();

            if (instruction.On)
            {
                instructionsToAdd.Insert(0, instruction);
            }

            resultInstructions.AddRange(instructionsToAdd);
        }

        return resultInstructions.Aggregate(0L, (totalVolume, i) => totalVolume + i.Cuboid.Volume * (i.On ? 1 : -1));
    }
}
