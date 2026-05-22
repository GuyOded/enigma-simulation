using UnityEngine;
using System.Runtime.InteropServices;

class ClipboardController : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CopyToClipboard(string text);
#endif

    public void CopyText(string text)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CopyToClipboard(text);
#else
        Debug.Log($"Coppied: {text}");
#endif
    }
}