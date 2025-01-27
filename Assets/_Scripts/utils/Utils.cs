using UnityEngine;

public class Utils : MonoBehaviour
{
    static public void Log(string in_message, bool in_debugFlag)
    {
#if !NO_DEBUG
        if (in_debugFlag) Debug.Log(in_message);
#endif
    }
}
