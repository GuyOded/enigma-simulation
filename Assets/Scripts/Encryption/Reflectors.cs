﻿using System.Collections.Generic;

namespace Encryption
{
    public static class Reflectors
    {
        public static readonly Dictionary<char, char> REFLECTOR_A = new Dictionary<char, char>
        {
            {'A', 'E'},
            {'B', 'J'},
            {'C', 'M'},
            {'D', 'Z'},
            {'E', 'A'},
            {'F', 'L'},
            {'G', 'Y'},
            {'H', 'X'},
            {'I', 'V'},
            {'J', 'B'},
            {'K', 'W'},
            {'L', 'F'},
            {'M', 'C'},
            {'N', 'R'},
            {'O', 'Q'},
            {'P', 'U'},
            {'Q', 'O'},
            {'R', 'N'},
            {'S', 'T'},
            {'T', 'S'},
            {'U', 'P'},
            {'V', 'I'},
            {'W', 'K'},
            {'X', 'H'},
            {'Y', 'G'},
            {'Z', 'D'}
        };

        public static readonly Dictionary<char, char> REFLECTOR_B = new Dictionary<char, char>
        {
            { 'A', 'Y' },
            { 'B', 'R' },
            { 'C', 'U' },
            { 'D', 'H' },
            { 'E', 'Q' },
            { 'F', 'S' },
            { 'G', 'L' },
            { 'H', 'D' },
            { 'I', 'P' },
            { 'J', 'X' },
            { 'K', 'N' },
            { 'L', 'G' },
            { 'M', 'O' },
            { 'N', 'K' },
            { 'O', 'M' },
            { 'P', 'I' },
            { 'Q', 'E' },
            { 'R', 'B' },
            { 'S', 'F' },
            { 'T', 'Z' },
            { 'U', 'C' },
            { 'V', 'W' },
            { 'W', 'V' },
            { 'X', 'J' },
            { 'Y', 'A' },
            { 'Z', 'T' }
        };

        public static readonly Dictionary<char, char> REFLECTOR_C = new Dictionary<char, char>
        {
            { 'A', 'F' },
            { 'B', 'V' },
            { 'C', 'P' },
            { 'D', 'J' },
            { 'E', 'I' },
            { 'F', 'A' },
            { 'G', 'O' },
            { 'H', 'Y' },
            { 'I', 'E' },
            { 'J', 'D' },
            { 'K', 'R' },
            { 'L', 'Z' },
            { 'M', 'X' },
            { 'N', 'W' },
            { 'O', 'G' },
            { 'P', 'C' },
            { 'Q', 'T' },
            { 'R', 'K' },
            { 'S', 'U' },
            { 'T', 'Q' },
            { 'U', 'S' },
            { 'V', 'B' },
            { 'W', 'N' },
            { 'X', 'M' },
            { 'Y', 'H' },
            { 'Z', 'L' }
        };

        public static readonly Dictionary<char, char> REFLECTOR_BETA = new Dictionary<char, char>
        {
            { 'A', 'L' },
            { 'B', 'E' },
            { 'C', 'Y' },
            { 'D', 'J' },
            { 'E', 'V' },
            { 'F', 'C' },
            { 'G', 'N' },
            { 'H', 'I' },
            { 'I', 'X' },
            { 'J', 'W' },
            { 'K', 'P' },
            { 'L', 'B' },
            { 'M', 'Q' },
            { 'N', 'M' },
            { 'O', 'D' },
            { 'P', 'R' },
            { 'Q', 'T' },
            { 'R', 'A' },
            { 'S', 'K' },
            { 'T', 'Z' },
            { 'U', 'G' },
            { 'V', 'F' },
            { 'W', 'U' },
            { 'X', 'H' },
            { 'Y', 'O' },
            { 'Z', 'S' }
        };

        public static readonly Dictionary<char, char> REFLECTOR_GAMMA = new Dictionary<char, char>
        {
            { 'A', 'F' },
            { 'B', 'S' },
            { 'C', 'O' },
            { 'D', 'K' },
            { 'E', 'A' },
            { 'F', 'N' },
            { 'G', 'U' },
            { 'H', 'E' },
            { 'I', 'R' },
            { 'J', 'H' },
            { 'K', 'M' },
            { 'L', 'B' },
            { 'M', 'T' },
            { 'N', 'I' },
            { 'O', 'Y' },
            { 'P', 'C' },
            { 'Q', 'W' },
            { 'R', 'L' },
            { 'S', 'Q' },
            { 'T', 'P' },
            { 'U', 'Z' },
            { 'V', 'X' },
            { 'W', 'V' },
            { 'X', 'G' },
            { 'Y', 'J' },
            { 'Z', 'D' }
        };
    }
}
