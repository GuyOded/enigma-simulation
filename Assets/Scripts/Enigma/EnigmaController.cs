using System.Collections.Generic;
using DG.Tweening;
using Encryption;
using InputHandler;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Enigma
{
    public class EnigmaController : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private RotorsController _rotorsController;
        [SerializeField] private EnigmaTypeModeController _typeModeController;
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
        }

        public void OnGearConfigurationClick()
        {
            _cameraTransform.DOMove(_gearsConfigurationAlignment.position, 1f);
            _cameraTransform.DORotate(_gearsConfigurationAlignment.rotation.eulerAngles, 1f);
            _currentMode = EnigmaOperationMode.RotorConfiguration;
        }

        public void OnTypeConfigurationClick()
        {
            _cameraTransform.DOMove(_typeModeAlignment.position, 1f);
            _cameraTransform.DORotate(_typeModeAlignment.rotation.eulerAngles, 1f);
            _currentMode = EnigmaOperationMode.Type;
        }

        public void OnLetterTranspositionsClick()
        {
            _cameraTransform.DOMove(_letterTranspositionModeAlignment.position, 1f);
            _cameraTransform.DORotate(_letterTranspositionModeAlignment.rotation.eulerAngles, 1f);
            _currentMode = EnigmaOperationMode.LettersTranspositions;
        }

        public void AttachKeysInput()
        {
            _inputController.Attach(OnKeyDown, OnKeyUp);
        }

        private void OnKeyDown(string key)
        {
            if (_currentMode != EnigmaOperationMode.Type)
                return;

            if (!StringUtils.IsLetter(key))
                return;

            char encrypted = _enigmaEncryptor.EncipherChar(key.ToUpper()[0]);
            _typeModeController.OnKeyDown(key, encrypted.ToString());
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
