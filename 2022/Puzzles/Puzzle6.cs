namespace AOC2022.Puzzles;

internal class Puzzle6 : Puzzle<int>
{
    protected override void Solve(string[] lines)
    {
        One = FindMarker(lines[0], 4);
        Two = FindMarker(lines[0], 14);
    }

    private static int FindMarker(string message, int windowLength) => message
        .SlidingWindow(windowLength)
        .Select((windowStr, idx) => new HashSet<char>(windowStr).Count == windowLength ? (idx + windowLength) : -1)
        .First(x => x > 0);
}
