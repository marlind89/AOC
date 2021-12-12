namespace AOC2021.Puzzles
{
    internal class Puzzle12 : Puzzle<int>
    {
        public class Cave
        {
            public string Name { get; }
            public bool IsSmall { get; }
            public bool CanEnterTwice { get; set; }
            public List<Cave> Connections { get; } = new();

            public Cave(string name, bool canEnterTwice = false)
            {
                Name = name;
                IsSmall = char.IsLower(name[0]);
                CanEnterTwice = canEnterTwice;
            }

            public int EnteredCount { get; set; }
        }

        protected override void Solve(string[] lines)
        {
            var caves = new Dictionary<string, Cave>();

            Cave GetOrCreateCave(string caveStr)
            {
                if (!caves.TryGetValue(caveStr, out var cave))
                {
                    caves[caveStr] = cave = new Cave(caveStr);
                }
                return cave;
            }
            
            foreach (var line in lines)
            {
                var cavesStr = line.Split('-');
                var from = GetOrCreateCave(cavesStr[0]);
                var to = GetOrCreateCave(cavesStr[1]);
                from.Connections.Add(to);
                to.Connections.Add(from);
            }

            var pathsFound = new HashSet<string>();
            FindPaths(caves["start"], pathsFound);
            One = pathsFound.Count;

            pathsFound.Clear();

            foreach (var c in caves.Values
                .Where(c => c.IsSmall && c.Name is not ("end" or "start")))
            {
                foreach (var cave in caves.Values)
                {
                    cave.EnteredCount = 0;
                    cave.CanEnterTwice = false;
                }
                c.CanEnterTwice = true;
                FindPaths(caves["start"], pathsFound);
            }

            Two = pathsFound.Count;
        }

        private void FindPaths(Cave cave, HashSet<string> pathsFound,
            ISet<string>? visitedCaves = null, List<string>? currentPath = null)
        {
            visitedCaves ??= new HashSet<string>();
            currentPath ??= new() { "start" };

            if (cave.Name == "end")
            {
                pathsFound.Add(string.Join(",", currentPath));
                return;
            }

            cave.EnteredCount++;
            visitedCaves.Add(cave.Name);

            foreach (var con in cave.Connections)
            {
                if (!con.IsSmall || !visitedCaves.Contains(con.Name) || (con.CanEnterTwice && con.EnteredCount < 2))
                {
                    currentPath.Add(con.Name);
                    FindPaths(con, pathsFound, visitedCaves, currentPath);
                    currentPath.RemoveAt(currentPath.FindLastIndex(x => x == con.Name));
                }
            }

            cave.EnteredCount--;
            visitedCaves.Remove(cave.Name);
        }
    }
}
