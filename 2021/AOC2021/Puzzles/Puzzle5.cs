namespace AOC2021.Puzzles
{
    internal class Puzzle5 : Puzzle<int>
    {
        public record struct Line(Point From, Point To);

        public struct Point
        {
            public int x;
            public int y;

            public Point(string coord)
            {
                var split = coord.Split(',');
                x = int.Parse(split[0]);
                y = int.Parse(split[1]);
            }

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        protected override void Solve(string[] lineData)
        {
            var lines = lineData
                .Select(x =>
                {
                    var split = x.Split(" -> ");
                    return new Line(new Point(split[0]), new Point(split[1]));
                })
                .ToArray();

            var endPoint = new Point(
                lines.SelectMany(l => new int[] { l.From.x, l.To.x }).Max() + 1,
                lines.SelectMany(l => new int[] { l.From.y, l.To.y }).Max() + 1);

            One = CountOverlaps(endPoint, lines.Where(x => x.From.x == x.To.x || x.From.y == x.To.y));
            Two = CountOverlaps(endPoint, lines);
        }

        private static int CountOverlaps(Point endPoint, IEnumerable<Line> lines)
        {
            var diagram = new int[endPoint.x, endPoint.y];

            foreach (var point in lines.SelectMany(IterateLine))
            {
                diagram[point.x, point.y] += 1;
            }

            return CountOverlaps(diagram);
        }

        private static IEnumerable<int> IterateBetween(int from, int to) 
        {
            var range = Enumerable.Range(Math.Min(from, to), Math.Abs(from - to) + 1);

            return from < to
                ? range
                : range.Reverse();
        }

        private static IEnumerable<Point> IterateLine(Line line)
        {
            if (line.From.x != line.To.x && line.From.y == line.To.y)
            {
                return IterateBetween(line.From.x, line.To.x)
                    .Select(x => new Point(x, line.From.y));
            }
            else if (line.From.y != line.To.y && line.From.x == line.To.x)
            {
                return IterateBetween(line.From.y, line.To.y)
                    .Select(y => new Point(line.From.x, y));
            }

            return IterateBetween(line.From.x, line.To.x)
                .Zip(IterateBetween(line.From.y, line.To.y))
                .Select(z => new Point(z.First, z.Second));
        }

        private static int CountOverlaps(int[,] diagram) => diagram.Cast<int>().Count(x => x > 1);
    }
}
