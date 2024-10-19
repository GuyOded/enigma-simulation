using System;
using System.Collections.Generic;
using System.Linq;
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

        public void RotateRotor(RotorsPlacement rotor, int stepsToRotate)
        {
            List<RotorConfiguration> rotorConfiguration = _enigmaEncryptor.GetInitialConfiguration();
            int rotorIndex = rotor switch
            {
                RotorsPlacement.Left => 0,
                RotorsPlacement.Middle => 1,
                RotorsPlacement.Right => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(rotor), rotor, null)
            };

            List<int> rotorsPositions = _enigmaEncryptor.GetCurrentRotorPositions();
            ICollection<RotorConfiguration> newConfig = rotorConfiguration.Zip(rotorsPositions, (configuration, position) => (configuration, position))
                .Select((configPositionTuple, index) => {
                    RotorConfiguration config = configPositionTuple.configuration;
                    int newPosition = (configPositionTuple.position + (rotorIndex == index ? stepsToRotate : 0)) % Consts.ALPHABET_SIZE;
                    newPosition += newPosition < 0 ? Consts.ALPHABET_SIZE : 0;

                    return new RotorConfiguration(config.RotorProps,
                        (char)(Consts.FIRST_LETTER + newPosition),
                        config.StepCallback,
                        config.RingSetting);
                }).ToArray();

            _enigmaEncryptor = new EnigmaEncryptor(_enigmaEncryptor.GetLetterTranspositions(), newConfig, _enigmaEncryptor.GetReflector());
            _rotorsController.RotateRotor(rotor, stepsToRotate);
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

            if (DOTween.IsTweening(_cameraTransform))
                _cameraTransform.DOKill();

            _cameraTransform.DOMove(alignmentTransform.position, 1f).OnComplete(() => _menuController.ModeSwitchAnimationCallback(mode));
            _cameraTransform.DORotate(alignmentTransform.rotation.eulerAngles, 1f);

            if (mode != EnigmaOperationMode.Type)
            {
                _textWriter.DetachInputEvent();
            }
            else if (_currentMode != EnigmaOperationMode.Type)
            {
                _textWriter.AttachInputEvent();
            }

            _menuController.HandleModeSwitch(mode);
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
            // Debug.Log(string.Join(",", _enigmaEncryptor.GetCurrentRotorPositions().ToArray()));
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
