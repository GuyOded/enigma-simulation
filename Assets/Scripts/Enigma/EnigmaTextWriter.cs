using InputHandler;
using TMPro;
using UnityEngine;
using Utils;

namespace Enigma
{
    public class EnigmaTextWriter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _plainText;
        [SerializeField] private TMP_Text _cipherText;
        [SerializeField] private InputController _inputController;
        [SerializeField] private ClipboardController _clipboardController;

        private void Start()
        {
            _plainText.text = "";
            _cipherText.text = "";
        }

        public void DetachInputEvent()
        {
            _inputController.DetachPressEvent(WritePlainText);
        }

        public void AttachInputEvent()
        {
            _inputController.AttachPressEvent(WritePlainText);
        }

        public void WriteCipherText(char c)
        {
            _cipherText.text += char.ToUpper(c);
        }

        public void ClearTextBoxes()
        {
            _plainText.text = "";
            _cipherText.text = "";
        }

        public void CopyTextsToClipboard()
        {
            string textToCopy = $"Plain: {_plainText.text}\nCipher: {_cipherText.text}\n";
            _clipboardController.CopyText(textToCopy);
        }

        private void WritePlainText(string text)
        {
            bool shiftIsPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            foreach (char c in text)
            {
                string currentText = _plainText.text;
                if (c == '\b')
                {
                    if (string.IsNullOrEmpty(currentText))
                        continue;

                    _plainText.text = currentText[..^1];
                    continue;
                }

                char currentChar = c;
                currentChar = currentChar == '\r' ? '\n' : currentChar;
                if (StringUtils.IsLetter(c.ToString()))
                {
                    currentChar = shiftIsPressed ? char.ToUpper(c) : char.ToLower(c);
                }

                _plainText.text += currentChar;
            }
        }
    }
}
