namespace AOC2022.Puzzles;

internal partial class Puzzle7 : Puzzle<int>
{
    interface INode
    {
        Directory? Parent { get; }
        string Filename { get; }
        int Size { get; }
    }

    record Directory(string Filename, Directory? Parent) : INode
    {
        public List<INode> Children { get; } = new List<INode>();
        public int Size => Children.Sum(c => c.Size);
    }

    record File(string Filename, int Size, Directory? Parent): INode;

    protected override void Solve(string[] lines)
    {
        var rootDir = lines.Skip(1).Aggregate(
            new Directory("/", null), (currentDir, line) =>
        {
            if (line[0] != '$')
            {
                currentDir.Children.Add(ParseNode(line, currentDir));
                return currentDir;
            }

            if (line.StartsWith("$ cd"))
            {
                var dirTarget = line.Split(' ').Last();
                return dirTarget switch
                {
                    ".." => currentDir.Parent!,
                    _ => currentDir.Children.OfType<Directory>().First(x => x.Filename == dirTarget)
                };
            }

            return currentDir;
        }, GetRoot);

        One = GetDirectories(rootDir).Where(dir => dir.Size <= 100000).Sum(c => c.Size);
        Two = GetDirectories(rootDir).Where(dir => dir.Size > (rootDir.Size - 40000000))
            .OrderBy(x => x.Size).First().Size;
    }

    private static INode ParseNode(string filename, Directory parent)
    {
        var split = filename.Split(' ');
        return filename.StartsWith("dir")
            ? new Directory(split[1], parent)
            : new File(split[1], int.Parse(split[0]), parent);
    }
    
    private static IEnumerable<Directory> GetDirectories(Directory dir) => dir.Children
        .OfType<Directory>().SelectMany(GetDirectories).Append(dir);

    private static Directory GetRoot(INode node) => node.Parent != null ? GetRoot(node.Parent) : (Directory) node;
}
