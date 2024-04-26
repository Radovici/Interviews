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

            public BoardPiece(IBoard board, string name, IBoardPiece[] pieces)
                : this(board, name, pieces.SelectMany(lmb => lmb.Moves).ToArray())
            {
            }

            /// <summary>
            /// (row, column)
            /// </summary>
            private readonly Dictionary<char, (int, int)> moveMap = new Dictionary<char, (int, int)>
            {
                {'u', (-1, 0)}, //NOTE: up is negative! becasue [0,0] is the top-left corner!
                {'d', (+1, 0)},
                {'r', (0, +1)},
                {'l', (0, -1)},
                {'q', (-1, -1)},
                {'w', (-1, +1)},
                {'s', (+1, +1)},
                {'a', (+1, -1)}
            };

            public string Name => _name;
            public IEnumerable<string> Moves { get { return _moves; } }

            public IEnumerable<(int, int)> GetValidMoves(IBoardPosition boardPosition, string movePattern)
            {
                (int, int) totalOffset = (0, 0); //no move yet
                foreach (char move in movePattern)
                {
                    if (moveMap.TryGetValue(char.ToLower(move), out var offset))
                    {
                        // Handle uppercase moves indicating multiple steps
                        if (char.IsUpper(move))
                        {
                            // Convert uppercase move to lowercase and repeat the move
                            int repeatCount = 1;
                            while (true)
                            {
                                var repeatedOffset = (offset.Item1 * repeatCount, offset.Item2 * repeatCount); //repeated is for determining if off-board
                                repeatCount++;
                                if (!IsWithinBoard(boardPosition, repeatedOffset))
                                {
                                    break;
                                }
                                yield return offset; //same incremental offset
                            }
                        }
                        else
                        {
                            totalOffset = (totalOffset.Item1 + offset.Item1, totalOffset.Item2 + offset.Item2); //yield return offset;
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid move: {move}");
                    }
                }
                if (IsWithinBoard(boardPosition, totalOffset)
                    && !(totalOffset.Item1 == 0 && totalOffset.Item2 == 0))
                {
                    yield return totalOffset; //return final offset / delta move
                }                
            }

            public override string ToString()
            {
                return Name;
            }

            private bool IsWithinBoard(IBoardPosition boardPosition, (int, int) offset)
            {
                // Check if the move goes out of the board bounds
                int newRowIndex = boardPosition.RowIndex + offset.Item1;
                int newColIndex = boardPosition.ColIndex + offset.Item2;
                return newRowIndex >= 0 && newRowIndex < _board.Rows && newColIndex >= 0 && newColIndex < _board.Cols; // Assuming 8x8 chessboard
            }
        }
    }
}
