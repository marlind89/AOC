namespace AOC2022.Puzzles;

internal class Puzzle2 : Puzzle<int>
{
    enum GameResult { Win = 6, Draw = 3, Loss = 0 }
    enum Hand { Rock, Paper, Scissor}
    private static readonly IReadOnlyCollection<Hand> Hands = Enum.GetValues(typeof(Hand)).Cast<Hand>().ToList();

    protected override void Solve(string[] lines)
    {
        One = lines.Sum(line =>
        {
            var hands = line.Split(" ").Select(ParseHand).ToList();
            return GetGameScore(hands[0], hands[1]);
        });
        Two = lines.Sum(line =>
        {
            var commands = line.Split(" ");
            return GetGameScore(ParseHand(commands[0]), ParseOutcome(commands[1]));
        });
    }

    private static Hand ParseHand(string hand) => hand switch
    {
        "A" or "X" => Hand.Rock,
        "B" or "Y" => Hand.Paper,
        "C" or "Z" => Hand.Scissor,
        _ => throw new Exception("Invalid hand")
    };

    private static GameResult ParseOutcome(string outcome) => outcome switch
    {
        "X" => GameResult.Loss,
        "Y" => GameResult.Draw,
        "Z" => GameResult.Win,
        _ => throw new Exception("Invalid game result")
    };

    private static int GetGameScore(Hand opponent, Hand my) => ((int) ((opponent, my) switch
    {
        (Hand.Rock,    Hand.Paper)   => GameResult.Win,
        (Hand.Paper,   Hand.Scissor) => GameResult.Win,
        (Hand.Scissor, Hand.Rock)    => GameResult.Win,
        _ when opponent == my => GameResult.Draw,
        _ => GameResult.Loss
    })) + (int) my + 1;

    private static int GetGameScore(Hand opponent, GameResult result) => (int) (result switch
    {
        GameResult.Win => opponent.Next(Hands),
        GameResult.Loss => opponent.Previous(Hands),
        _ => opponent,
    }) + (int) result + 1;
}