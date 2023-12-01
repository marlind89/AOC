namespace AOC.Helpers;

public interface IPuzzle
{
    int PuzzleNumber { get; }
    void Solve();
    IEnumerable<string> ValidateAnswers(string one, string two);
}
