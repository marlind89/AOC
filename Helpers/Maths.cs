namespace AOC.Helpers;

public static class Maths
{
    public static long Lcm(IEnumerable<int> numbers)
    {
        long lcm = numbers.First();

        foreach (var number in numbers.Skip(1))
        {
            lcm = Lcm(lcm, number);
        }

        return lcm;
    }

    public static long Lcm(long a, long b)
    {
        return (a / Gcf(a, b)) * b;
    }

    public static long Gcf(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
