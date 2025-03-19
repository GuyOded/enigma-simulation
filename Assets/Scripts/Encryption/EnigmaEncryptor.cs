using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encryption
{
    public class EnigmaEncryptor : IEncryptor
    {
        private readonly RotorChain _rc;
        private readonly IDictionary<char, char> _reflector;
        private readonly IDictionary<char, char> _letterTranspositions;
        private readonly ICollection<RotorConfiguration> _initialRotorConfig;

        public EnigmaEncryptor(IDictionary<char, char> letterTranspositions,
            ICollection<RotorConfiguration> rotorConfigurations,
            IDictionary<char, char> reflector)
        {
            if (!letterTranspositions.All(pair => Validations.IsCharInRange(pair.Key) && Validations.IsCharInRange(pair.Value)))
                throw new ArgumentOutOfRangeException(
                    $"All letter transpositions must map letters in the range of {Consts.FIRST_LETTER} and {Consts.LAST_LETTER}");

            if (!Validations.IsInjective(letterTranspositions))
                throw new ArgumentException("The letter transpositions given is not injective");

            if (!Validations.IsInjective(reflector))
                throw new ArgumentException("Reflector given is not an injective map");

            if (!Validations.IsInvolution(letterTranspositions))
                throw new ArgumentException("The letter transposition map is not an involution");

            _letterTranspositions = SymmetrizeMap(letterTranspositions);
            _initialRotorConfig = rotorConfigurations.ToList();
            _rc = new RotorChain(rotorConfigurations);
            _reflector = reflector;
        }

        public string Encrypt(string payload)
        {
            StringBuilder sb = new();
            sb.AppendJoin("", payload.AsEnumerable().Select(EncipherChar));
            return sb.ToString();
        }

        public string Decrypt(string payload)
        {
            return Encrypt(payload);
        }

        public List<RotorConfiguration> GetInitialConfiguration()
        {
            return _initialRotorConfig.ToList();
        }

        public List<int> GetCurrentRotorPositions()
        {
            return _rc.GetCurrentRotorPositions();
        }

        public IDictionary<char, char> GetLetterTranspositions()
        {
            return _letterTranspositions;
        }

        public IDictionary<char, char> GetReflector()
        {
            return _reflector;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public char EncipherChar(char letter)
        {

            char enciphered = letter;
            if (_letterTranspositions.ContainsKey(letter))
                _letterTranspositions.TryGetValue(letter, out enciphered);

            enciphered = _rc.IncrementAndGetMappedCharacter(enciphered);
            if (_reflector.ContainsKey(enciphered))
                _reflector.TryGetValue(enciphered, out enciphered);

            enciphered = _rc.GetInverseCharacter(enciphered);
            if (!_letterTranspositions.ContainsKey(enciphered))
                return enciphered;

            _letterTranspositions.TryGetValue(enciphered, out enciphered);
            return enciphered;
        }

        private static IDictionary<char, char> SymmetrizeMap(IDictionary<char, char> map)
        {
            Dictionary<char, char> result = new();
            result = map.Aggregate(result, (symmetrized, keyValuePair) =>
            {
                symmetrized.TryAdd(keyValuePair.Key, keyValuePair.Value);
                symmetrized.TryAdd(keyValuePair.Value, keyValuePair.Key);
                return symmetrized;
            });

            return result;
        }
    }
}
