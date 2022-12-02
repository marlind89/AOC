namespace AOC.Helpers.Extensions
{
    public static class EnumExtensions
    {

        public static T Next<T>(this T v)
            where T : Enum
        {
            return v.Next(Enum.GetValues(v.GetType()).Cast<T>());
        }

        public static T Next<T>(this T v, IEnumerable<T> values)
            where T : Enum
        {
            return values
                .Concat(new[] { default(T)! })
                .SkipWhile(e => !v.Equals(e))
                .Skip(1)
                .First();
        }

        public static T Previous<T>(this T v)
            where T : Enum
        {
            return v.Previous(Enum.GetValues(v.GetType()).Cast<T>());
        }

        public static T Previous<T>(this T v, IEnumerable<T> values)
            where T : Enum
        {
            return values
                .Concat(new[] { default(T)! })
                .Reverse()
                .SkipWhile(e => !v.Equals(e))
                .Skip(1)
                .First();
        }
    }
}
