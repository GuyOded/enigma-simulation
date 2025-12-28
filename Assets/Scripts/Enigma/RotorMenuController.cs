using System;
using System.Collections.Generic;
using System.Linq;
using Encryption;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace Enigma
{
    public class RotorMenuController : MonoBehaviour
    {
        [SerializeField] private EnigmaController _enigmaController;

        [SerializeField] private TMP_InputField _leftRotorMenuText;
        [SerializeField] private TMP_InputField _middleRotorMenuText;
        [SerializeField] private TMP_InputField _rightRotorMenuText;

        public void SetLettersText(List<int> rotorPositions)
        {
            List<string> currentLetters = rotorPositions.Select(letterIndex => ((char)('A' + letterIndex)).ToString()).ToList();
            _leftRotorMenuText.text = currentLetters[0];
            _middleRotorMenuText.text = currentLetters[1];
            _rightRotorMenuText.text = currentLetters[^1];
        }

        public void OnLeftRotorTextEndEdit(string letter)
        {
            RotateRotorToLetter(RotorsPlacement.Left, letter);
        }

        public void OnMiddleRotorTextEndEdit(string letter)
        {
            RotateRotorToLetter(RotorsPlacement.Middle, letter);
        }

        public void OnRightRotorTextEndEdit(string letter)
        {
            RotateRotorToLetter(RotorsPlacement.Right, letter);
        }

        public void OnLeftRotorTypeChange(int index)
        {
            _enigmaController.SetEnigmaRotorType(RotorsPlacement.Left, GetRotorPropsByIndex(index));
        }

        public void OnMiddleRotorTypeChange(int index)
        {
            _enigmaController.SetEnigmaRotorType(RotorsPlacement.Middle, GetRotorPropsByIndex(index));
        }

        public void OnRightRotorTypeChange(int index)
        {
            _enigmaController.SetEnigmaRotorType(RotorsPlacement.Right, GetRotorPropsByIndex(index));
        }

        public void OnReflectorTypeChange(int index)
        {
            _enigmaController.SetEnigmaReflectorType(GetReflectorByIndex(index));
        }

        private RotorProps GetRotorPropsByIndex(int index)
        {
            return index switch
            {
                0 => Rotors.ROTOR_PROPS_1,
                1 => Rotors.ROTOR_PROPS_2,
                2 => Rotors.ROTOR_PROPS_3,
                3 => Rotors.ROTOR_PROPS_4,
                4 => Rotors.ROTOR_PROPS_5,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "No rotor types available for this index")
            };
        }

        private IDictionary<char, char> GetReflectorByIndex(int index)
        {
            return index switch
            {
                0 => Reflectors.REFLECTOR_A,
                1 => Reflectors.REFLECTOR_B,
                2 => Reflectors.REFLECTOR_C,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "No reflector types available for this index")
            };
        }

        private void RotateRotorToLetter(RotorsPlacement placement, string letter)
        {
            string currentRotorPosition = ((char)('A' + _enigmaController.GetRotorPosition(placement))).ToString();
            TMP_InputField rotorInput = placement switch
            {
                RotorsPlacement.Left => _leftRotorMenuText,
                RotorsPlacement.Middle => _middleRotorMenuText,
                RotorsPlacement.Right => _rightRotorMenuText,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (!StringUtils.IsLetter(letter))
            {
                rotorInput.text = currentRotorPosition;
                return;
            }

            int forwardSteps = letter[0] - currentRotorPosition[0];
            int backwardSteps = letter[0] - currentRotorPosition[0] - Encryption.Consts.ALPHABET_SIZE;
            int steps = math.abs(forwardSteps) <= math.abs(backwardSteps) ? forwardSteps : backwardSteps;
            _enigmaController.RotateRotor(placement, steps);
        }

        public void OnLeftRotorUp()
        {
            _enigmaController.StepwiseRotateRotor(RotorsPlacement.Left, 1);
            _leftRotorMenuText.text = ((char)('A' + _enigmaController.GetRotorPositions()[0])).ToString();
        }

        public void OnLeftRotorDown()
        {
            _enigmaController.StepwiseRotateRotor(RotorsPlacement.Left, -1);
            _leftRotorMenuText.text = ((char)('A' + _enigmaController.GetRotorPositions()[0])).ToString();
        }

        public void OnRightRotorUp()
        {
            _enigmaController.StepwiseRotateRotor(RotorsPlacement.Right, 1);
            _rightRotorMenuText.text = ((char)('A' + _enigmaController.GetRotorPositions()[^1])).ToString();
        }

        public void OnRightRotorDown()
        {
            _enigmaController.StepwiseRotateRotor(RotorsPlacement.Right, -1);
            _rightRotorMenuText.text = ((char)('A' + _enigmaController.GetRotorPositions()[^1])).ToString();
        }

        public void OnMiddleRotorUp()
        {
            _enigmaController.StepwiseRotateRotor(RotorsPlacement.Middle, 1);
            _middleRotorMenuText.text = ((char)('A' + _enigmaController.GetRotorPositions()[1])).ToString();
        }

        public void OnMiddleRotorDown()
        {
            _enigmaController.StepwiseRotateRotor(RotorsPlacement.Middle, -1);
            _middleRotorMenuText.text = ((char)('A' + _enigmaController.GetRotorPositions()[1])).ToString();
        }
    }
}
