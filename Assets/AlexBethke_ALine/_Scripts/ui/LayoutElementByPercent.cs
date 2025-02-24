using UnityEngine;
using UnityEngine.UI;

public class LayoutElementByPercent : MonoBehaviour
{
    void Awake()
    {
        _layout = GetComponent<LayoutElement>();
        if (_layout == null)
        {
            _layout = gameObject.AddComponent<LayoutElement>();
        }

        if (transform.parent != null)
        {
            _parent = transform.parent.GetComponent<RectTransform>();
        }
    }
    public void OnEnable()
    {
        // start of life cycle this value will be 0 still as
        // the parent container has not fully initialized yet
        // so we trigger a delayed call 
        if (_parent.rect.height == 0)
        {
            Invoke("SetCalculatedValues", 0.1f);
        }
        else
        {
            SetCalculatedValues();
        }
    }
    public void SetCalculatedValues()
    {
#if UNITY_EDITOR
        // inspector can call this functionality outside of run time
        // so these are here to cover that scenario
        _layout = GetComponent<LayoutElement>();
        if (_layout == null)
        {
            _layout = gameObject.AddComponent<LayoutElement>();
        }

        if (transform.parent != null)
        {
            _parent = transform.parent.GetComponent<RectTransform>();
        }
#endif

        if (_parent == null)
        {
            Debug.LogError("Orphans can't be laid out by percent");
            return;
        }

        if (preferredHeightPercent > 0)
        {
            _layout.preferredHeight = _parent.rect.height * preferredHeightPercent;
        }
        if (preferredWidthPercent > 0)
        {
            _layout.preferredWidth = _parent.rect.width * preferredWidthPercent;
        }
    }

    protected LayoutElement _layout;
    protected RectTransform _parent;
    [Range(0, 1)]
    public float preferredHeightPercent = 0;
    public float preferredWidthPercent = 0;
}
