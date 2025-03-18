using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Enigma
{
    public class PlugboardController : MonoBehaviour
    {
        [SerializedDictionary("Letter", "Game Object")] [SerializeField]
        private SerializedDictionary<char, LetterPlug> _letterPlugsMap = new()
        {
            { 'A', null },
            { 'B', null },
            { 'C', null },
            { 'D', null },
            { 'E', null },
            { 'F', null },
            { 'G', null },
            { 'H', null },
            { 'I', null },
            { 'J', null },
            { 'K', null },
            { 'L', null },
            { 'M', null },
            { 'N', null },
            { 'O', null },
            { 'P', null },
            { 'Q', null },
            { 'R', null },
            { 'S', null },
            { 'T', null },
            { 'U', null },
            { 'V', null },
            { 'W', null },
            { 'X', null },
            { 'Y', null },
            { 'Z', null }
        };

        private bool _isClickEventsActive;

        private void Start()
        {
            _isClickEventsActive = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _isClickEventsActive)
            {
                CastRayInMouseDirection();
            }
        }

        public void AttachClickEvents()
        {
            _isClickEventsActive = true;
        }

        public void DetachClickEvents()
        {
            _isClickEventsActive = false;
        }

        private void CastRayInMouseDirection()
        {
            Debug.Log("This is active!");
        }
    }
}
