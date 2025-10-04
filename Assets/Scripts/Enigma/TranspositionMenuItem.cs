using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranspositionMenuItem : MonoBehaviour
{
    [SerializeField] private TranspositionMenuController _transpositionMenuController;
    [SerializeField] private TMP_InputField _leftTextField;
    [SerializeField] private TMP_InputField _rightTextField;
    [SerializeField] private Image _outlineComponent;
    [SerializeField] private RectTransform _itemRectTransform;

    private bool _isCurrentlyRepresentingValidTransposition;
    private bool _isFirstTimeEdit = true;

    private readonly Vector2 ShakeStrength = new(5f, 0);

    private char _currentLeft;
    private char _currentRight;

    private void OnEnable()
    {
        _leftTextField.text = "";
        _rightTextField.text = "";
        _isCurrentlyRepresentingValidTransposition = false;
        _isFirstTimeEdit = true;
        _outlineComponent.enabled = false;
    }

    public void OnEndEdit()
    {
        if (_isFirstTimeEdit && (_leftTextField.text == "" || _rightTextField.text == ""))
        {
            return;
        }

        char newLeft = _leftTextField.text.FirstOrDefault();
        char newRight = _rightTextField.text.FirstOrDefault();

        if (_isCurrentlyRepresentingValidTransposition)
        {
            _transpositionMenuController.RemoveTransposition(_currentLeft, _currentRight);
        }

        bool renderConnectionResult = _transpositionMenuController.OnMenuItemEdit(newLeft, newRight);
        if (!renderConnectionResult)
        {
            _outlineComponent.enabled = true;
            _isCurrentlyRepresentingValidTransposition = false;
            _itemRectTransform.DOShakeAnchorPos(0.5f, ShakeStrength);
            return;
        }

        _isCurrentlyRepresentingValidTransposition = true;
        _outlineComponent.enabled = false;
        _currentLeft = newLeft;
        _currentRight = newRight;
    }

    public void SetTransposition(char left, char right)
    {
        _currentLeft = left;
        _currentRight = right;
        _outlineComponent.enabled = false;
        _isFirstTimeEdit = false;
        _leftTextField.text = left.ToString();
        _rightTextField.text = right.ToString();
        _isCurrentlyRepresentingValidTransposition = true;
    }

    public void DisableMenuItem()
    {
        gameObject.SetActive(false);
        if (_isCurrentlyRepresentingValidTransposition)
        {
            _transpositionMenuController.RemoveTransposition(_leftTextField.text.FirstOrDefault(), _rightTextField.text.FirstOrDefault());
        }
        else
        {
            _transpositionMenuController.OnDeleteEmptyMenuItem();
        }
    }

    public void OnDeleteClick()
    {
        DisableMenuItem();
    }
}
