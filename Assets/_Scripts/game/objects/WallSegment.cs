using System.Collections;
using UnityEngine;

public class WallSegment : MonoBehaviour
{
    public void Reset()
    {
        if (_blink != null)
        {
            StopCoroutine(_blink);
            _blink = null;
        }
        sprite.color = GameConfiguration.instance.blockColour;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (game.state == GameStates.ActiveGame)
        {
            if (game.invincible)
            {
                roadBuilder.RemoveWall(this);
            }
            else
            {
                _blink = StartCoroutine(BlinkRoutine());
                game.GameOver();
            }
        }
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
            sprite.color = Color.Lerp(GameConfiguration.instance.blockColour, GameConfiguration.instance.blockHitColour, lerp);
            yield return new WaitForFixedUpdate();
        }
    }

    protected Coroutine _blink;

    public IGameController game;

    [Header("References")]
    public SpriteRenderer sprite;

    [Header("Dynamic")]
    public RoadBuilder roadBuilder;
}
