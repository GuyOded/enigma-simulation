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

        private void Start()
        {
            if (!_showInstructionsOnStart)
            {
                HideInstructions();
                return;
            }

            _instructionFrame.localScale = Vector3.zero;
            _instructionFrame.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBounce);
        }

        public void HideInstructions()
        {
            _instructionsPanel.SetActive(false);
            _enigmaController.AttachKeysInput();
        }

        public void EnableRotorConfigurationMode()
        {

        }
    }
}
