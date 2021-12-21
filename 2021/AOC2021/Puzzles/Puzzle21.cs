using System.Collections.Immutable;

namespace AOC2021.Puzzles
{
    internal class Puzzle21 : Puzzle<long>
    {
        record Player(int Position, int Score = 0)
        {
            public Player Move(int positions) 
            {
                var newPosition = ((Position + positions - 1) % 10) + 1;
                var newScore = Score + newPosition;
                return new Player(newPosition, newScore);
            }

            public bool IsWin(int max) => Score >= max;
        }

        record Game(Player Player1, Player Player2)
        {
            private static readonly int[] QuantumRolls =
                Enumerable.Range(1, 3)
                    .SelectMany(d1 => Enumerable.Range(1, 3)
                        .SelectMany(d2 => Enumerable.Range(1, 3)
                            .Select(d3 => d1 + d2 + d3)
                        )
                    )
                .ToArray();


            public int? GetWinner(int max)
            {
                if (Player1.IsWin(max))
                {
                    return 0;
                }

                if (Player2.IsWin(max))
                {
                    return 1;
                }
                    
                return default;
            }

            public Game PracticeMove(int player, int positions) => player switch
            {
                0 => this with { Player1 = Player1.Move(positions) },
                1 => this with { Player2 = Player2.Move(positions) },
                _ => throw new InvalidOperationException("Invalid player")
            };

            public IEnumerable<Game> QuantumMove(int player) => player switch
            {
                0 => QuantumRolls.Select(x => (this with { Player1 = Player1.Move(x) })),
                1 => QuantumRolls.Select(x => (this with { Player2 = Player2.Move(x) })),
                _ => Enumerable.Empty<Game>()
            };
        }

        protected override void Solve(string[] lines)
        {
            var initialPlayers = lines
                .Select((x, idx) => new Player(int.Parse(x.Split(' ')[^1])))
                .ToList();

            One = PlayPracticeGame(initialPlayers);
            Two = PlayQuantumGame(initialPlayers);
        }

        private static int PlayPracticeGame(IEnumerable<Player> initialPlayers)
        {
            var players = initialPlayers.ToArray();
            var game = new Game(players[0], players[1]);

            int dice = 1;
            var currPlayer = 0;
            while (true)
            {
                var roll = dice + dice + 1 + dice + 2;
                dice += 3;

                game = game.PracticeMove(currPlayer, roll);

                var winner = game.GetWinner(1000);
                if (winner != null)
                {
                    return Math.Min(game.Player1.Score, game.Player2.Score) * (dice - 1);
                }

                currPlayer = (currPlayer + 1) % players.Length;
            }
        }

        private static long PlayQuantumGame(IList<Player> initialPlayers)
        {
            var wins = new long[initialPlayers.Count];
            var games = ImmutableHashSet.Create<(Game game, long count)>(
              (new Game(initialPlayers[0], initialPlayers[1]), 1)
            );

            var currPlayer = 0;
            while (games.Count > 0)
            {
                var next = games
                    .SelectMany(a => a.game.QuantumMove(currPlayer)
                        .Select(newGame => (newGame, a.count)))
                    .GroupBy(a => a.newGame)
                    .Select(g => (game: g.Key, count: g.Sum(x => x.count)))
                    .ToImmutableHashSet();

                var newWins = next
                    .Select(g => (player: g.game.GetWinner(21), games: g))
                    .Where(x => x.player != null)
                    .ToList();

                if (newWins.Count > 0)
                {
                    foreach (var w in newWins)
                    {
                        wins[w.player!.Value] += w.games.count;
                    }

                    games = next.Except(newWins.Select(w => w.games));
                }
                else
                {
                    games = next;
                }
                
                currPlayer = (currPlayer + 1) % initialPlayers.Count;
            }

            return wins.Max();
        }
    }
}
