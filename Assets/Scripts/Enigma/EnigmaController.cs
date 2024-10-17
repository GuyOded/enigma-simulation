using System;
using System.Collections.Generic;
using DG.Tweening;
using Encryption;
using InputHandler;
using UnityEngine;
using Utils;

namespace Enigma
{
    public class EnigmaController : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private RotorsController _rotorsController;
        [SerializeField] private EnigmaTypeModeController _typeModeController;
        [SerializeField] private MenuController _menuController;
        [SerializeField] private EnigmaTextWriter _textWriter;
        [SerializeField] private Transform _cameraTransform;

        [SerializeField] private Transform _gearsConfigurationAlignment;
        [SerializeField] private Transform _typeModeAlignment;
        [SerializeField] private Transform _letterTranspositionModeAlignment;

        private EnigmaEncryptor _enigmaEncryptor;
        private EnigmaOperationMode _currentMode;

        private void Start()
        {
            _enigmaEncryptor = new EnigmaEncryptor(new Dictionary<char, char>(), _rotorsController.GetDefaultRotorsConfig(), Reflectors.REFLECTOR_A);
            _currentMode = EnigmaOperationMode.Type;
            _textWriter.AttachInputEvent();
        }

        public void OnGearConfigurationClick()
        {
            HandleModeSwitch(EnigmaOperationMode.RotorConfiguration);
        }

        public void OnTypeConfigurationClick()
        {
            HandleModeSwitch(EnigmaOperationMode.Type);
        }

        public void OnLetterTranspositionsClick()
        {
            HandleModeSwitch(EnigmaOperationMode.LettersTranspositions);
        }

        public void AttachKeysInput()
        {
            _inputController.AttachTypeEvent(OnKeyDown, OnKeyUp);
        }

        private void HandleModeSwitch(EnigmaOperationMode mode)
        {
            Transform alignmentTransform = mode switch
            {
                EnigmaOperationMode.Type => _typeModeAlignment,
                EnigmaOperationMode.LettersTranspositions => _letterTranspositionModeAlignment,
                EnigmaOperationMode.RotorConfiguration => _gearsConfigurationAlignment,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };

            _cameraTransform.DOMove(alignmentTransform.position, 1f);
            _cameraTransform.DORotate(alignmentTransform.rotation.eulerAngles, 1f);

            if (mode != EnigmaOperationMode.Type)
            {
                _textWriter.DetachInputEvent();
                _menuController.HideTextsContainer();
            }
            else if (_currentMode != EnigmaOperationMode.Type)
            {
                _menuController.ShowTextsContainer();
                _textWriter.AttachInputEvent();
            }

            _currentMode = mode;
        }

        private void OnKeyDown(string key)
        {
            if (_currentMode != EnigmaOperationMode.Type)
                return;

            if (!StringUtils.IsLetter(key))
                return;

            char encrypted = _enigmaEncryptor.EncipherChar(key.ToUpper()[0]);
            _typeModeController.OnKeyDown(key, encrypted.ToString());
            _textWriter.WriteCipherText(encrypted);
        }

        private void OnKeyUp(string key)
        {
            if (_currentMode != EnigmaOperationMode.Type)
                return;

            if (StringUtils.IsLetter(key))
            {
                _typeModeController.OnKeyUp(key);
            }
        }
    }
}
