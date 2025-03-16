using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

        public void OnLeftRotorUp()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Left, 1);
            _leftRotorMenuText.text = ((char)('A' + _enigmaController.GetCurrentRotorPositions()[0])).ToString();
        }

        public void OnLeftRotorDown()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Left, -1);
            _leftRotorMenuText.text = ((char)('A' + _enigmaController.GetCurrentRotorPositions()[0])).ToString();
        }

        public void OnRightRotorUp()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Right, 1);
            _rightRotorMenuText.text = ((char)('A' + _enigmaController.GetCurrentRotorPositions()[^1])).ToString();
        }

        public void OnRightRotorDown()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Right, -1);
            _rightRotorMenuText.text = ((char)('A' + _enigmaController.GetCurrentRotorPositions()[^1])).ToString();
        }

        public void OnMiddleRotorUp()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Middle, 1);
            _middleRotorMenuText.text = ((char)('A' + _enigmaController.GetCurrentRotorPositions()[1])).ToString();
        }

        public void OnMiddleRotorDown()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Middle, -1);
            _middleRotorMenuText.text = ((char)('A' + _enigmaController.GetCurrentRotorPositions()[1])).ToString();
        }
    }
}
