using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Encryption;
using InputHandler;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

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

        public IEnumerable<RotorConfiguration> GetDefaultRotorsConfig()
        {
            List<RotorConfiguration> rotorsConfigurations = new() {
                new RotorConfiguration(Rotors.ROTOR_PROPS_3, 'A'),
                new RotorConfiguration(Rotors.ROTOR_PROPS_2, 'A', _ =>
                {
                    RotateRotorOneStep(_leftGear, _leftLetterWheel).Forget();
                }),
                new RotorConfiguration(Rotors.ROTOR_PROPS_1, 'A', _ =>
                {
                    RotateRotorOneStep(_middleGear, _middleLetterWheel).Forget();
                }),
            };

            return rotorsConfigurations;
        }

        public UniTaskVoid RotateFirstRotorOneStep()
        {
            return RotateRotorOneStep(_rightGear, _rightLetterWheel);
        }

        private async UniTaskVoid RotateRotorOneStep(Transform gear, Transform letterWheel)
        {
            if (DOTween.IsTweening(gear))
            {
                List<Tween> tweens = DOTween.TweensByTarget(gear);
                foreach (Tween tween in tweens)
                {
                    await tween.AsyncWaitForCompletion();
                }
            }

            if (DOTween.IsTweening(letterWheel))
            {
                List<Tween> tweens = DOTween.TweensByTarget(letterWheel);
                foreach (Tween tween in tweens)
                {
                    await tween.AsyncWaitForCompletion();
                }
            }

            gear.DOLocalRotate(Vector3.up * (360f / LETTERS_IN_ENGLISH), _stepAnimationDuration, RotateMode.LocalAxisAdd);
            letterWheel.DORotate(Vector3.up * (360f / LETTERS_IN_ENGLISH), _stepAnimationDuration, RotateMode.LocalAxisAdd);
        }
    }
}
