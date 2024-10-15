using System;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform _tooltipImage;
        [SerializeField] private string _description;
        [SerializeField] private TooltipDirection _direction;
        [SerializeField] private float _margin = 15;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _tooltipText;
        [SerializeField] private RectTransform _arrow;
        [SerializeField] private RectTransform _tooltipFrame;

        public void Show()
        {
            _tooltipImage.transform.SetParent(transform);
            _tooltipText.text = _description;

            float positionScaler = _direction switch
            {
                TooltipDirection.Bottom or TooltipDirection.Top => _rectTransform.rect.height / 2 + _tooltipImage.rect.height / 2,
                _ => _rectTransform.rect.width / 2 + _tooltipImage.rect.width / 2
            };
            OrientArrow();
            _tooltipImage.anchoredPosition = GetDirectionVector() * (positionScaler + _margin);
            _tooltipImage.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _tooltipImage.gameObject.SetActive(false);
        }

        public void OnValidate()
        {
            _rectTransform = GetComponent<RectTransform>();
            _tooltipText = _tooltipImage.gameObject.GetComponentInChildren<TMP_Text>();
            _tooltipFrame = _tooltipImage.GetChild(_tooltipImage.childCount - 1).GetComponent<RectTransform>();
            _arrow = _tooltipFrame.transform.GetChild(_tooltipFrame.transform.childCount - 1).GetComponent<RectTransform>();
        }

        private void OrientArrow()
        {
            Quaternion rotation = _direction switch
            {
                TooltipDirection.Bottom => Quaternion.Euler(0, 0, 270),
                TooltipDirection.Left => Quaternion.Euler(0, 0, 180),
                TooltipDirection.Right => Quaternion.identity,
                _ => Quaternion.Euler(0, 0, 90)
            };
            _arrow.rotation = rotation;

            float positionScaler = _direction switch
            {
                TooltipDirection.Bottom or TooltipDirection.Top => _tooltipFrame.rect.height / 2,
                _ => _tooltipFrame.rect.width / 2
            };
            _arrow.anchoredPosition = -GetDirectionVector() * positionScaler;
        }

        private Vector2 GetDirectionVector()
        {
            return _direction switch
            {
                TooltipDirection.Left => Vector2.left,
                TooltipDirection.Bottom => Vector2.down,
                TooltipDirection.Right => Vector2.right,
                TooltipDirection.Top => Vector2.up,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum TooltipDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
