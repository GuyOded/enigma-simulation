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
        [SerializeField] private RectTransform _textFrameExpandArrow;
        [SerializeField] private RectTransform _textFrameContainer;
        [SerializeField] private RectTransform _textFrame;

        // Gear config related
        [SerializeField] private GameObject _rotorConfigCanvas;
        [SerializeField] private Transform[] _rotorsConfigButtons;

        private bool _isTextsFrameShown = true;

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

        public void ToggleTextsFrame(bool value)
        {
            _isTextsFrameShown = value;

            if (DOTween.IsTweening(_textFrameContainer))
                _textFrameContainer.DOKill();
            if (DOTween.IsTweening(_textFrameExpandArrow))
                _textFrameExpandArrow.DOKill();

            float finalXPosition = value ? 0 : _textFrame.rect.width;
            _textFrameContainer.DOAnchorPosX(finalXPosition, 1f).SetEase(Ease.OutQuart);

            float finalArrowZRotation = value ? 0 : 180;
            _textFrameExpandArrow.DORotate(Vector3.forward * finalArrowZRotation, 1f).SetEase(Ease.OutQuart);
        }

        public void HandleModeSwitch(EnigmaOperationMode enigmaMode)
        {
            if (enigmaMode != EnigmaOperationMode.Type)
            {
                HideTextsContainer();
            }
            else
            {
                ShowTextsContainer();
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

        private void HideTextsContainer()
        {
            if (DOTween.IsTweening(_textFrameContainer))
                _textFrameContainer.DOKill();

            _textFrameContainer.DOAnchorPosX(_textFrameContainer.rect.width, 1f).SetEase(Ease.OutQuart);
        }

        private void ShowTextsContainer()
        {
            if (DOTween.IsTweening(_textFrameContainer))
                _textFrameContainer.DOKill();

            float finalXPosition = _isTextsFrameShown ? 0 : _textFrame.rect.width;
            _textFrameContainer.DOAnchorPosX(finalXPosition, 1f).SetEase(Ease.OutQuart);
        }
    }
}
