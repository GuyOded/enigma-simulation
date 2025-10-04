using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Consts;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using Utils;

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
        [SerializeField] private RectTransform _deleteConnectionPopupCanvas;
        [SerializeField] private RectTransform _deleteConnectionPopupMenu;
        [SerializeField] private TMP_Text _deleteConnectionPopupMenuText;
        [SerializeField] private Collider _plugCollider;

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
        private float _plugRadius;
        private bool _isClickEventsActive;
        private TweenerCore<Vector3, Vector3, VectorOptions> _scaleTween = null;
        private (char, char)? _deleteMenuLetters = null;

        private void Start()
        {
            _isClickEventsActive = false;
            _deleteConnectionPopupCanvas.gameObject.SetActive(false);
            _plugRadius = Mathf.Abs(_plugCollider.bounds.max.y - _plugCollider.bounds.min.y) / 2;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _isClickEventsActive && !_deleteConnectionPopupCanvas.gameObject.activeSelf)
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

        /// <summary>
        /// Renders a new transposition connection. Does not add the connection to the enigma controller state (assumes the caller is responsible for that)
        /// </summary>
        /// <param name="first">First letter in connection</param>
        /// <param name="second">Second letter in connection</param>
        /// <exception cref="ArgumentException">If the chars given are not letters or a transposition including one of them already exist</exception>
        public void RenderConnection(char first, char second)
        {
            if (!StringUtils.IsLetter(first.ToString()) || !StringUtils.IsLetter(second.ToString()))
            {
                throw new ArgumentException($"Both arguments must be letters {first}, {second}");
            }

            IDictionary<char, char> currentTranspositions = _enigmaController.GetLetterTranspositions();
            if (currentTranspositions.ContainsKey(first) || currentTranspositions.ContainsKey(second))
            {
                throw new ArgumentException("Characters given are already exist in a transposition");
            }

            Color outlineColor = PlugboardColors[currentTranspositions.Count / 2];
            LetterPlug firstPlug = _letterPlugsMap[first];
            LetterPlug secondPlug = _letterPlugsMap[second];

            firstPlug.Outline.enabled = true;
            firstPlug.Outline.OutlineColor = outlineColor;
            secondPlug.Outline.enabled = true;
            secondPlug.Outline.OutlineColor = outlineColor;

            _plugboardConnectorController.RenderNewConnection(firstPlug, secondPlug, outlineColor);
        }

        public void UnrenderTransposition(char first, char second)
        {
            ClearPlugOutline(first);
            ClearPlugOutline(second);

            _plugboardConnectorController.RemoveTranspositionSpline(first, second);
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
                (char, char)? plugsConnectedToSpline = _plugboardConnectorController.FindLettersFromSpline(hit.transform.gameObject);
                if (!plugsConnectedToSpline.HasValue)
                {
                    Debug.LogError($"Can't find spline from the raycast hit. Hit: {hit.transform.gameObject}.");
                    return;
                }

                ShowDeleteConnectionMenu(hit.point, (plugsConnectedToSpline.Value.Item1, plugsConnectedToSpline.Value.Item2));
            }
        }

        private void HandlePlugClick(char letter, string hitTag, Transform hitTransform)
        {
            if (!_letterPlugsMap.TryGetValue(letter, out LetterPlug hitPlug))
            {
                Debug.Log($"Raycast hit but found no object in letter to object map. Tag hit is {hitTag}");
                return;
            }

            (char, char)? transposition = _enigmaController.GetTranspositionByLetter(letter);
            if (transposition.HasValue)
            {
                ShowDeleteConnectionMenu(hitTransform.position + Vector3.up * _plugRadius, transposition.Value);
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

        private void ShowDeleteConnectionMenu(Vector3 position, (char, char) letters)
        {
            Vector3 screenPoint = _mainCamera.WorldToScreenPoint(position);
            _deleteConnectionPopupMenu.anchoredPosition = screenPoint;
            _deleteMenuLetters = letters;
            _deleteConnectionPopupMenuText.text = $"{letters.Item1}-{letters.Item2}";
            _scaleTween?.Kill();
            _deleteConnectionPopupMenu.localScale = Vector3.zero + Vector3.forward;
            _deleteConnectionPopupCanvas.gameObject.SetActive(true);
            _scaleTween = _deleteConnectionPopupMenu.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutElastic);
        }

        public void HideDeleteConnectionPopupMenu()
        {
            _scaleTween?.Kill();
            _deleteConnectionPopupCanvas.gameObject.SetActive(false);
            _deleteMenuLetters = null;
        }

        public void OnPopupMenuDelete()
        {
            if (!_deleteMenuLetters.HasValue)
            {
                Debug.LogWarning("Delete button in popup menu clicked with no letters assigned.");
                return;
            }
            UnrenderTransposition(_deleteMenuLetters.Value.Item1, _deleteMenuLetters.Value.Item2);
            _enigmaController.RemoveTransposition(_deleteMenuLetters.Value.Item1, _deleteMenuLetters.Value.Item2);
            HideDeleteConnectionPopupMenu();
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


        private void ClearPlugOutline(char letter)
        {
            LetterPlug plug = _letterPlugsMap[letter];
            plug.Outline.enabled = false;
        }
    }
}
