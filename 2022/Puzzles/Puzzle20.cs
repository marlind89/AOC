namespace AOC2022.Puzzles;

internal class Puzzle20 : Puzzle<long>
{
    record struct Number(int Idx, long Val);

    protected override void Solve(string[] lines)
    {
        var numbers = lines.Select((x, idx) => new Number(idx, int.Parse(x))).ToList();

        One = Solve(numbers, 1, 1);
        Two = Solve(numbers, 811589153, 10);
    }

    private static long Solve(List<Number> source, long multiplyer, int mixAmount)
    {
        if (multiplyer != 1)
        {
            source = source.Select(x => new Number(x.Idx, x.Val * multiplyer)).ToList();
        }

        var numbers = new LinkedList<Number>(source);
        LinkedListNode<Number> start = null!;

        foreach (var number in Enumerable.Range(0, mixAmount).SelectMany(_ => source))
        {
            var prevNode = numbers.Find(number)!;

            if (number.Val == 0)
            {
                start = prevNode;
                continue;
            }

            var range = Enumerable.Range(0, Math.Abs((int)(number.Val % (source.Count - 1))));
            var newNode = number.Val > 0
                ? range.Aggregate(prevNode, (a, _) => a.Next ?? numbers.First!)
                : range.Aggregate(prevNode, (a, _) => a.Previous ?? numbers.Last!);

            if (prevNode == newNode)
            {
                continue;
            }

            numbers.Remove(prevNode);

            if (number.Val > 0)
            {
                numbers.AddAfter(newNode, number);
            }
            else
            {
                numbers.AddBefore(newNode, number);
            }
        }

        return Enumerable.Range(1, 3).Sum(x => Enumerable.Range(0, x * 1000 % source.Count)
            .Aggregate(start, (a, b) => a.Next ?? numbers.First!)!.Value.Val);
    }
}
