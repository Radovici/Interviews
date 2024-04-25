using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{
    public interface IBoardPiece // chess piece that identifies moves
    {
        public string Name { get; }
        public IEnumerable<(int, int)> GetValidMoves(IBoardPosition boardPosition, string movePattern);

        public IEnumerable<string> Moves { get; }
    }
}
