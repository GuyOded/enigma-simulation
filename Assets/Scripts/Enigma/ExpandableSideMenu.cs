using System;
using DG.Tweening;
using UnityEngine;

namespace Enigma
{
    public class ExpandableSideMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform _expandArrow;
        [SerializeField] private RectTransform _menuFrameContainer;
        [SerializeField] private RectTransform _menuFrame;
        [HideInInspector] [SerializeField] private bool _isExpanded = true;

        private const float ANIMATION_DURATION = 1f;

        private void Start()
        {
            _isExpanded = true;
        }

        public void ToggleExpansionState(bool isExpanded)
        {
            _isExpanded = isExpanded;
            if (_isExpanded)
            {
                Expand();
                return;
            }

            Collapse();
        }

        public void Show()
        {
            if (DOTween.IsTweening(_menuFrameContainer))
                DOTween.Kill(_menuFrameContainer);

            float xPos = _isExpanded ? 0 : _menuFrame.rect.width;
            _menuFrameContainer.DOAnchorPosX(xPos, ANIMATION_DURATION).SetEase(Ease.OutQuart);
        }

        public void Hide(Action callback = null)
        {
            if (DOTween.IsTweening(_menuFrameContainer))
                DOTween.Kill(_menuFrameContainer);

            _menuFrameContainer.DOAnchorPosX(_menuFrameContainer.rect.width, ANIMATION_DURATION).SetEase(Ease.OutQuart).OnComplete(() => callback?.Invoke());
        }

        private void Expand()
        {
            if (DOTween.IsTweening(_expandArrow))
                DOTween.Kill(_expandArrow);

            if (DOTween.IsTweening(_menuFrameContainer))
                DOTween.Kill(_menuFrameContainer);

            _expandArrow.DORotate(Vector3.zero, ANIMATION_DURATION).SetEase(Ease.OutQuart);
            _menuFrameContainer.DOAnchorPosX(0, ANIMATION_DURATION).SetEase(Ease.OutQuart);
        }

        private void Collapse()
        {
            if (DOTween.IsTweening(_expandArrow))
                DOTween.Kill(_expandArrow);

            if (DOTween.IsTweening(_menuFrameContainer))
                DOTween.Kill(_menuFrameContainer);

            _expandArrow.DORotate(180 * Vector3.forward, ANIMATION_DURATION).SetEase(Ease.OutQuart);
            _menuFrameContainer.DOAnchorPosX(_menuFrame.rect.width, ANIMATION_DURATION).SetEase(Ease.OutQuart);
        }
    }
}
