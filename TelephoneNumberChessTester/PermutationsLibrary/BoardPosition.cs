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
            private readonly int _x;
            private readonly int _y;

            public BoardPosition(IBoard board, int x, int y)
            {
                _board = board;
                _x = x;
                _y = y;
            }

            public IBoard Board => _board;

            public int X => _x;

            public int Y => _y;

            public char Value => _board.BoardValues[_x][_y];
        }
    }
}
