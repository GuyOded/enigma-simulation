using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Encryption;
using Enigma.Plugboard;
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

        [SerializeField] private PlugboardController _plugboardController;

        private EnigmaEncryptor _enigmaEncryptor;
        private EnigmaOperationMode _currentMode;

        private void Start()
        {
            _enigmaEncryptor = new EnigmaEncryptor(new Dictionary<char, char>(),
                _rotorsController.GetDefaultRotorsConfig(), Reflectors.REFLECTOR_B);
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

        public void StepwiseRotateRotor(RotorsPlacement rotor, int stepsToRotate, bool animate = true)
        {
            SetEnigmaRotorPositions(rotor, stepsToRotate);
            _rotorsController.StepwiseRotateRotor(rotor, stepsToRotate, animate);
        }

        public void RotateRotor(RotorsPlacement rotor, int stepsToRotate)
        {
            SetEnigmaRotorPositions(rotor, stepsToRotate);
            _rotorsController.RotateRotor(rotor, stepsToRotate);
        }

        public List<int> GetRotorPositions()
        {
            return _enigmaEncryptor.GetCurrentRotorPositions();
        }

        public IDictionary<char, char> GetLetterTranspositions()
        {
            return _enigmaEncryptor.GetLetterTranspositions();
        }

        public (char, char)? GetTranspositionByLetter(char letter)
        {
            IDictionary<char, char> transpositions = _enigmaEncryptor.GetLetterTranspositions();
            if (transpositions.ContainsKey(letter))
            {
                return new(letter, transpositions[letter]);
            }

            char? key = transpositions.Where(kvp => kvp.Value == letter).Select(kvp => (char?)kvp.Key).FirstOrDefault();
            if (key.HasValue)
            {
                return new(key.Value, letter);
            }

            return null;
        }

        public void AddNewTransposition(char first, char second)
        {
            IDictionary<char, char> currentTranspositions = _enigmaEncryptor.GetLetterTranspositions();
            if (currentTranspositions.ContainsKey(first) || currentTranspositions.ContainsKey(second) ||
                currentTranspositions.Values.Contains(first) || currentTranspositions.Values.Contains(second))
            {
                throw new ArgumentException("Letter already exists in transpositions");
            }

            Dictionary<char, char> newTranspositions = new(currentTranspositions) { { first, second } };

            _enigmaEncryptor = new EnigmaEncryptor(newTranspositions, _enigmaEncryptor.GetInitialConfiguration(),
                _enigmaEncryptor.GetReflector());
        }

        public int GetRotorPosition(RotorsPlacement placement)
        {
            int rotorIndex = placement switch
            {
                RotorsPlacement.Left => 0,
                RotorsPlacement.Middle => 1,
                RotorsPlacement.Right => 2,
                _ => throw new IndexOutOfRangeException("Unknown rotor placement")
            };

            return _enigmaEncryptor.GetCurrentRotorPositions()[rotorIndex];
        }

        private void SetEnigmaRotorPositions(RotorsPlacement rotor, int stepsToRotate)
        {
            List<RotorConfiguration> rotorConfiguration = _enigmaEncryptor.GetInitialConfiguration();
            int rotorIndex = GetRotorIndexByPlacement(rotor);

            List<int> rotorsPositions = _enigmaEncryptor.GetCurrentRotorPositions();
            ICollection<RotorConfiguration> newConfig = rotorConfiguration
                .Zip(rotorsPositions, (configuration, position) => (configuration, position))
                .Select((configPositionTuple, index) =>
                {
                    RotorConfiguration config = configPositionTuple.configuration;
                    int newPosition = (configPositionTuple.position + (rotorIndex == index ? stepsToRotate : 0)) %
                                      Encryption.Consts.ALPHABET_SIZE;
                    newPosition += newPosition < 0 ? Encryption.Consts.ALPHABET_SIZE : 0;

                    return new RotorConfiguration(config.RotorProps,
                        (char)(Encryption.Consts.FIRST_LETTER + newPosition),
                        config.StepCallback,
                        config.RingSetting);
                }).ToArray();

            _enigmaEncryptor = new EnigmaEncryptor(_enigmaEncryptor.GetLetterTranspositions(), newConfig,
                _enigmaEncryptor.GetReflector());
        }

        public void SetEnigmaRotorType(RotorsPlacement rotor, RotorProps rotorProps)
        {
            int rotorIndex = GetRotorIndexByPlacement(rotor);

            List<RotorConfiguration> currentEncryptorConfig = BuildCurrentEncryptorConfig();
            RotorConfiguration previousRotorConfig = currentEncryptorConfig[rotorIndex];
            currentEncryptorConfig[rotorIndex] = new RotorConfiguration(rotorProps,
                previousRotorConfig.InitialPosition,
                previousRotorConfig.StepCallback,
                previousRotorConfig.RingSetting);

            _enigmaEncryptor = new EnigmaEncryptor(_enigmaEncryptor.GetLetterTranspositions(), currentEncryptorConfig,
                _enigmaEncryptor.GetReflector());
        }

        private List<RotorConfiguration> BuildCurrentEncryptorConfig()
        {
            List<int> positions = GetRotorPositions();
            return _enigmaEncryptor.GetInitialConfiguration()
                .Zip(positions, (configuration, position) => (configuration, position)).Select(configPositionTuple =>
                {
                    RotorConfiguration config = configPositionTuple.configuration;
                    int position = configPositionTuple.position;

                    return new RotorConfiguration(config.RotorProps, (char)('A' + position), config.StepCallback,
                        config.RingSetting);
                }).ToList();
        }

        private static int GetRotorIndexByPlacement(RotorsPlacement placement)
        {
            return placement switch
            {
                RotorsPlacement.Left => 0,
                RotorsPlacement.Middle => 1,
                RotorsPlacement.Right => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, null)
            };
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

            _cameraTransform.DOMove(alignmentTransform.position, 1f)
                .OnComplete(() => _menuController.ModeSwitchAnimationCallback(mode));
            _cameraTransform.DORotate(alignmentTransform.rotation.eulerAngles, 1f);

            if (mode != EnigmaOperationMode.Type)
            {
                _textWriter.DetachInputEvent();
            }
            else if (_currentMode != EnigmaOperationMode.Type)
            {
                _textWriter.AttachInputEvent();
            }

            if (mode != EnigmaOperationMode.LettersTranspositions)
            {
                _plugboardController.DetachClickEvents();
                _plugboardController.HideDeleteConnectionMenu();
            }
            else
            {
                _plugboardController.AttachClickEvents();
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
