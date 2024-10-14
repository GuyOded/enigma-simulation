using System.Collections.Generic;
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

        private EnigmaEncryptor _enigmaEncryptor;
        private string _lastKeyPressed;

        private void Start()
        {
            _enigmaEncryptor = new EnigmaEncryptor(new Dictionary<char, char>(), _rotorsController.GetDefaultRotorsConfig(), Reflectors.REFLECTOR_A);
        }

        public void AttachKeysInput()
        {
            _inputController.Attach(OnKeyDown, OnKeyUp);
        }

        public void DetachKeysInput()
        {
            _inputController.Detach(OnKeyDown, OnKeyUp);
        }

        private void OnKeyDown(string key)
        {
            if (StringUtils.IsLetter(key))
            {
                char encrypted = _enigmaEncryptor.EncipherChar(key.ToUpper()[0]);
                _typeModeController.OnKeyDown(key, encrypted.ToString());
            }
        }

        private void OnKeyUp(string key)
        {
            if (StringUtils.IsLetter(key))
            {
                _typeModeController.OnKeyUp(key);
            }
        }
    }
}
