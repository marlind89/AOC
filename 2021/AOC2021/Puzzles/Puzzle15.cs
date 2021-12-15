using AOC2021.Helpers;

namespace AOC2021.Puzzles
{
    internal class Puzzle15 : Puzzle<int>
    {
        protected override void Solve(string[] lines)
        {
            var grid = Grid.CreateGrid(lines);
            One = GetLowestRisk(grid);
            Two = GetLowestRisk(ExpandGrid(grid));
        }

        private static int[,] ExpandGrid(int[,] grid)
        {
            var newGrid = new int[grid.GetLength(0) * 5, grid.GetLength(1) * 5];
            
            foreach (var i in Enumerable.Range(0, 25))
            {
                var col = i % 5;
                var row = (i / 5);
                foreach (var (x, y) in Grid.Iterate(grid))
                {
                    var oldVal = grid[x, y] + col + row;
                    if (oldVal > 9)
                    {
                        oldVal -= 9;
                    }
                    newGrid[x + (col * grid.GetLength(0)), y + (row * grid.GetLength(1))] = oldVal;
                }
            }
            
            return newGrid;
        }

        private static int GetLowestRisk(int[,] riskGrid)
        {
            var target = (x: riskGrid.GetLength(0) - 1, y: riskGrid.GetLength(1) -1);
            var queue = new PriorityQueue<(int x, int y), int>();
            var totalRiskGrid = new int[riskGrid.GetLength(0), riskGrid.GetLength(1)];

            totalRiskGrid[0, 0] = 0;
            queue.Enqueue((0, 0), 0);
           
            while (queue.Count > 0)
            {
                var p =  queue.Dequeue();

                foreach (var n in Grid.GetNeighbours(riskGrid, p, false))
                {
                    if (totalRiskGrid[n.x, n.y] == 0)
                    {
                        var totalRisk = totalRiskGrid[p.x, p.y] + riskGrid[n.x, n.y];
                        totalRiskGrid[n.x, n.y] = totalRisk;
                        if (n == target)
                        {
                            break;
                        }
                        queue.Enqueue(n, totalRisk);
                    }
                }
            }
            return totalRiskGrid[target.x, target.y];
        }
    }
}
