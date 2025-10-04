using System.Collections.Generic;
using System.Linq;
using Enigma;
using Enigma.Plugboard;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class TranspositionMenuController : MonoBehaviour
{
    [SerializeField] private EnigmaController _enigmaController;
    [SerializeField] private TranspositionMenuItem[] _availableMenuItems;
    [SerializeField] private PlugboardController _plugboardController;
    [SerializeField] private GameObject _plusButton;
    [SerializeField] private RectTransform _itemsAndAddButtonLayout;

    private Dictionary<(char, char), TranspositionMenuItem> _transpositionToTranspositionMenuItem = new();

    public void UpdateTranspositionsItemList()
    {
        IDictionary<char, char> transpositions = _enigmaController.GetLetterTranspositions();
        foreach (TranspositionMenuItem item in _availableMenuItems)
        {
            item.gameObject.SetActive(false);
        }

        Dictionary<char, char> transpositionsWithoutDuplicates = transpositions.GroupBy(kvp => kvp.Key < kvp.Value ? (kvp.Key, kvp.Value) : (kvp.Value, kvp.Key))
        .Select(grouping => grouping.First())
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach ((KeyValuePair<char, char> transposition, TranspositionMenuItem menuItem) transpositionToItem in transpositionsWithoutDuplicates.Zip(_availableMenuItems, (kvp, menuItem) => (kvp, menuItem)))
        {
            transpositionToItem.menuItem.gameObject.SetActive(true);
            transpositionToItem.menuItem.SetTransposition(transpositionToItem.transposition.Key, transpositionToItem.transposition.Value);
        }
    }

    public void OnAddMenuItemPressed()
    {
        TranspositionMenuItem availableItem = _availableMenuItems.FirstOrDefault(item => !item.gameObject.activeInHierarchy);
        if (availableItem)
        {
            availableItem.gameObject.SetActive(true);
        }

        if (_availableMenuItems.All(item => item.gameObject.activeInHierarchy))
        {
            _plusButton.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_itemsAndAddButtonLayout);
    }

    public bool OnMenuItemEdit(char left, char right)
    {
        IDictionary<char, char> transpositions = _enigmaController.GetLetterTranspositions();
        if (!StringUtils.IsLetter(left.ToString()) || !StringUtils.IsLetter(right.ToString()))
        {
            return false;
        }

        char leftUpper = left.ToString().ToUpper()[0];
        char rightUpper = right.ToString().ToUpper()[0];

        if (transpositions.ContainsKey(leftUpper) || transpositions.ContainsKey(rightUpper))
        {
            return false;
        }

        _plugboardController.RenderConnection(left, right);
        _enigmaController.AddNewTransposition(left, right, MutationSource.TranspositionMenu);
        return true;
    }

    public void RemoveTransposition(char left, char right)
    {
        _plugboardController.UnrenderTransposition(left, right);
        _enigmaController.RemoveTransposition(left, right, MutationSource.TranspositionMenu);
        _transpositionToTranspositionMenuItem.Remove((left, right));
        _plusButton.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_itemsAndAddButtonLayout);
    }

    public void OnDeleteEmptyMenuItem()
    {
        _plusButton.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_itemsAndAddButtonLayout);
    }
}
