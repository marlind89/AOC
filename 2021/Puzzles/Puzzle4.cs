namespace AOC2021.Puzzles;

internal class Puzzle4 : Puzzle<int>
{
    private class Square
    {
        public int Number { get; }
        public bool IsMarked { get; set; }

        public Square(int number) => Number = number;
    }

    protected override void Solve(string[] lines)
    {
        var numbers = lines[0].Split(',')
           .Select(int.Parse)
           .ToArray();

        var boards = lines.Skip(1)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(boardLine => boardLine
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new Square(int.Parse(x)))
                .ToArray())
            .Chunk(5)
            .ToArray();

        var bingoResults = PlayBingo(numbers, boards);

        One = bingoResults.First();
        Two = bingoResults.Last();
    }

    private static List<int> PlayBingo(int[] numbers, Square[][][] boards)
    {
        var winningResults = new List<int>();
        var playingBoards = boards.ToList();

        foreach (var number in numbers)
        {
            foreach (var board in playingBoards.ToList())
            {
                var numbersToCheck = board
                    .Select(line => line.FirstOrDefault(s => s.Number == number))
                    .Where(s => s != null);

                foreach (var numberToCheck in numbersToCheck)
                {
                    numberToCheck!.IsMarked = true;
                }

                if (HasRowWin(board) || HasColumnWin(board))
                {
                    var score = board.Sum(line => line.Where(s => !s.IsMarked).Sum(s => s.Number)) * number;
                    winningResults.Add(score);
                    playingBoards.Remove(board);
                }
            }

            if (playingBoards.Count == 0)
            {
                break;
            }
        }

        return winningResults;
    }

    private static bool HasRowWin(Square[][] board) =>
        board.Any(line => line.All(s => s.IsMarked));

    private static bool HasColumnWin(Square[][] board)
    {
        for (int col = 0; col < board.Length; col++)
        {
            var hasColumnWin = true;
            for (int row = 0; row < board[col].Length; row++)
            {
                if (!board[row][col].IsMarked)
                {
                    hasColumnWin = false;
                }
            }

            if (hasColumnWin)
            {
                return true;
            }
        }

        return false;
    }
}
