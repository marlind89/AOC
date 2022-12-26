using System.Text.RegularExpressions;

namespace AOC2022.Puzzles;

internal partial class Puzzle22 : Puzzle<int>
{
    [GeneratedRegex("(R|L)")] private static partial Regex InstructionRegex();

    public static readonly (int x, int y) Right = ( 1,  0);
    public static readonly (int x, int y) Down  = ( 0,  1);
    public static readonly (int x, int y) Left  = (-1,  0);
    public static readonly (int x, int y) Up    = ( 0, -1);

    public static readonly List<(int x, int y)> Vectors = new List<(int x, int y)> { Right, Down, Left, Up };

    delegate ((int x, int y) gridIdx, (int x, int y) pos, (int x, int y) vector) OobFunc((int x, int y) gridIdx, (int x, int y) vector, (int x, int y) current);

    protected override void Solve(string[] lines)
    {
        var grids = CreateGrids(lines);

        var gridlist = Iterate(grids)
            .Where(p => grids[p.x, p.y] != null)
            .Select(p => grids[p.x, p.y])
            .ToList();

        var getGridIndex = (int x, int y) => gridlist.IndexOf(grids[x, y]);

        var instructions = InstructionRegex().Split(lines[^1]);

        One = Walk(grids, instructions, (gridIdx, vector, current) =>
        {
            char[,]? nextGrid = null;
            var newGridIdx = gridIdx;
            while (nextGrid == null)
            {
                var x = (newGridIdx.x + vector.x) % grids.GetLength(0);
                if (x == -1)
                {
                    x = grids.GetLength(0) - 1;
                }
                var y = (newGridIdx.y + vector.y) % grids.GetLength(1);
                if (y == -1)
                {
                    y = grids.GetLength(1) - 1;
                }

                nextGrid = grids[x, y];
                newGridIdx = (x, y);
            }

            var pos = vector switch
            {
                { x: 1 }  => (0, current.y),
                { x: -1 } => (49, current.y),
                { y: 1 }  => (current.x, 0),
                _         => (current.x, 49),
            };

            return (gridIdx: newGridIdx, pos, vector);
        });

        Two = Walk(grids, instructions, (gridIdx, vector, current) =>
        {
            var gridIndex = getGridIndex(gridIdx.x, gridIdx.y);

            var (newGridIndex, newVector, newPos) = gridIndex switch
            {
                0 when vector == Right => (1, Right, (0, current.y)),
                0 when vector == Down  => (2, Down,  (current.x, 0)),
                0 when vector == Left  => (3, Right, (0, 49 - current.y)),
                0 when vector == Up    => (5, Right, (0, current.x)),

                1 when vector == Right => (4, Left,  (49, 49 - current.y)),
                1 when vector == Down  => (2, Left,  (49, current.x)),
                1 when vector == Left  => (0, Left,  (49, current.y)),
                1 when vector == Up    => (5, Up,    (current.x, 49)),

                2 when vector == Right => (1, Up,    (current.y, 49)),
                2 when vector == Down  => (4, Down,  (current.x, 0)),
                2 when vector == Left  => (3, Down,  (current.y, 0)),
                2 when vector == Up    => (0, Up,    (current.x, 49)),

                3 when vector == Right => (4, Right, (0, current.y)),
                3 when vector == Down  => (5, Down,  (current.x, 0)),
                3 when vector == Left  => (0, Right, (0, 49 - current.y)),
                3 when vector == Up    => (2, Right, (0, current.x)),

                4 when vector == Right => (1, Left,  (49, 49 - current.y)),
                4 when vector == Down  => (5, Left,  (49, current.x)),
                4 when vector == Left  => (3, Left,  (49, current.y)),
                4 when vector == Up    => (2, Up,    (current.x, 49)),

                5 when vector == Right => (4, Up,    (current.y, 49)),
                5 when vector == Down  => (1, Down,  (current.x, 0)),
                5 when vector == Left  => (0, Down,  (current.y, 0)),
                5 when vector == Up    => (3, Up,    (current.x, 49)),

                _ => throw new InvalidOperationException()
            };

            var newGrid = gridlist[newGridIndex];

            var newGridIdx = Iterate(grids).First(p => grids[p.x, p.y] == newGrid);
            return (newGridIdx, newPos, newVector);
        });
    }

    

    private static int Walk(char[,][,] grids, string[] instructions, OobFunc oobFunc)
    {
        var vec = Right;
        var pos = (x: 0, y: 0);
        var gridIdx = (x: 1, y: 0);
        foreach (var instruction in instructions)
        {
            if (int.TryParse(instruction, out var forward))
            {
                foreach (var _ in Enumerable.Range(0, forward))
                {
                    var next = (gridIdx, pos: (x: pos.x + vec.x, y: pos.y + vec.y) , vec);
                    if (Grid.IsOutOfRange(grids[gridIdx.x, gridIdx.y], next.pos))
                    {
                        next = oobFunc(gridIdx, vec, pos);
                    }

                    if (grids[next.gridIdx.x, next.gridIdx.y][next.pos.x, next.pos.y] == '#')
                    {
                        break;
                    }
                    
                    pos = next.pos;
                    gridIdx = next.gridIdx;
                    vec = next.vec;
                }
            }
            else
            {
                var vectorIdx = Vectors.IndexOf(vec);
                var newVectorIdx = instruction == "R"
                    ? (vectorIdx + 1) % Vectors.Count
                    : vectorIdx == 0 ? (Vectors.Count - 1) : (vectorIdx - 1);
                vec = Vectors[newVectorIdx];
            }
        }

        return (1 + pos.y + (gridIdx.y * 50)) * 1000 + 4 * (1 + pos.x + (gridIdx.x * 50)) + Vectors.IndexOf(vec);
    }

    private static IEnumerable<(int x, int y)> Iterate<T>(T[,] arr) => Enumerable.Range(0, arr.GetLength(1))
        .SelectMany(y => Enumerable.Range(0, arr.GetLength(0)).Select(x => (x, y)));

    private static char[,][,] CreateGrids(string[] lines)
    {
        var grid = Grid.CreateGrid(lines[..^2], x => x, ' ');

        var grids = new char[3, 4][,];

        foreach (var gridIdx in Enumerable.Range(0, 12))
        {
            var gridX = gridIdx % 3;
            var gridY = gridIdx / 3;
            var xStart = gridX * 50;
            var yStart = gridY * 50;

            if (Grid.IsOutOfRange(grid, (xStart, yStart)) || grid[xStart, yStart] == ' ')
            {
                continue;
            }

            var curGrid = new char[50, 50];

            foreach (var (x, y) in Enumerable.Range(0, 50)
                .SelectMany(y => Enumerable.Range(0, 50).Select(x => (x, y))))
            {
                var pos = (x: x + xStart, y: y + yStart);

                curGrid[x, y] = grid[pos.x, pos.y];
            }

            grids[gridX, gridY] = curGrid;
        }

        return grids;
    }


}

