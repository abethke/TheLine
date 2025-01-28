using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Constants;
public class WallSegement : MonoBehaviour
{
    void Start() { }
    public void Reset()
    {
        if (_blink != null)
        {
            StopCoroutine(_blink);
            _blink = null;
        }
        _img.color = BLOCK_COLOUR;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (refs.game.state == GameController.GameStates.ActiveGame)
        {
            if (refs.game.invincible)
            {
                refs.game.RemoveWall(this);
            }
            else
            {
                _blink = StartCoroutine(BlinkRoutine());
                refs.game.GameOver();
            }
        }
    }
    public Vector2 anchoredPosition
    {
        set { rectTransform.anchoredPosition = value; }
        get { return rectTransform.anchoredPosition; }
    }
    protected IEnumerator BlinkRoutine()
    {
        float startedAt = Time.time;
        float blinkDuration = 0.25f;
        while (true)
        {
            // allows for percent over 100% by design
            float percent = (Time.time - startedAt) / blinkDuration;
            float lerp = Mathf.Abs(Mathf.Sin(percent));
            _img.color = Color.Lerp(BLOCK_COLOUR, BLOCK_HIT_COLOUR, lerp);
            yield return new WaitForFixedUpdate();
        }
    }

    protected Coroutine _blink;

    [Header("References")]
    public RectTransform rectTransform;
    [SerializeField]
    protected Image _img;

    [Header("Dynamic")]
    public SharedReferences refs;
}
