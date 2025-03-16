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
            int backwardSteps = letter[0] - currentRotorPosition[0] - Consts.ALPHABET_SIZE;
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
