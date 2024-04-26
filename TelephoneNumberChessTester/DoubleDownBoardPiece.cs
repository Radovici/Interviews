using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{ 
    public class DoubleDownBoardPiece : IBoardPiece
    {
        private readonly IBoard _board;

        public DoubleDownBoardPiece(IBoard board)
        {
            _board = board;
        }

        public string Name => "DoubleDown";
        public IEnumerable<string> Moves { get { return new string[] { Name }; } }

        public IEnumerable<(int, int)> GetValidMoves(IBoardPosition boardPosition, string movePattern)
        {
            int boardPositionValue;
            if (!int.TryParse(boardPosition.Value.ToString(), out boardPositionValue))
            {
                boardPositionValue = 1;
            }
            IBoardPosition newBoardPosition = _board.AllPositions.Single(lmb => lmb.Value.ToString() == ((boardPositionValue * 2) % 10).ToString());
            (int, int) offset = (newBoardPosition.RowIndex - boardPosition.RowIndex, newBoardPosition.ColIndex - boardPosition.ColIndex);
            if (IsWithinBoard(boardPosition, offset))
            {
                yield return offset; //return final offset / delta move
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
