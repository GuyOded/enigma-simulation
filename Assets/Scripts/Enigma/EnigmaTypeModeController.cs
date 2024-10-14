using AYellowpaper.SerializedCollections;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Enigma
{
    public class EnigmaTypeModeController : MonoBehaviour
    {
        [SerializedDictionary("Letter", "Key Game Object")] [SerializeField]
        private SerializedDictionary<string, GameObject> _keys = new()
        {
            { "Q", null },
            { "W", null },
            { "E", null },
            { "R", null },
            { "T", null },
            { "Y", null },
            { "U", null },
            { "I", null },
            { "O", null },
            { "P", null },
            { "A", null },
            { "S", null },
            { "D", null },
            { "F", null },
            { "G", null },
            { "H", null },
            { "J", null },
            { "K", null },
            { "L", null },
            { "Z", null },
            { "X", null },
            { "C", null },
            { "V", null },
            { "B", null },
            { "N", null },
            { "M", null }
        };

        [SerializedDictionary("Bulb", "Key Game Object")] [SerializeField]
        private SerializedDictionary<string, GameObject> _bulbs = new()
        {
            { "Q", null },
            { "W", null },
            { "E", null },
            { "R", null },
            { "T", null },
            { "Y", null },
            { "U", null },
            { "I", null },
            { "O", null },
            { "P", null },
            { "A", null },
            { "S", null },
            { "D", null },
            { "F", null },
            { "G", null },
            { "H", null },
            { "J", null },
            { "K", null },
            { "L", null },
            { "Z", null },
            { "X", null },
            { "C", null },
            { "V", null },
            { "B", null },
            { "N", null },
            { "M", null }
        };

        [SerializeField] private Transform _topKey;
        [SerializeField] private Transform _middleKey;
        [SerializeField] private Transform _bottomKey;
        [SerializeField] private Mesh _keyMesh;
        [SerializeField] private RotorsController _rotorsController;
        [SerializeField] private GameObject _lightCube;
        [Range(0, 1)] [SerializeField] private float _keyDownAnimationBottomLimit = 0.2f; // The amount that the key will be pressed down relatively to it's height.
        [Range(0.05f, 1)] [SerializeField] private float _animationDuration = 0.1f;

        private string _lastKeyPressed;
        private float _keyHeight;

        private float _bottomKeysIdleZPosition;
        private float _middleKeysIdleZPosition;
        private float _topKeysIdleZPosition;

        private const string TOP_KEYS_LETTERS = "QWERTZUIO";
        private const string MID_KEYS_LETTERS = "ASDFGHJK";
        private const string BOT_KEYS_LETTERS = "PYXCVBMNL";

        private void Start()
        {
            _lightCube.SetActive(false);

            _topKeysIdleZPosition = _topKey.localPosition.z;
            _middleKeysIdleZPosition = _middleKey.localPosition.z;
            _bottomKeysIdleZPosition = _bottomKey.localPosition.z;

            _keyHeight = _keyMesh.bounds.size.z;
        }

        public void OnKeyDown(string key, string encrypted)
        {
            if (!StringUtils.IsLetter(key))
                return;

            _lastKeyPressed = key;
            AnimateKeyDown(key.ToUpper());
            MoveLightCubeUnderBulb(encrypted);
            _lightCube.SetActive(true);
            _rotorsController.RotateFirstRotorOneStep();
        }

        public void OnKeyUp(string key)
        {
            if (!StringUtils.IsLetter(key))
                return;

            if (_lastKeyPressed == key)
                _lightCube.SetActive(false);

            AnimateKeyUp(key.ToUpper());
        }

        private void AnimateKeyDown(string key)
        {
            if (!_keys.TryGetValue(key, out GameObject keyObject))
                return;

            if (keyObject == null)
                return;

            if (DOTween.IsTweening(keyObject.transform))
                DOTween.Kill(keyObject.transform);

            float idlePosition = GetLetterIdleZPosition(key);

            keyObject.transform.DOLocalMoveZ(idlePosition - _keyHeight * _keyDownAnimationBottomLimit, _animationDuration);
        }

        private void AnimateKeyUp(string key)
        {
            if (!_keys.TryGetValue(key, out GameObject keyObject))
                return;

            if (keyObject == null)
                return;

            float idlePosition = GetLetterIdleZPosition(key);

            keyObject.transform.DOLocalMoveZ(idlePosition, _animationDuration);
        }

        private float GetLetterIdleZPosition(string key)
        {
            float idlePosition = 0;

            if (TOP_KEYS_LETTERS.Contains(key))
                idlePosition = _topKeysIdleZPosition;
            else if (MID_KEYS_LETTERS.Contains(key))
                idlePosition = _middleKeysIdleZPosition;
            else if (BOT_KEYS_LETTERS.Contains(key))
                idlePosition = _bottomKeysIdleZPosition;

            return idlePosition;
        }

        private void MoveLightCubeUnderBulb(string key)
        {
            if(!_bulbs.TryGetValue(key.ToUpper(), out GameObject bulb))
                return;

            if (bulb == null)
                return;

            Vector3 bulbPosition = bulb.transform.position;
            float cubePositionY = _lightCube.transform.position.y;

            _lightCube.transform.position = new Vector3(bulbPosition.x, cubePositionY, bulbPosition.z);
        }
    }
}
