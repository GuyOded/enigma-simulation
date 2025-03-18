using UnityEngine;

namespace Enigma
{
    public class LetterPlug : MonoBehaviour
    {
        [SerializeField] private Outline _outline;

        public Outline Outline
        {
            get;
            private set;
        }

        private void Start()
        {
            Outline = _outline;
        }
    }
}
