namespace AOC2021.Puzzles;

internal class Puzzle18 : Puzzle<long>
{
    interface INode
    {
        long Magnitude();
    }

    class Node: INode
    {
        public INode Left { get; set; }
        public INode Right { get; set; }
        public Node(INode left, INode right) => (Left, Right) = (left, right);

        public override string ToString() => $"[{Left},{Right}]";
        public long Magnitude() => 3 * Left!.Magnitude() + 2 * Right!.Magnitude();

        public static Node operator +(Node left, Node right) 
        {
            var res = new Node(left, right);

            bool reduced, splitted;
            do
            {
                reduced = Reduce(res);
                splitted = Split(res);
            } while (reduced == true || splitted == true);
            return res;
        } 
    }

    class Leaf: INode
    {
        public int Value { get; set; }

        public Leaf(int value) => Value = value;
        public override string ToString() => Value.ToString();
        public long Magnitude() => Value;
    }

    protected override void Solve(string[] lines)
    {
        One = lines
            .Select(CreateNode)
            .Aggregate((Node?)null, (sum, node) =>
               sum == null ? node : sum + node)
            !.Magnitude();
        Two = Enumerable.Range(0, lines.Length)
            .SelectMany(i => Enumerable.Range(0, lines.Length).Select(j => (i, j)))
            .Where(x => x.i != x.j)
            .Max(x => (CreateNode(lines[x.i]) + CreateNode(lines[x.j])).Magnitude());
    }


    private static Node CreateNode(string value)
    {
        INode? left = null, right = null;

        if (value[1] == '[')
        {
            var endIdx = FindIndex(value[1..], '[', ']') + 1;
            left = CreateNode(value[1..(endIdx + 1)]);
        }
        else
        {
            left = new Leaf(value[1] - '0');
        }

        if (value[^2] == '[')
        {
            var startIdx = value.LastIndexOf(',');
            right = CreateNode(value[startIdx..^2]);
        }
        else if (value[^2] == ']')
        {
            var startIdx = value.Length - 2 - FindIndex(string.Concat(value[..^1].Reverse()), ']', '[');
            right = CreateNode(value[startIdx..^1]);
        }
        else
        {
            right = new Leaf(value[^2] - '0');
        }

        return new Node(left, right);
    }

    private static int FindIndex(string value, char push, char pop)
    {
        var stack = new Stack<char>();

        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == push)
            {
                stack.Push(value[i]);
            }
            else if (value[i] == pop)
            {
                stack.Pop();
            }

            if (stack.Count == 0)
            {
                return i;
            }
        }

        return 0;
    }

    private static bool Reduce(Node node)
    {
        var reduced = false;
        while (true)
        {
            int depth = 0;
            
            var child = FindLeaf(node, ref depth);
            
            if (child == null || depth < 4)
            {
                break;
            }

            var leftVal = ((Leaf) child.Left).Value;
            var rightVal = ((Leaf) child.Right).Value;

            var parent = FindParent(node, child);
            if (parent != null)
            {
                var leaf = new Leaf(0);
                reduced = true;
                if (parent.Left == child)
                {
                    parent.Left = leaf;
                }
                else
                {
                    parent.Right = leaf;
                }

                var leftNode = LeftOf(node, leaf);
                var rightNode = RightOf(node, leaf);
                if (leftNode != null)
                {
                    leftNode.Value += leftVal;
                }
                if (rightNode != null)
                {
                    rightNode.Value += rightVal;
                }
            }
        }

        return reduced;
    }

    private static bool Split(INode node)
    {
        if (node is not Node nl)
        {
            return false;
        }

        static Node CreateNode(int value) => new(
            new Leaf((int)Math.Floor(value / 2.0)),
            new Leaf((int)Math.Ceiling(value / 2.0))
        );

        if (nl.Left is Leaf ll && ll.Value >= 10)
        {
            nl.Left = CreateNode(ll.Value);
            return true;
        }
        
        if (Split(nl.Left))
        {
            return true;
        }

        if (nl.Right is Leaf lr && lr.Value >= 10)
        {
            nl.Right = CreateNode(lr.Value);
            return true;
        }

        if (Split(nl.Right))
        {
            return true;
        };

        return false;
    }

    private static Node? FindLeaf(Node node, ref int depth)
    {
        depth++;

        if (node is Node n)
        {
            if (n.Left is Node nl)
            {
                var res = FindLeaf(nl, ref depth);
                if (res != null)
                {
                    return res;
                }
            }
            if (n.Right is Node nr)
            {
                var res = FindLeaf(nr, ref depth);
                if (res != null)
                {
                    return res;
                }
            }
        }

        if (depth > 4)
        {
            return node;
        }

        depth--;

        return null;
    }

    private static Node? FindParent(Node root, Node child)
    {
        if (root.Left is Node nl)
        {
            if (nl == child)
            {
                return root;
            }

            var par = FindParent(nl, child);
            if (par != null)
            {
                return par;
            }
        }

        if (root.Right is Node nr)
        {
            if (nr == child)
            {
                return root;
            }

            var par = FindParent(nr, child);
            if (par != null)
            {
                return par;
            }
        }

        return null;
    }

    static Leaf? LeftOf(INode node, Leaf source)
    {
        var nodes = AllLeafs(node);

        var idx = nodes.IndexOf(source);
        if (idx == 0)
        {
            return null;
        }

        return nodes[idx - 1];
    }

    static Leaf? RightOf(INode node, Leaf source)
    {
        var nodes = AllLeafs(node);

        var idx = nodes.IndexOf(source);
        if (idx == nodes.Count -1)
        {
            return null;
        }

        return nodes[idx + 1];
    }
        
    static List<Leaf> AllLeafs(INode node)
    {
        var nodes = new List<Leaf>();
        if (node is Leaf l)
        {
            nodes.Add(l);
        }

        if (node is Node n)
        {
            nodes.AddRange(AllLeafs(n.Left));
            nodes.AddRange(AllLeafs(n.Right));
        }

        return nodes;
    }
}
