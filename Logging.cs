using UnityEngine;

namespace DavveDP.Tools
{
    public class Logging : MonoBehaviour
    {
        [SerializeField] bool showLogs;
        [SerializeField] string prefix;
        [SerializeField] Color prefixColor;

        private string HexColor => "#" + ColorUtility.ToHtmlStringRGB(prefixColor);

        public void Log(object message, Object sender)
        {
            if (showLogs)
                Debug.Log($"<color={HexColor}>{prefix}: </color>{message}", sender);
        }
        
    }
}
