namespace AOC2023.Puzzles;

internal class Puzzle7 : Puzzle<int>
{
    private static readonly List<char> RegularCards = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];
    private static readonly List<char> JokerCards = ['J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A'];

    public enum Type
    {
        HighCard,
        OnePair,
        TwoPairs,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    record Hand(string Cards, int Bid)
    {
        public Type GetHandType() => GetHandType(Cards);
        public Type GetHandTypeWithJokers()
        {
            var cardsWithoutJokers = new string(Cards.Select((x, i) => (char)(x == 'J' ? (i + 'B') : x)).ToArray());
            var handType = GetHandType(cardsWithoutJokers);

            return (handType, Cards.Count(x => x == 'J')) switch
            {
                (Type.HighCard, 1) => Type.OnePair,
                (Type.HighCard, 2) => Type.ThreeOfAKind,
                (Type.HighCard, 3) => Type.FourOfAKind,
                (Type.HighCard, 4) => Type.FiveOfAKind,

                (Type.OnePair, 1) => Type.ThreeOfAKind,
                (Type.OnePair, 2) => Type.FourOfAKind,
                (Type.OnePair, 3) => Type.FiveOfAKind,

                (Type.TwoPairs, 1) => Type.FullHouse,
                (Type.TwoPairs, 2) => Type.FourOfAKind,

                (Type.ThreeOfAKind, 1) => Type.FourOfAKind,
                (Type.ThreeOfAKind, 2) => Type.FiveOfAKind,

                (Type.FullHouse, 1) => Type.FourOfAKind,
                (Type.FullHouse, 2) => Type.FiveOfAKind,

                (Type.FourOfAKind, 1) => Type.FiveOfAKind,
                (_, 5) => Type.FiveOfAKind,

                _ => handType
            };
        }

        public int GetPoints(List<char> availableCards)
        {
            var multiplier = (int)Math.Pow(availableCards.Count, 5);
            return Cards
                .Sum(x =>
                {
                    var points = availableCards.IndexOf(x) * multiplier;
                    multiplier /= availableCards.Count;
                    return points;
                });
        }

        private static Type GetHandType(string cards)
        {
            var groupedCards = cards
                .GroupBy(x => x, (_, vals) => vals.ToList())
                .OrderByDescending(x => x.Count)
                .ToList();

            return (groupedCards.Count, groupedCards[0].Count) switch
            {
                (1, _) => Type.FiveOfAKind,
                (2, 4) => Type.FourOfAKind,
                (2, _) => Type.FullHouse,
                (3, 3) => Type.ThreeOfAKind,
                (3, _) => Type.TwoPairs,
                (4, _) => Type.OnePair,
                _ => Type.HighCard
            }; ;
        }
    }

    protected override void Solve(string[] lines)
    {
        var hands = lines
            .Select(x =>
            {
                var split = x.Split(' ');
                return new Hand(split[0], int.Parse(split[1]));
            })
            .ToList();

        One = hands
            .OrderBy(x => x.GetHandType())
            .ThenBy(x => x.GetPoints(RegularCards))
            .Select((x, i) => x.Bid * (i + 1))
            .Sum();

        Two = hands
            .OrderBy(x => x.GetHandTypeWithJokers())
            .ThenBy(x => x.GetPoints(JokerCards))
            .Select((x, i) => x.Bid * (i + 1))
            .Sum();
    }
}

