using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{
    public partial class Board : IBoard
    {
        public const char WILDCARD = '*';
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

        public char[][] BoardValues { get { return _boardValues; } }

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

        public IEnumerable<IBoardPosition> NextMoves(IBoardPiece piece, IBoardPosition currentBoardPosition)
        {
            foreach (string movePattern in piece.Moves)
            {
                int newX = currentBoardPosition.X;
                int newY = currentBoardPosition.Y;
                if (movePattern == WILDCARD.ToString())
                {
                    foreach ((int dx, int dy) in piece.GetValidMoves(movePattern))
                    {
                        newX = currentBoardPosition.X + dx;
                        newY = currentBoardPosition.Y + dy;
                        if (movePattern.ToUpper() == movePattern) // uppercase, iterate over each change, also wildcard
                        {
                            // Check if the new position is within the board bounds
                            if (newX >= 0 && newX < Rows && newY >= 0 && newY < Cols)
                            {
                                yield return new BoardPosition(this, newX, newY);
                            }
                        }
                    }
                }
                else
                {
                    foreach ((int dx, int dy) in piece.GetValidMoves(movePattern))
                    {
                        newX = newX + dx;
                        newY = newY + dy;
                        if (movePattern.ToUpper() == movePattern) // uppercase, iterate over each change, also wildcard
                        {
                            // Check if the new position is within the board bounds
                            if (newX >= 0 && newX < Rows && newY >= 0 && newY < Cols)
                            {
                                yield return new BoardPosition(this, newX, newY);
                            }
                        }
                    }
                    if (movePattern.ToUpper() != movePattern) // lowercase, keep the last move
                    {
                        // Check if the new position is within the board bounds
                        if (newX >= 0 && newX < Rows && newY >= 0 && newY < Cols)
                        {
                            yield return new BoardPosition(this, newX, newY);
                        }
                    }
                }
            }
        }
    }
}
