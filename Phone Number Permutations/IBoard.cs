using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{
    public interface IBoard
    {
        public int Rows { get; }
        public int Cols { get; }
        public char[][] BoardValues { get; }
        public IEnumerable<IBoardPosition> AllPositions { get; }
        public IEnumerable<IBoardPosition> NextMoves(IBoardPiece piece, IBoardPosition currentBoardPosition);
    }
}
