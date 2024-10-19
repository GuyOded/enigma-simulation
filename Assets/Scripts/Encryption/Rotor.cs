using System;
using System.Collections.Generic;

namespace Encryption
{
    public class Rotor
    {
        public delegate void StepMechanismCallback(char newPosition);

        private readonly RotorProps _rotorProps;
        private readonly int _ringSetting;
        private int _currentPosition;
        private readonly StepMechanismCallback _stepCallback;

        public Rotor(RotorProps rotorProps, char initialPosition, StepMechanismCallback stepCallback = null, char ringSetting = 'A')
        {
            ValidateInputInRange(initialPosition, "Position");
            ValidateInputInRange(ringSetting, "Ring Setting");

            _rotorProps = rotorProps;
            _currentPosition = initialPosition - Consts.FIRST_LETTER;
            _stepCallback = stepCallback;
            _ringSetting = MathUtils.MathMod(ringSetting - Consts.FIRST_LETTER, Consts.ALPHABET_SIZE);
        }

        public void Increment()
        {
            _currentPosition = (_currentPosition + 1) % Consts.ALPHABET_SIZE;
            if (_currentPosition == (_rotorProps.StepPosition - Consts.FIRST_LETTER))
            {
                _stepCallback?.Invoke((char)(Consts.FIRST_LETTER + _currentPosition));
            }
        }

        public int GetCurrentPosition()
        {
            return _currentPosition;
        }

        public char IncrementAndGet(char input)
        {
            Increment();

            return GetMappedCharacter(input);
        }

        public char GetInverseCharacter(char input)
        {
            return GetMappedCharacter(input, _rotorProps.Inverse, _currentPosition - _ringSetting, _ringSetting - _currentPosition);
        }

        public char GetMappedCharacter(char input)
        {
            return GetMappedCharacter(input, _rotorProps.Permutation, _currentPosition - _ringSetting, _ringSetting -_currentPosition);
        }

        private static char GetMappedCharacter(char input, Dictionary<char, char> permutation, int firstCycle,
            int secondCycle)
        {
            ValidateInputInRange(input, "Position");

            char firstCycleResult = MathUtils.AddNumToLetterModAlphabet(input, firstCycle);
            permutation.TryGetValue(firstCycleResult, out char permutationResult);
            char secondCycleResult = MathUtils.AddNumToLetterModAlphabet(permutationResult, secondCycle);

            return secondCycleResult;
        }

        private static void ValidateInputInRange(char input, string subjectName)
        {
            if (!Validations.IsCharInRange(input))
            {
                throw new ArgumentOutOfRangeException($"{subjectName} must be between '{Consts.FIRST_LETTER}' and '{Consts.LAST_LETTER}'.");
            }
        }
    }
}
