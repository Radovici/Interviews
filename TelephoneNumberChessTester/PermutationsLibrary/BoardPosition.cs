using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{
    public partial class Board : IBoard
    {
        public class BoardPosition : IBoardPosition
        {
            private readonly IBoard _board;
            private readonly int _rowIndex;
            private readonly int _colIndex;

            public BoardPosition(IBoard board, int rowIndex, int colIndex)
            {
                _board = board;
                _rowIndex = rowIndex;
                _colIndex = colIndex;
            }

            public IBoard Board => _board;

            public int RowIndex => _rowIndex;

            public int ColIndex => _colIndex;

            public char Value => _board.BoardValues[_rowIndex][_colIndex];

            public override string ToString()
            {
                return $"[{RowIndex},{ColIndex}]={Value}";
            }
        }
    }
}
