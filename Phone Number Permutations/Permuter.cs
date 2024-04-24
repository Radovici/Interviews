using System.Collections.Generic;
using System.Text;

namespace Phone_Number_Permutations
{
    public class ChessPieces
    {
        public static IPiece Pawn => new Piece(new string[] { "u" });
        public static IPiece Rook => new Piece(new string[] { "U", "D", "R", "L" });
        public static IPiece Knight => new Piece(new string[] { "uur", "uul", "llu", "lld", "rru", "rrd", "ddl", "ddr" }); // Knight's L-shaped moves
        public static IPiece Bishop => new Piece(new string[] { "1", "2", "3", "4" }); // Diagonal moves
        public static IPiece Queen => new Piece(new IPiece[] { Rook, Bishop }); // Combination of rook and bishop moves
        public static IPiece King => new Piece(new string[] { "*" }); // King can move in any direction, including diagonals
    }

    public class Permuter
    {
        private IBoard _board;
        private IEnumerable<IPiece> _pieces;
        private IEnumerable<Func<string, bool>> _exclusions;
        private IEnumerable<Func<string, bool>> _terminators;
        public Permuter(IBoard board,
            IEnumerable<IPiece> pieces,
            IEnumerable<Func<string, bool>> exclusions,
            IEnumerable<Func<string, bool>> terminators)
        {
            _board = board;
            _pieces = pieces;
            _exclusions = exclusions;
            _terminators = terminators;
        }

        public IEnumerable<string> GetPermutations(IPiece piece)
        {
            List<string> allPermutations = new List<string>();
            IEnumerable<IBoardPosition> boardPositions = _board.AllPositions;
            foreach (IBoardPosition boardPosition in boardPositions)
            {
                char boardPositionCharacter = boardPosition.Value;
                string startPermutation = boardPositionCharacter.ToString();
                IEnumerable<string> permutations = GetPermutations(piece, boardPosition, startPermutation);
                allPermutations.AddRange(permutations);
            }
            HashSet<string> uniquePermutations = new HashSet<string>(allPermutations);
            IEnumerable<string> acceptableUniquePermutations = uniquePermutations.Where(p => _exclusions.Any(lmb => lmb(p))).ToList();
            return acceptableUniquePermutations;
        }

        private IEnumerable<string> GetPermutations(IPiece piece, IBoardPosition boardPosition, string startPermutation)
        {
            List<string> returnedPermutations = new List<string>();
            if (_terminators.Any(lmb => lmb(startPermutation)))
            {
                returnedPermutations.Add(startPermutation);
            }
            else
            { 
                IEnumerable<IBoardPosition> nextBoardPositions = _board.NextMoves(piece, boardPosition);
                foreach (IBoardPosition nextBoardPosition in nextBoardPositions)
                {
                    char boardPositionCharacter = nextBoardPosition.Value;
                    string permutation = startPermutation + boardPositionCharacter;
                    IEnumerable<string> permutations = GetPermutations(piece, nextBoardPosition, permutation);
                    returnedPermutations.AddRange(permutations);
                }
            }
            return returnedPermutations;
        }
    }

    public class Board : IBoard
    {
        private char[][] _boardValues;

        public Board(char[][] boardValues)
        {
            // Ensure non-empty board
            if (boardValues == null || boardValues.Length == 0)
            {
                throw new ArgumentException("Board must not be empty.");
            }

            // Initialize the board values
            _boardValues = boardValues;
        }


        public int Rows => _boardValues.Length;

        public int Cols => _boardValues[0].Length;

        public IEnumerable<IBoardPosition> AllPositions
        {
            get
            {
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Cols; col++)
                    {
                        yield return new BoardPosition(this, row, col);
                    }
                }
            }
        }

        public IEnumerable<IBoardPosition> NextMoves(IPiece piece, IBoardPosition currentBoardPosition)
        {
            foreach (string movePattern in piece.Moves)
            {
                int newX = currentBoardPosition.X;
                int newY = currentBoardPosition.Y;
                foreach ((int dx, int dy) in piece.GetValidMoves(movePattern))
                {
                     newX += dx;
                     newY += dy;
                }
                // Check if the new position is within the board bounds
                if (newX >= 0 && newX < Rows && newY >= 0 && newY < Cols)
                {
                    yield return new BoardPosition(this, newX, newY);
                }
            }
        }

        public class BoardPosition : IBoardPosition
        {
            private readonly Board _board;
            private readonly int _x;
            private readonly int _y;

            public BoardPosition(Board board, int x, int y)
            {
                _board = board;
                _x = x;
                _y = y;
            }

            public IBoard Board => _board;

            public int X => _x;

            public int Y => _y;

            public char Value => _board._boardValues[_x][_y];
        }
    }

    public interface IBoard
    {
        public int Rows { get; }
        public int Cols { get; }        
        IEnumerable<IBoardPosition> AllPositions { get; }
        IEnumerable<IBoardPosition> NextMoves(IPiece piece, IBoardPosition currentBoardPosition);
    }

    public interface IBoardPosition
    {
        public IBoard Board { get; }
        public int X { get; }
        public int Y { get; }
        public char Value { get; }
    }

    public class Piece : IPiece
    {
        IEnumerable<string> _moves;

        public Piece(string[] moves)
        {
            _moves = new List<string>(moves);
        }

        public Piece(IPiece[] pieces)
        {
            _moves = pieces.SelectMany(lmb => lmb.Moves).ToList();
        }

        private readonly Dictionary<char, (int, int)> moveMap = new Dictionary<char, (int, int)>
        {
            {'u', (0, +1)},
            {'d', (0, -1)},
            {'r', (+1, 0)},
            {'l', (-1, 0)},
            {'1', (-1, +1)},
            {'2', (+1, +1)},
            {'3', (+1, -1)},
            {'4', (-1, -1)}
        };

        public IEnumerable<string> Moves { get { return _moves; } }

        public IEnumerable<(int, int)> GetValidMoves(string movePattern)
        {
            foreach (char move in movePattern)
            {
                if (moveMap.TryGetValue(move, out var offset))
                {
                    yield return offset;
                }
                else
                {
                    throw new ArgumentException($"Invalid move: {move}");
                }
            }
        }
    }

    public interface IPiece // chess piece that identifies moves
    {
        public IEnumerable<(int, int)> GetValidMoves(string movePattern);

        public IEnumerable<string> Moves { get; }
    }
}
