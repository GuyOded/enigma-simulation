using System.Collections.Generic;
using DG.Tweening;
using Encryption;
using UnityEngine;

namespace Enigma
{
    public class RotorsController : MonoBehaviour
    {
        [SerializeField] private Transform _rightGear;
        [SerializeField] private Transform _rightLetterWheel;

        [SerializeField] private Transform _middleGear;
        [SerializeField] private Transform _middleLetterWheel;

        [SerializeField] private Transform _leftGear;
        [SerializeField] private Transform _leftLetterWheel;

        [Range(0.1f, 1)] [SerializeField] private float _stepAnimationDuration = 0.1f;

        private const int LETTERS_IN_ENGLISH = 26;

        private readonly Queue<(Transform gear, Transform letterWheel)> _animationQueue = new();

        private void Update()
        {
            if (!_animationQueue.TryPeek(out (Transform gear, Transform letterWheel) peek))
            {
                return;
            }

            if (DOTween.IsTweening(peek.gear) || DOTween.IsTweening(peek.letterWheel))
            {
                return;
            }

            (Transform gear, Transform letterWheel) = _animationQueue.Dequeue();
            gear.DOLocalRotate(Vector3.up * -(360f / LETTERS_IN_ENGLISH), _stepAnimationDuration, RotateMode.LocalAxisAdd);
            letterWheel.DORotate(Vector3.up * -(360f / LETTERS_IN_ENGLISH), _stepAnimationDuration, RotateMode.LocalAxisAdd);
        }

        public ICollection<RotorConfiguration> GetDefaultRotorsConfig()
        {
            List<RotorConfiguration> rotorsConfigurations = new() {
                new RotorConfiguration(Rotors.ROTOR_PROPS_3, 'A'),
                new RotorConfiguration(Rotors.ROTOR_PROPS_2, 'A', _ =>
                {
                    RotateRotorOneStep(_leftGear, _leftLetterWheel);
                }),
                new RotorConfiguration(Rotors.ROTOR_PROPS_1, 'A', _ =>
                {
                    RotateRotorOneStep(_middleGear, _middleLetterWheel);
                }),
            };

            return rotorsConfigurations;
        }

        public void RotateFirstRotorOneStep()
        {
            RotateRotorOneStep(_rightGear, _rightLetterWheel);
        }

        private void RotateRotorOneStep(Transform gear, Transform letterWheel)
        {
            _animationQueue.Enqueue((gear, letterWheel));
        }
    }
}
