namespace AOC2021.Puzzles
{
    internal interface IPuzzle
    {
    }

    internal interface IPuzzle<TAnswer> : IPuzzle
    {
        TAnswer One();
        TAnswer Two();
    }
}
