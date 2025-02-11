using System.Collections;
using UnityEngine;

public class WallSegment : MonoBehaviour
{
    protected void Start()
    {
        _game = ServiceManager.instance.Get(Services.GAME) as IGameController;
        _roadBuilder= ServiceManager.instance.Get(Services.ROAD_BUILDER) as RoadBuilder;
    }
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
        if (_game.state == GameStates.ActiveGame)
        {
            if (_game.invincible)
            {
                _roadBuilder.RemoveWall(this);
            }
            else
            {
                _blink = StartCoroutine(BlinkRoutine());
                _game.GameOver();
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
            yield return new WaitForEndOfFrame();
        }
    }

    protected Coroutine _blink;
    protected IGameController _game;
    protected RoadBuilder _roadBuilder;

    [Header("References")]
    public SpriteRenderer sprite;
}
