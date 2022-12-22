namespace AOC2022.Puzzles;

internal class Puzzle17 : Puzzle<int, ulong>
{
    protected override void Solve(string[] lines)
    {
        var rocks = File.ReadAllText("Inputs/Rocks.txt")
            .Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(rock =>
            {
                var structure = new char[4, 4];
                Grid.Fill(structure, '.');

                var rows = rock.Split(Environment.NewLine);
                for (var y = 0; y < rows.Length; y++)
                {
                    var row = rows[y];
                    for (int x = 0; x < rows[y].Length; x++)
                    {
                        structure[x, y] = row[x];
                    }
                }
                return structure;
            })
            .ToArray();

        var movements = lines[0];

        One = DropRocks(movements, rocks, 2022, 0);

        // After analyzing the output we can see that the pattern repeat itself after a while.
        // Then we can only simulate the cyclic part and multiply that to get the correct result
        var (heightCycleStart, heightCycleLength, rockCycleStart, rockCycleLength, jetCycleStart) = (792U, 2724U, 496U, 1740U, 2870);

        ulong totalRocks = 1000000000000UL - rockCycleStart;
        ulong totalCycles = totalRocks / rockCycleLength;
        var rocksRemainder = (int) (totalRocks % rockCycleLength);
        Two = heightCycleStart + (ulong) DropRocks(movements, rocks, rocksRemainder, jetCycleStart) + (totalCycles * heightCycleLength);
    }

    private static int DropRocks(string movements, char[][,] rocks, int amountRocks, int jetIdx)
    {
        var depth = 4000;
        var chamber = new char[7, depth + 1];
        Grid.Fill(chamber, '.');
        foreach (var x in Enumerable.Range(0, 7))
        {
            chamber[x, depth] = '#';
        }
        
        var fallenRocks = 0;
        var tick = jetIdx;
        var height = depth;

        while (true)
        {
            foreach (var rock in rocks)
            {
                height = DropRock(chamber, rock, movements, ref tick, height);
                fallenRocks++;
                if (fallenRocks == amountRocks)
                {
                    return depth - height;
                }
            }
        }
    }

    private static int DropRock(char[,] chamber, char[,] rock, string movements, ref int tick, int height)
    {
        var heightLeverage = height - 7;

        AddRockToChamber(height, chamber, rock);
        while (true)
        {
            var m = movements[tick % movements.Length];
            Move(chamber, heightLeverage, m == '<');

            tick++;

            if (Grid.Iterate(chamber, heightLeverage)
                .Where(x => chamber[x.x, x.y] == '@')
                .Any(p => chamber[p.x, p.y + 1] == '#'))
            {
                break;
            }

            MoveDown(chamber, heightLeverage);
        }

        foreach (var (x, y) in Grid.Iterate(chamber, heightLeverage)
            .Where(p => chamber[p.x, p.y] == '@'))
        {
            chamber[x, y] = '#';
        }

        return GetNewHeight(chamber, height);
    }

    private static int GetNewHeight(char[,] chamber, int prevHeight)
    {
        for (int y = prevHeight - 4; y < prevHeight + 1; y++)
        {
            for (int x = 0; x < chamber.GetLength(0); x++)
            {
                if (chamber[x,y] == '#')
                {
                    return y;
                }
            }
        }

        return 0;
    }

    private static void AddRockToChamber(int floor, char[,] chamber, char[,] rock)
    {
        foreach (var (x, y) in Grid.Iterate(rock))
        {
            chamber[2 + x, floor - (7 - y)] = rock[x, y];
        }
    }

    private static readonly (int x, int y) Left = (-1, 0);
    private static readonly (int x, int y) Right = (1, 0);

    private static void Move(char[,] chamber, int height, bool left)
    {
        var move = left ? Left : Right;

        var gridIterate = Grid.Iterate(chamber, height);
        if (!left)
        {
            gridIterate = gridIterate.Reverse();
        }

        var count = gridIterate
            .Where(p => chamber[p.x, p.y] == '@')
            .Select(p =>
            {
                if (Grid.IsOutOfRange(chamber, (p.x + move.x, p.y)) || chamber[p.x + move.x, p.y] == '#')
                {
                    return (-1, -1);
                }

                return p;
            })
            .ToList();

        if (count.Any(x => x.Item1 == -1))
        {
            return;
        }

        foreach (var (x, y) in count)
        {
            chamber[x + move.x, y] = chamber[x, y];
            chamber[x, y] = '.';
        }
    }

    private static void MoveDown(char[,] chamber, int height)
    {
        foreach (var (x, y) in Grid.Iterate(chamber, height).Reverse())
        {
            if (chamber[x, y] == '@')
            {
                if (Grid.IsOutOfRange(chamber, (x, y + 1)))
                {
                    return;
                }

                chamber[x, y + 1] = chamber[x, y];
                chamber[x, y] = '.';
            }
        }
    }
}
