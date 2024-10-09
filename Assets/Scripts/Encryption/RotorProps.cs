using System.Collections.Generic;
using System.Linq;

namespace Encryption
{
    public record RotorProps
    {
        public Dictionary<char, char> Permutation
        {
            get;
        }

        public Dictionary<char, char> Inverse
        {
            get;
        }

        public char StepPosition
        {
            get;
        }

        public RotorProps(Dictionary<char, char> Permutation, char StepPosition)
        {
            this.Permutation = Permutation;
            this.Inverse = Permutation.ToDictionary(x => x.Value, x => x.Key);
            this.StepPosition = StepPosition;
        }
    };

    public static class Rotors
    {
        public static readonly RotorProps ROTOR_PROPS_1 = new(new Dictionary<char, char>
            {
                { 'A', 'E' },
                { 'B', 'K' },
                { 'C', 'M' },
                { 'D', 'F' },
                { 'E', 'L' },
                { 'F', 'G' },
                { 'G', 'D' },
                { 'H', 'Q' },
                { 'I', 'V' },
                { 'J', 'Z' },
                { 'K', 'N' },
                { 'L', 'T' },
                { 'M', 'O' },
                { 'N', 'W' },
                { 'O', 'Y' },
                { 'P', 'H' },
                { 'Q', 'X' },
                { 'R', 'U' },
                { 'S', 'S' },
                { 'T', 'P' },
                { 'U', 'A' },
                { 'V', 'I' },
                { 'W', 'B' },
                { 'X', 'R' },
                { 'Y', 'C' },
                { 'Z', 'J' }
            },
            'R');

        public static readonly RotorProps ROTOR_PROPS_2 = new(new Dictionary<char, char>
            {
                { 'A', 'A' },
                { 'B', 'J' },
                { 'C', 'D' },
                { 'D', 'K' },
                { 'E', 'S' },
                { 'F', 'I' },
                { 'G', 'R' },
                { 'H', 'U' },
                { 'I', 'X' },
                { 'J', 'B' },
                { 'K', 'L' },
                { 'L', 'H' },
                { 'M', 'W' },
                { 'N', 'T' },
                { 'O', 'M' },
                { 'P', 'C' },
                { 'Q', 'Q' },
                { 'R', 'G' },
                { 'S', 'Z' },
                { 'T', 'N' },
                { 'U', 'P' },
                { 'V', 'Y' },
                { 'W', 'F' },
                { 'X', 'V' },
                { 'Y', 'O' },
                { 'Z', 'E' }
            },
            'F');

        public static readonly RotorProps ROTOR_PROPS_3 = new(new Dictionary<char, char>
            {
                { 'A', 'B' },
                { 'B', 'D' },
                { 'C', 'F' },
                { 'D', 'H' },
                { 'E', 'J' },
                { 'F', 'L' },
                { 'G', 'C' },
                { 'H', 'P' },
                { 'I', 'R' },
                { 'J', 'T' },
                { 'K', 'X' },
                { 'L', 'V' },
                { 'M', 'Z' },
                { 'N', 'N' },
                { 'O', 'Y' },
                { 'P', 'E' },
                { 'Q', 'I' },
                { 'R', 'W' },
                { 'S', 'G' },
                { 'T', 'A' },
                { 'U', 'K' },
                { 'V', 'M' },
                { 'W', 'U' },
                { 'X', 'S' },
                { 'Y', 'Q' },
                { 'Z', 'O' }
            },
            'W');

        public static readonly RotorProps ROTOR_PROPS_4 = new(new Dictionary<char, char>
            {
                { 'A', 'E' },
                { 'B', 'S' },
                { 'C', 'O' },
                { 'D', 'V' },
                { 'E', 'P' },
                { 'F', 'Z' },
                { 'G', 'J' },
                { 'H', 'A' },
                { 'I', 'Y' },
                { 'J', 'Q' },
                { 'K', 'U' },
                { 'L', 'I' },
                { 'M', 'R' },
                { 'N', 'H' },
                { 'O', 'X' },
                { 'P', 'L' },
                { 'Q', 'N' },
                { 'R', 'F' },
                { 'S', 'T' },
                { 'T', 'G' },
                { 'U', 'K' },
                { 'V', 'D' },
                { 'W', 'C' },
                { 'X', 'M' },
                { 'Y', 'W' },
                { 'Z', 'B' }
            },
            'K');

        public static readonly RotorProps ROTOR_PROPS_5 = new(new Dictionary<char, char>
            {
                { 'A', 'V' },
                { 'B', 'Z' },
                { 'C', 'B' },
                { 'D', 'R' },
                { 'E', 'G' },
                { 'F', 'I' },
                { 'G', 'T' },
                { 'H', 'Y' },
                { 'I', 'U' },
                { 'J', 'P' },
                { 'K', 'S' },
                { 'L', 'D' },
                { 'M', 'N' },
                { 'N', 'H' },
                { 'O', 'L' },
                { 'P', 'X' },
                { 'Q', 'A' },
                { 'R', 'W' },
                { 'S', 'M' },
                { 'T', 'J' },
                { 'U', 'Q' },
                { 'V', 'O' },
                { 'W', 'F' },
                { 'X', 'E' },
                { 'Y', 'C' },
                { 'Z', 'K' }
            },
            'A');
    }
}