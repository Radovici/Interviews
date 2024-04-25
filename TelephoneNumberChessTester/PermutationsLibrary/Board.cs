using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{
    public partial class Board : IBoard
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
                int newRowIndex = currentBoardPosition.RowIndex;
                int newColIndex = currentBoardPosition.ColIndex;
                foreach ((int deltaRowIndex, int deltaColIndex) in piece.GetValidMoves(currentBoardPosition, movePattern))
                {
                    newRowIndex = newRowIndex + deltaRowIndex;
                    newColIndex = newColIndex + deltaColIndex;
                    if (movePattern.ToUpper() == movePattern) // uppercase, iterate over each change
                    {
                        // Check if the new position is within the board bounds
                        if (newRowIndex >= 0 && newRowIndex < Rows && newColIndex >= 0 && newColIndex < Cols)
                        {
                            yield return new BoardPosition(this, newRowIndex, newColIndex);
                        }
                    }
                }
                if (movePattern.ToUpper() != movePattern) // lowercase, keep the last move
                {
                    // Check if the new position is within the board bounds
                    if (newRowIndex >= 0 && newRowIndex < Rows && newColIndex >= 0 && newColIndex < Cols)
                    {
                        yield return new BoardPosition(this, newRowIndex, newColIndex);
                    }
                }
            }
        }
    }
}
