using System;
using UnityEngine;

namespace InputHandler
{
    public class InputController : MonoBehaviour
    {
        private event Action<string> KeyDown;
        private event Action<string> KeyUp;

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
        }

        private void TypeActionOnCanceled(string keyName)
        {
            KeyUp?.Invoke(keyName);
        }

        private void TypeActionOnStarted(string keyName)
        {
            KeyDown?.Invoke(keyName);
        }

        public void Attach(Action<string> onDown, Action<string> onUp)
        {
            KeyDown += onDown;
            KeyUp += onUp;
        }

        public void Detach(Action<string> onDown, Action<string> onUp)
        {
            KeyDown -= onDown;
            KeyUp -= onUp;
        }
    }
}
