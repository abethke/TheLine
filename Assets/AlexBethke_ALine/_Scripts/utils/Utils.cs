using UnityEngine;
namespace ALine
{
    public class Utils : MonoBehaviour
    {
        static public void Log(string in_message, bool in_debug)
        {
#if !NO_DEBUG
            if (in_debug) Debug.Log(in_message);
#endif
        }
        static public string ScoreForDisplay(float in_score)
        {
            return Mathf.FloorToInt(in_score).ToString();
        }
    }
}