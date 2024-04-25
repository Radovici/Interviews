using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using static PermutationsLibrary.Board;

namespace PermutationsLibrary
{
    public class Permuter
    {
        private IBoard _board;
        private IEnumerable<IBoardPiece> _pieces;
        private IEnumerable<Func<string, bool>> _exclusions;
        private IEnumerable<Func<string, bool>> _terminators;
        public Permuter(IBoard board,
            IEnumerable<IBoardPiece> pieces,
            IEnumerable<Func<string, bool>> exclusions,
            IEnumerable<Func<string, bool>> terminators)
        {
            _board = board;
            _pieces = pieces;
            _exclusions = exclusions;
            _terminators = terminators;
        }

        public IEnumerable<string> GetPermutations(IBoardPiece piece)
        {
            List<string> allPermutations = new List<string>();
            IEnumerable<IBoardPosition> boardPositions = _board.AllPositions;
            foreach (IBoardPosition boardPosition in boardPositions)
            {
                char boardPositionCharacter = boardPosition.Value;
                string startPermutation = boardPositionCharacter.ToString();
                IEnumerable<string> permutations = GetPermutations(piece, boardPosition, startPermutation);
                allPermutations.AddRange(permutations);
            }
            HashSet<string> uniquePermutations = new HashSet<string>(allPermutations);
            IEnumerable<string> excludedPermutations = uniquePermutations.Where(p => _exclusions.Any(lmb => lmb(p))).ToList();
            IEnumerable<string> acceptedPermutations = uniquePermutations.Except(excludedPermutations);
            return acceptedPermutations;
        }

        private IEnumerable<string> GetPermutations(IBoardPiece piece, IBoardPosition boardPosition, string startPermutation)
        {
            List<string> returnedPermutations = new List<string>();
            if (_exclusions.Any(lmb => lmb(startPermutation)))
            {
                // do nothing, don't add
            }
            else if (_terminators.Any(lmb => lmb(startPermutation)))
            {
                returnedPermutations.Add(startPermutation);
            }
            else
            { 
                IEnumerable<IBoardPosition> nextBoardPositions = _board.NextMoves(piece, boardPosition);
                foreach (IBoardPosition nextBoardPosition in nextBoardPositions)
                {
                    char boardPositionCharacter = nextBoardPosition.Value;
                    string permutation = startPermutation + boardPositionCharacter;
                    IEnumerable<string> permutations = GetPermutations(piece, nextBoardPosition, permutation);
                    returnedPermutations.AddRange(permutations);
                }
            }
            return returnedPermutations;
        }
    }
}
