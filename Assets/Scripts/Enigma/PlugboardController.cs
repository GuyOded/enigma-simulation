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
        [SerializeField] private Camera _mainCamera;

        private const float RAYCAST_LENGTH = 100f;

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
            Ray outgoingRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(outgoingRay, out RaycastHit hit, RAYCAST_LENGTH, LayerMask.GetMask("LetterPlugs")))
            {
                foreach (LetterPlug letterPlug in _letterPlugsMap.Values)
                    letterPlug.Outline.enabled = false;

                return;
            }

            char letter = hit.collider.tag[0];
            if (!_letterPlugsMap.TryGetValue(letter, out LetterPlug plug))
            {
                Debug.LogError($"Raycast hit but found no object in letter to object map. Tag hit is {hit.collider.tag}");
                return;
            }

            plug.Outline.enabled = true;
        }
    }
}
