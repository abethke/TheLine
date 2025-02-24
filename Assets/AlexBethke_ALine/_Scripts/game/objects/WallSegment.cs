using System.Collections;
using UnityEngine;

namespace ALine
{
    public class WallSegment : MonoBehaviour
    {
        protected void Start()
        {
            _game = ServiceManager.instance.Get<IGameController>();
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
        private void OnTriggerEnter2D(Collider2D in_other)
        {
            _game.OnWallCollision(this, in_other);
        }
        public void Blink()
        {
            if (_blink != null)
            {
                StopCoroutine(_blink);
            }
            _blink = StartCoroutine(BlinkRoutine());
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

        [Header("References")]
        public SpriteRenderer sprite;
    }
}