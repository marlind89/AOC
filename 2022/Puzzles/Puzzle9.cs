namespace AOC2022.Puzzles;

internal class Puzzle9 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        One = Move(lines, 2);
        Two = Move(lines, 10);
    }

    private static int Move(string[] lines, int knotsAmount)
    {
        var knots = new (int x, int y)[knotsAmount];
        var visited = new HashSet<(int x, int y)>{ (0, 0) };

        foreach (var line in lines)
        {
            var split = line.Split(' ');
            var direction = split[0];

            foreach (var _ in Enumerable.Range(0, int.Parse(split[1])))
            {
                knots[0] = MoveHead(direction, knots[0].x, knots[0].y);
                foreach (var x in Enumerable.Range(1, knotsAmount - 1))
                {
                    knots[x] = MoveKnot(knots[x - 1].x, knots[x - 1].y, knots[x].x, knots[x].y);
                }
                visited.Add(knots[^1]);
            } 
        }

        return visited.Count;
    }

    private static (int x, int y) MoveHead(string direction, int x, int y) => direction switch
    {
        "U" => (x, y - 1),
        "D" => (x, y + 1),
        "L" => (x - 1, y),
        "R" => (x + 1, y),
        _ => throw new Exception("Invalid direction")
    };

    private static (int x, int y) MoveKnot(int hx, int hy, int tx, int ty) => 
        (Math.Abs(hx - tx) + Math.Abs(hy - ty) > 2)
            ? (GetNewPos(hx, tx), GetNewPos(hy, ty))
            : (GetNewPos(tx, hx), GetNewPos(ty, hy));

    private static int GetNewPos(int first, int second) => (first - second) switch
    {
        > 1  => first - 1,
        < -1 => first + 1,
        _    => first
    };
}
