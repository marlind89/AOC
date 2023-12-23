namespace AOC.Helpers.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<IReadOnlyCollection<T>> SlidingWindow<T>(
        this IEnumerable<T> source, int amount)
    {
        var enumerators = Enumerable.Range(0, amount)
            .Select(x => source.Skip(x).GetEnumerator())
            .ToList();

        while (enumerators.All(x => x.MoveNext()))
        {
            yield return enumerators.Select(x => x.Current).ToList();
        }
    }

    public static long Lcm(this IEnumerable<int> numbers) => Maths.Lcm(numbers);

    public static IEnumerable<(T First, T Second)> AsPairs<T>(this IEnumerable<T> items)
    {
        var itemList = items.ToList();
        return itemList.SelectMany((x, idx) => itemList.Skip(idx + 1).Select(y => (x, y)));
    }
}
