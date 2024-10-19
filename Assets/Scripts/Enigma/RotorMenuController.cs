using UnityEngine;
using UnityEngine.UI;

namespace Enigma
{
    public class RotorMenuController : MonoBehaviour
    {
        [SerializeField] private EnigmaController _enigmaController;

        public void OnLeftRotorUp()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Left, 1);
        }

        public void OnLeftRotorDown()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Left, -1);
        }

        public void OnRightRotorUp()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Right, 1);
        }

        public void OnRightRotorDown()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Right, -1);
        }

        public void OnMiddleRotorUp()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Middle, 1);
        }

        public void OnMiddleRotorDown()
        {
            _enigmaController.RotateRotor(RotorsPlacement.Middle, -1);
        }
    }
}
