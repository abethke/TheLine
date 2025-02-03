using UnityEngine;

public class CameraResizer : MonoBehaviour
{
    void Start()
    {
        Resize();
    }
    public void Resize()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = sizeReference.bounds.size.x / sizeReference.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = sizeReference.bounds.size.y / 2f;
        }
        else
        {
            float difference = targetRatio / screenRatio;
            Camera.main.orthographicSize = sizeReference.bounds.size.y / 2 * difference;
        }
    }
    [Header("Configuration")]
    public SpriteRenderer sizeReference;
}
