using System.Collections.Generic;
using System.Linq;

namespace Encryption
{
    public static class MathUtils
    {
        public static char AddNumToLetterModAlphabet(char letter, int number)
        {
            int firstIndex = letter - Consts.FIRST_LETTER;
            firstIndex += number;
            firstIndex = MathMod(firstIndex, Consts.ALPHABET_SIZE);
            return (char)(firstIndex + Consts.FIRST_LETTER);
        }

        public static int MathMod(int number, int mod)
        {
            return number < 0 ? (number % mod) + mod : number % mod;
        }
    }

    public static class Validations
    {
        public static bool IsCharInRange(char letter)
        {
            return letter >= Consts.FIRST_LETTER && letter <= Consts.LAST_LETTER;
        }

        public static bool IsInjective<TKey, TValue>(IDictionary<TKey, TValue> map) where TKey : notnull
        {
            return !(map.Values.Distinct().Count() < map.Count);
        }

        public static bool IsInvolution<TKey>(IDictionary<TKey, TKey> map) where TKey : notnull
        {
            if (!IsInjective(map))
                return false;

            return map.Keys.All(key =>
            {
                map.TryGetValue(key, out TKey mapOfKey);
                if (mapOfKey == null)
                    return false;

                // If value is a key and is mapped to anything other than "key" map contains a cycle of order at least three which means it is not an involution.
                return !map.ContainsKey(mapOfKey) || map.TryGetValue(mapOfKey, out TKey mapOfValue) && key.Equals(mapOfValue);
            });
        }
    }
}