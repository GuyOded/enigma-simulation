using Enigma;
using UnityEngine;

public class TranspositionMenuController : MonoBehaviour
{
    [SerializeField] private EnigmaController _enigmaController;

    private void UpdateTranspositionsList()
    {
        _enigmaController.GetLetterTranspositions();
    }
}
