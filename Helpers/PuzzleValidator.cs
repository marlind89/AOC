namespace AOC.Helpers;

internal static class PuzzleValidator
{
    private const string AnswersFile = "Answers.txt";
    
    internal static string? ValidatePuzzles(IEnumerable<IPuzzle> puzzles)
    {
        if (!File.Exists(AnswersFile))
        {
            return null;
        }

        var puzzleAnswers = ParseExpectedAnswers();
        return string.Join(Environment.NewLine, puzzles.SelectMany(p =>
        {
            if (!puzzleAnswers.TryGetValue(p.PuzzleNumber, out var answers))
            {
                return Enumerable.Empty<string>();
            }

            return p.ValidateAnswers(answers.One, answers.Two);
        }));
    }

    private static IDictionary<int, (string One, string Two)> ParseExpectedAnswers()
    {
        var answers = File.ReadAllText(AnswersFile);

        var puzzleAnswers = new Dictionary<int, (string One, string Two)>();
        var num = 0;
        while (num < 25)
        {
            var start = answers.IndexOf($"{++num}:") + num.ToString().Length + 1;
            if (start == -1)
            {
                continue;
            }

            var end = answers.IndexOf($"{num + 1}:");
            end = end == -1 ? answers.Length - 2 : end - 1;
            var parts = answers[start..(end + 1)].Split('|');
            puzzleAnswers[num] = (parts[0], parts.Length > 1 ? parts[1] : "");
        }

        return puzzleAnswers;
    }
}
