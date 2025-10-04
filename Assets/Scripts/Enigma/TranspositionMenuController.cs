using System.Collections.Generic;
using Enigma;
using Enigma.Plugboard;
using UnityEngine;
using Utils;

public class TranspositionMenuController : MonoBehaviour
{
    [SerializeField] private EnigmaController _enigmaController;
    [SerializeField] private TranspositionMenuItem[] _availableMenuItems;
    [SerializeField] private PlugboardController _plugboardController;

    private Dictionary<(char, char), TranspositionMenuItem> _activeTranspositions = new();

    private void UpdateTranspositionsItemList()
    {
        _enigmaController.GetLetterTranspositions();
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
        _enigmaController.AddNewTransposition(left, right);
        return true;
    }

    public void RemoveTransposition(char left, char right)
    {
        _plugboardController.UnrenderTransposition(left, right);
        _enigmaController.RemoveTransposition(left, right);
    }
}
