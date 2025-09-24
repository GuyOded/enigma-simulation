using AYellowpaper.SerializedCollections;
using Consts;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.VisualScripting;
using UnityEngine;

namespace Enigma.Plugboard
{
    public class PlugboardController : MonoBehaviour
    {
        [SerializedDictionary("Letter", "Game Object")]
        [SerializeField]
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
        [SerializeField] private Transform _deleteConnectionPopupCanvas;
        [SerializeField] private Transform _deleteConnectionPopupMenu;

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
        private TweenerCore<Vector3, Vector3, VectorOptions> _scaleTween = null;

        private void Start()
        {
            _isClickEventsActive = false;
            _deleteConnectionPopupCanvas.gameObject.SetActive(false);
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

        public static bool IsTopPlug(LetterPlug plug)
        {
            return plug.tag.ToUpper()[0] >= LetterPlacement.TopRowRange.Item1 || plug.tag.ToUpper()[0] <= LetterPlacement.TopRowRange.Item2;
        }

        public static bool IsMiddlePlug(LetterPlug plug)
        {
            return plug.tag.ToUpper()[0] >= LetterPlacement.MidRowRange.Item1 || plug.tag.ToUpper()[0] <= LetterPlacement.MidRowRange.Item2;
        }

        public static bool IsBottomPlug(LetterPlug plug)
        {
            return plug.tag.ToUpper()[0] >= LetterPlacement.BotRowRange.Item1 || plug.tag.ToUpper()[0] <= LetterPlacement.BotRowRange.Item2;
        }

        private void CastRayInMouseDirection()
        {
            Ray outgoingRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(outgoingRay, out RaycastHit hit, RAYCAST_LENGTH, LayerMask.GetMask(LayerNames.LETTER_PLUGS, LayerNames.SPLINES)))
            {
                if (_lastLetterPlugHit)
                {
                    ClearSelection();
                }
                return;
            }


            int hitLayer = hit.transform.gameObject.layer;
            if (hitLayer == LayerMask.NameToLayer(LayerNames.LETTER_PLUGS))
            {
                HandlePlugClick(hit.transform.tag[0], hit.transform.tag, hit.transform);
            }

            if (hitLayer == LayerMask.NameToLayer(LayerNames.SPLINES))
            {
                // do something
            }
        }

        private void HandlePlugClick(char letter, string hitTag, Transform hitTransform)
        {
            if (!_letterPlugsMap.TryGetValue(letter, out LetterPlug hitPlug))
            {
                Debug.Log($"Raycast hit but found no object in letter to object map. Tag hit is {hitTag}");
                return;
            }

            if (_enigmaController.GetLetterTranspositions().Keys.Contains(letter) || _enigmaController.GetLetterTranspositions().Values.Contains(letter))
            {
                ShowDeleteConnectionMenu(hitTransform.position);
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

        private void ShowDeleteConnectionMenu(Vector3 position)
        {
            _deleteConnectionPopupCanvas.position = position;
            _scaleTween?.Kill();
            _deleteConnectionPopupMenu.localScale = new Vector3(0.0001f, 0.0001f, 1);
            _deleteConnectionPopupCanvas.gameObject.SetActive(true);
            _scaleTween = _deleteConnectionPopupMenu.DOScale(new Vector3(0.001f, 0.001f, 1), 0.5f).SetEase(Ease.InOutElastic);
        }

        private void ClearSelection()
        {
            _lastLetterPlugHit.Outline.enabled = false;
            _lastLetterPlugHit = null;
        }

        private void SelectLetterPlug(LetterPlug selected)
        {
            int transpositionsCount = _enigmaController.GetLetterTranspositions().Count;
            if (transpositionsCount >= Encryption.Consts.ALPHABET_SIZE)
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

        private void RemoveTransposition(LetterPlug remove)
        {
            // Check if letter plug is connected.
        }
    }
}
