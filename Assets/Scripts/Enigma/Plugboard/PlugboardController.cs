using AYellowpaper.SerializedCollections;
using Encryption;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma.Plugboard
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
        [SerializeField] private EnigmaController _enigmaController;
        [SerializeField] private PlugboardCableConnectorController _plugboardConnectorController;

        private static readonly Color[] PlugboardColors =
        {
            Color.HSVToRGB(0.09142857142857142f, 0.75f, 0.80f),
            Color.HSVToRGB(0.14285714285714285f, 0.75f, 0.80f),
            Color.HSVToRGB(0.21428571428571427f, 0.75f, 0.80f),
            Color.HSVToRGB(0.2857142857142857f, 0.75f, 0.80f),
            Color.HSVToRGB(0.35714285714285715f, 0.75f, 0.80f),
            Color.HSVToRGB(0.42857142857142855f, 0.75f, 0.80f),
            Color.HSVToRGB(0.5f, 0.75f, 0.80f),
            Color.HSVToRGB(0.5714285714285714f, 0.75f, 0.80f),
            Color.HSVToRGB(0.6428571428571429f, 0.75f, 0.80f),
            Color.HSVToRGB(0.7142857142857143f, 0.75f, 0.80f),
            Color.HSVToRGB(0.7857142857142858f, 0.75f, 0.80f),
            Color.HSVToRGB(0.8571428571428571f, 0.75f, 0.80f),
            Color.HSVToRGB(0.97f, 0.75f, 0.80f)
        };
        private const float RAYCAST_LENGTH = 100f;

        private LetterPlug _lastLetterPlugHit = null;
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
                if (_lastLetterPlugHit)
                {
                    ClearSelection();
                }
                return;
            }

            char letter = hit.collider.tag[0];
            if (!_letterPlugsMap.TryGetValue(letter, out LetterPlug hitPlug))
            {
                Debug.LogError($"Raycast hit but found no object in letter to object map. Tag hit is {hit.collider.tag}");
                return;
            }

            if (!_lastLetterPlugHit)
            {
                SelectLetterPlug(hitPlug);
                return;
            }

            if (letter == _lastLetterPlugHit.tag[0])
                return;

            PairNewTransposition(hitPlug);
        }

        private void ClearSelection()
        {
            _lastLetterPlugHit.Outline.enabled = false;
            _lastLetterPlugHit = null;
        }

        private void SelectLetterPlug(LetterPlug selected)
        {
            int transpositionsCount = _enigmaController.GetLetterTranspositions().Count;
            if (transpositionsCount >= Consts.ALPHABET_SIZE)
                return;

            selected.Outline.OutlineColor = PlugboardColors[transpositionsCount / 2];
            selected.Outline.enabled = true;
            _lastLetterPlugHit = selected;
        }

        private void PairNewTransposition(LetterPlug secondPlug)
        {
            _enigmaController.AddNewTransposition(_lastLetterPlugHit.tag[0], secondPlug.tag[0]);
            secondPlug.Outline.OutlineColor = _lastLetterPlugHit.Outline.OutlineColor;
            secondPlug.Outline.enabled = true;
            _plugboardConnectorController.RenderNewConnection(_lastLetterPlugHit, secondPlug,
                _lastLetterPlugHit.Outline.OutlineColor);

            _lastLetterPlugHit = null;
        }
    }
}
