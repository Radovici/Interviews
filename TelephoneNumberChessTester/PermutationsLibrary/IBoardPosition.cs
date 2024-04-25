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
        public int RowIndex { get; }
        public int ColIndex { get; }
        public char Value { get; }
    }
}
