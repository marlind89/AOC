namespace AOC2023.Puzzles;

internal class Puzzle8 : Puzzle<long>
{
    record Node(string Name, string Left, string Right);

    protected override void Solve(string[] lines)
    {
        var instructions = lines[0];
        var nodes = lines[2..]
            .Select(x =>
            {
                var split = x.Split('=', StringSplitOptions.TrimEntries);
                var name = split[0];
                var nodeSplit = split[1].Split(", ");
                return new Node(name, nodeSplit[0][1..], nodeSplit[1][..^1]);
            })
            .ToDictionary(x => x.Name);

        One = WalkToGoal(instructions, nodes, nodes["AAA"], x => x == "ZZZ");
        Two = GhostWalk(instructions, nodes);
    }

    private static long GhostWalk(string instructions, Dictionary<string, Node> nodes)
    {
        return nodes.Values.Where(x => x.Name.EndsWith('A'))
            .Select(x => WalkToGoal(instructions, nodes, x, x => x.EndsWith('Z')))
            .Lcm();
    }

    private static int WalkToGoal(string instructions, Dictionary<string, Node> nodes,
        Node startNode, Func<string,bool> hasReachedGoal)
    {
        var (steps, idx, currentNode) = (0, 0, startNode);
        while (!hasReachedGoal(currentNode.Name))
        {
            var instruction = instructions[idx];
            idx = (idx + 1) % instructions.Length;
            currentNode = nodes[instruction == 'R' ? currentNode.Right : currentNode.Left];
            steps++;
        }
        return steps;
    }
}

