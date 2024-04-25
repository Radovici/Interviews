using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationsLibrary
{
    public interface IBoardPosition
    {
        public IBoard Board { get; }
        public int X { get; }
        public int Y { get; }
        public char Value { get; }
    }
}
