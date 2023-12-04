using System.Text.RegularExpressions;

namespace AOC2023.Puzzles;

internal partial class Puzzle4 : Puzzle<int>
{
    [GeneratedRegex(@"Card\s*(\d+):")] private static partial Regex CardRegex();
    private record struct Card(ISet<int> WinningNumbers, ISet<int> Numbers);

    protected override void Solve(string[] lines)
    {
        var cards = lines.Select(line =>
        {
            var gameId = int.Parse(CardRegex().Match(line).Groups[1].Value);
            var winningNumbersStr = line[(1 + line.IndexOf(':'))..line.IndexOf('|')];
            var myNumbersStr = line[(1 + line.IndexOf('|'))..];
            return new Card(ParseNumbers(winningNumbersStr), ParseNumbers(myNumbersStr));
        }).ToList();

        One = cards.Sum(x => (int)Math.Pow(2, x.Numbers.Count(x.WinningNumbers.Contains) - 1));

        var cardCopies = cards.Select((x, i) => i).ToDictionary(x => x, _ => 1);
        foreach (var cardIdx in Enumerable.Range(0, cards.Count))
        {
            var card = cards[cardIdx];
            var wins = card.Numbers.Count(card.WinningNumbers.Contains);
            var existingCards = cardCopies.GetValueOrDefault(cardIdx);
            foreach (var cardWin in Enumerable.Range(cardIdx + 1, wins))
            {
                cardCopies[cardWin] += existingCards;
            }
        }

        Two = cardCopies.Values.Sum();
    }

    private ISet<int> ParseNumbers(string numbersString) => numbersString
        .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .ToHashSet();
}
