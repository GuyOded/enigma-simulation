using System;
using UnityEngine;

namespace InputHandler
{
    public class InputController : MonoBehaviour
    {
        private event Action<string> KeyDown;
        private event Action<string> KeyUp;
        private event Action<string> KeyPress;

        private const string KEYS = "qwertyuiopasdfghjklzxcvbnm";

        private void Update()
        {
            foreach (char key in KEYS)
            {
                if (Input.GetKeyDown(key.ToString()))
                {
                    TypeActionOnStarted(key.ToString());
                }

                if (Input.GetKeyUp(key.ToString()))
                {
                    TypeActionOnCanceled(key.ToString());
                }
            }

            string input = Input.inputString;
            if (Input.anyKeyDown && !string.IsNullOrEmpty(input))
            {
                KeyPress?.Invoke(input);
            }
        }

        public void AttachTypeEvent(Action<string> onDown, Action<string> onUp)
        {
            KeyDown += onDown;
            KeyUp += onUp;
        }

        public void DetachTypeEvent(Action<string> onDown, Action<string> onUp)
        {
            KeyDown -= onDown;
            KeyUp -= onUp;
        }

        public void AttachPressEvent(Action<string> onKeyPress)
        {
            KeyPress += onKeyPress;
        }

        public void DetachPressEvent(Action<string> onKeyPress)
        {
            KeyPress -= onKeyPress;
        }

        private void TypeActionOnCanceled(string keyName)
        {
            KeyUp?.Invoke(keyName);
        }

        private void TypeActionOnStarted(string keyName)
        {
            KeyDown?.Invoke(keyName);
        }
    }
}
