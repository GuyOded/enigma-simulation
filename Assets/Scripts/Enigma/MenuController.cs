using System;
using DG.Tweening;
using UnityEngine;

namespace Enigma
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _instructionsPanel;
        [SerializeField] private RectTransform _instructionFrame;
        [SerializeField] private EnigmaController _enigmaController;
        [SerializeField] private bool _showInstructionsOnStart = true;

        // Text frame related
        [SerializeField] private ExpandableSideMenu _textMenu;

        // Gear config related
        [SerializeField] private ExpandableSideMenu _rotorConfigMenu;
        [SerializeField] private GameObject _rotorConfigCanvas;
        [SerializeField] private Transform[] _rotorsConfigButtons;

        private void Start()
        {
            if (!_showInstructionsOnStart)
            {
                HideInstructions();
                return;
            }

            _instructionFrame.localScale = Vector3.zero;
            _instructionFrame.DOScale(Vector3.one, 0.4f).SetEase(Ease.InOutElastic);
        }

        public void HideInstructions()
        {
            _instructionsPanel.SetActive(false);
            _enigmaController.AttachKeysInput();
        }

        public void HandleModeSwitch(EnigmaOperationMode enigmaMode)
        {
            _textMenu.Hide(() => _textMenu.gameObject.SetActive(false));
            _rotorConfigMenu.Hide(() => _rotorConfigMenu.gameObject.SetActive(false));

            switch (enigmaMode)
            {
                case EnigmaOperationMode.Type:
                    _textMenu.gameObject.SetActive(true);
                    _textMenu.Show();
                    break;
                case EnigmaOperationMode.RotorConfiguration:
                    _rotorConfigMenu.gameObject.SetActive(true);
                    _rotorConfigMenu.Show();
                    break;
                case EnigmaOperationMode.LettersTranspositions:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enigmaMode), enigmaMode, null);
            }

            if (enigmaMode != EnigmaOperationMode.RotorConfiguration)
            {
                _rotorConfigCanvas.SetActive(false);
            }
        }

        public void ModeSwitchAnimationCallback(EnigmaOperationMode enigmaMode)
        {
            if (enigmaMode == EnigmaOperationMode.RotorConfiguration)
            {
                _rotorConfigCanvas.SetActive(true);
                foreach (Transform rotorsConfigButton in _rotorsConfigButtons)
                {
                    rotorsConfigButton.localScale = Vector3.zero;
                    rotorsConfigButton.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutElastic);
                }
            }
        }
    }
}
