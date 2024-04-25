using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{
    public partial class Board : IBoard
    {
        public class BoardPiece : IBoardPiece
        {
            private readonly IBoard _board;
            private readonly string _name;
            IEnumerable<string> _moves;

            public BoardPiece(IBoard board, string name, string[] moves)
            {
                _board = board;
                _name = name;
                _moves = new List<string>(moves);
            }

            public BoardPiece(IBoardPiece[] pieces)
            {
                _moves = pieces.SelectMany(lmb => lmb.Moves).ToList();
            }

            private readonly Dictionary<char, (int, int)> moveMap = new Dictionary<char, (int, int)>
            {
                {'u', (+1, 0)},
                {'d', (-1, 0)},
                {'r', (0, +1)},
                {'l', (0, -1)},
                {'1', (+1, -1)},
                {'2', (+1, +1)},
                {'3', (-1, +1)},
                {'4', (-1, -1)}
            };

            public string Name => _name;
            public IEnumerable<string> Moves { get { return _moves; } }

            public IEnumerable<(int, int)> GetValidMoves(string movePattern)
            {
                foreach (char move in movePattern)
                {
                    if (move == Board.WILDCARD)
                    {
                        // For the King piece, return all possible moves
                        foreach (var offset in moveMap.Values)
                        {
                            yield return offset;
                        }
                    }
                    else if (moveMap.TryGetValue(char.ToLower(move), out var offset))
                    {
                        // Handle uppercase moves indicating multiple steps
                        if (char.IsUpper(move))
                        {
                            // Convert uppercase move to lowercase and repeat the move
                            int repeatCount = 1;
                            while (true)
                            {
                                var repeatedOffset = (offset.Item1 * repeatCount, offset.Item2 * repeatCount);
                                yield return repeatedOffset;

                                repeatCount++;
                                if (!IsWithinBoard(repeatedOffset))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            yield return offset;
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid move: {move}");
                    }
                }
            }

            public override string ToString()
            {
                return Name;
            }

            private bool IsWithinBoard((int, int) offset)
            {
                // Check if the move goes out of the board bounds
                int newX = offset.Item1;
                int newY = offset.Item2;
                return newX >= 0 && newX < _board.Cols && newY >= 0 && newY < _board.Rows; // Assuming 8x8 chessboard
            }
        }
    }
}
