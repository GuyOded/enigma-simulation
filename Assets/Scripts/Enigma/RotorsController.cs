using System;
using System.Collections.Generic;
using DG.Tweening;
using Encryption;
using Unity.Mathematics;
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

        [Range(0.1f, 1)][SerializeField] private float _stepAnimationDuration = 0.1f;

        private readonly Queue<(Transform gear, Transform letterWheel, bool rotateBack)> _animationQueue = new();

        private void Update()
        {
            if (!_animationQueue.TryPeek(out (Transform gear, Transform letterWheel, bool rotateBack) peek))
            {
                return;
            }

            if (DOTween.IsTweening(peek.gear) || DOTween.IsTweening(peek.letterWheel))
            {
                return;
            }

            (Transform gear, Transform letterWheel, bool rotateBack) = _animationQueue.Dequeue();
            int directionScaler = rotateBack ? -1 : 1;
            gear.DOLocalRotate(Vector3.up * (-(360f / Encryption.Consts.ALPHABET_SIZE) * directionScaler), _stepAnimationDuration, RotateMode.LocalAxisAdd);
            letterWheel.DORotate(Vector3.up * (-(360f / Encryption.Consts.ALPHABET_SIZE) * directionScaler), _stepAnimationDuration, RotateMode.LocalAxisAdd);
        }

        public ICollection<RotorConfiguration> GetDefaultRotorsConfig()
        {
            List<RotorConfiguration> rotorsConfigurations = new() {
                new RotorConfiguration(Rotors.ROTOR_PROPS_1, 'A'),
                new RotorConfiguration(Rotors.ROTOR_PROPS_1, 'A', _ =>
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

        public void StepwiseRotateRotor(RotorsPlacement rotor, int steps, bool animate = true)
        {
            (Transform gear, Transform letterWheel) = rotor switch
            {
                RotorsPlacement.Left => (_leftGear, _leftLetterWheel),
                RotorsPlacement.Middle => (_middleGear, _middleLetterWheel),
                RotorsPlacement.Right => (_rightGear, _rightLetterWheel),
                _ => throw new ArgumentOutOfRangeException(nameof(rotor), rotor, null)
            };

            if (animate)
            {
                for (int i = 0; i < math.abs(steps); i++)
                {
                    RotateRotorOneStep(gear, letterWheel, steps < 0);
                }
                return;
            }

            gear.localRotation *= Quaternion.Euler(gear.localRotation.eulerAngles + Vector3.up * (-360f / Encryption.Consts.ALPHABET_SIZE) * steps);
            letterWheel.Rotate(Vector3.up, -360f / Encryption.Consts.ALPHABET_SIZE * steps);
        }

        public void RotateRotor(RotorsPlacement rotor, int steps)
        {
            (Transform gear, Transform letterWheel) = rotor switch
            {
                RotorsPlacement.Left => (_leftGear, _leftLetterWheel),
                RotorsPlacement.Middle => (_middleGear, _middleLetterWheel),
                RotorsPlacement.Right => (_rightGear, _rightLetterWheel),
                _ => throw new ArgumentOutOfRangeException(nameof(rotor), rotor, null)
            };

            letterWheel.DOLocalRotate(Vector3.up * (-360f / Encryption.Consts.ALPHABET_SIZE) * steps, 1, RotateMode.LocalAxisAdd);
            gear.DOLocalRotate(Vector3.up * (-360f / Encryption.Consts.ALPHABET_SIZE) * steps, 1, RotateMode.LocalAxisAdd);
        }

        private void RotateRotorOneStep(Transform gear, Transform letterWheel, bool rotateBack = false)
        {
            _animationQueue.Enqueue((gear, letterWheel, rotateBack));
        }
    }
}
