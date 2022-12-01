using System.Text;

namespace AOC2021.Puzzles;

internal class Puzzle23 : Puzzle<int>
{
    readonly struct Board
    {
        readonly char[] _hall;
        readonly Stack<char>[] _rooms;
        public readonly int Cost;
        public readonly int RoomSize;

        public Board(char[] hall, Stack<char>[] rooms, int cost, int roomSize)
        {
            _hall = hall;
            _rooms = rooms;
            Cost = cost;
            RoomSize = roomSize;
        }

        public Board(string state)
        {
            var splits = state.Split('|');
            _hall = splits[0].ToCharArray();
            Cost = int.Parse(splits[2]);
            RoomSize = int.Parse(splits[3]);
            var rooms = splits[1].Split(',');
            _rooms = Enumerable.Range(0, 4)
                .Select(x => new Stack<char>(rooms[x].Reverse()))
                .ToArray();
        }

        public override int GetHashCode() => HashCode.Combine(_hall, _rooms);

        public (string State, int Cost) ToStateString()
        {
            return ToStateString(this);
        }

        private (string State, int Cost) ToStateString(Board b)
        {
            var hallway = new string(b._hall);
            var rooms = string.Join(',', b._rooms.Select(r => string.Concat(r)));
            return (hallway + "|" + rooms + "|" + b.Cost + "|" + b.RoomSize, b.Cost);
        }

        internal IEnumerable<(string State, int Cost)> GetPossibleMoves()
        {
            var newBoards = FromHallToRoom();

            return (newBoards.Count > 0
                ? newBoards
                : FromRoomToHall()
            ).Select(ToStateString);
        }

        private List<Board> FromRoomToHall()
        {
            var newBoards = new List<Board>();
            for (int r = 0; r < _rooms.Length; r++)
            {
                var room = _rooms[r];
                var expectedAmph = r switch
                {
                    0 => 'A',
                    1 => 'B',
                    2 => 'C',
                    3 => 'D',
                    _ => ' '
                };

                if (room.Count == 0 || room.All(c => c == expectedAmph))
                {
                    continue;
                }

                var roomIdx = r*2 + 2;

                for (int hallSpace = 0; hallSpace < _hall.Length; hallSpace++)
                {
                    if (hallSpace is (2 or 4 or 6 or 8))
                    {
                        continue;
                    }

                    var amph = _hall[hallSpace];
                    if (amph != '.' ||
                        (roomIdx < hallSpace && _hall[roomIdx..(hallSpace+1)].Any(c => c != '.')) ||
                        (roomIdx > hallSpace && _hall[hallSpace..(roomIdx+1)].Any(c => c != '.')))
                    {
                        continue;
                    }

                    var newRoom = CopyStack(room);
                    var amphToMove = newRoom.Pop();
                    int roomSteps = RoomSize - newRoom.Count;

                    var newHall = new char[_hall.Length];
                    _hall.CopyTo(newHall, 0);
                    newHall[hallSpace] = amphToMove;

                    var newRooms = new Stack<char>[_rooms.Length];
                    _rooms.CopyTo(newRooms, 0);
                    newRooms[r] = newRoom;

                    var amphipod = newHall[hallSpace];
                    var amphipodCost = AmphipodCost(amphipod);

                    newBoards.Add(new Board(newHall, newRooms, Cost + (amphipodCost * (roomSteps + Math.Abs(hallSpace - roomIdx))), RoomSize));
                }
            }

            return newBoards;
        }

        private List<Board> FromHallToRoom()
        {
            var newBoards = new List<Board>();
            for (int i = 0; i < _hall.Length; i++)
            {
                var amph = _hall[i];
                if (amph == '.')
                {
                    continue;
                }

                var roomDestIdx = GetRoomIdx(amph);
                var roomDest = _rooms[roomDestIdx];
                var roomIdx = roomDestIdx*2 + 2;

                if ((roomDest.Count > 0 && roomDest.Any(c => c != amph)) ||
                    (roomIdx < i && _hall[roomIdx..i].Any(c => c != '.')) ||
                    (roomIdx > i && _hall[(i+1)..(roomIdx+1)].Any(c => c != '.')))
                {
                    continue;
                }

                var newHall = new char[_hall.Length];
                _hall.CopyTo(newHall, 0);
                newHall[i] = '.';

                var newRoom = CopyStack(roomDest);
                newRoom.Push(amph);
                int roomSteps = (RoomSize + 1) - newRoom.Count;

                var newRooms = new Stack<char>[_rooms.Length];
                _rooms.CopyTo(newRooms, 0);
                newRooms[roomDestIdx] = newRoom;

                newBoards.Add(new Board(newHall, newRooms, Cost + (AmphipodCost(amph) * (roomSteps + Math.Abs(roomIdx - i))), RoomSize));
            }

            return newBoards;
        }

        internal bool IsCompleted(string expected) => 
            expected == string.Concat(_rooms.Select(r => string.Concat(r)));

        private static int AmphipodCost(char amphipod) => amphipod switch
        {
            'A' => 1,
            'B' => 10,
            'C' => 100,
            'D' => 1000,
            _ => -1
        };

        private static int GetRoomIdx(char amph) => amph switch
        {
            'A' => 0,
            'B' => 1,
            'C' => 2,
            'D' => 3,
            _ => -1
        };

        private static Stack<T> CopyStack<T>(Stack<T> original)
        {
            var arr = new T[original.Count];
            original.CopyTo(arr, 0);
            Array.Reverse(arr);
            return new Stack<T>(arr);
        }
    }

    protected override void Solve(string[] lines)
    {
        var hall = lines[1].Where(c => c == '.').ToArray();

        var roomsByLine = lines
            .SelectMany(x => x)
            .Where(char.IsLetter)
            .ToList();

        var rooms = roomsByLine.Zip(roomsByLine.Skip(4))
            .Select(a => new Stack<char>(new char[] { a.Second, a.First}))
            .ToArray();

        One = GetLowestWin(new Board(hall, rooms, 0, 2));

        rooms = rooms
            .Select((r, idx) =>
            {
                var room = r.ToList();
                if (idx == 0)
                {
                    room.Insert(1, 'D');
                    room.Insert(2, 'D');
                }
                if (idx == 1)
                {
                    room.Insert(1, 'C');
                    room.Insert(2, 'B');
                }
                if (idx == 2)
                {
                    room.Insert(1, 'B');
                    room.Insert(2, 'A');
                }

                if (idx == 3)
                {
                    room.Insert(1, 'A');
                    room.Insert(2, 'C');
                }
                room.Reverse();
                return new Stack<char>(room);
            })
            .ToArray();

        Two = GetLowestWin(new Board(hall, rooms, 0, 4));
    }

    private static int GetLowestWin(Board initialState)
    {
        var expected = new char[] { 'A', 'B', 'C', 'D' }
              .Aggregate(new StringBuilder(), (a, b) => a
                .Append(new string(b, initialState.RoomSize)))
              .ToString();

        var queue = new PriorityQueue<string, int>();

        queue.Enqueue(initialState.ToStateString().State, 0);

        var visitedBoards = new HashSet<string>();
        while (queue.TryDequeue(out var p, out _))
        {
            if (!visitedBoards.Add(p))
            {
                continue;
            }

            var board = new Board(p);


            if (board.IsCompleted(expected))
            {
                return board.Cost;
            }

            foreach (var (State, Cost) in board.GetPossibleMoves())
            {
                queue.Enqueue(State, Cost);
            }
        }

        return int.MaxValue;
    }
}
