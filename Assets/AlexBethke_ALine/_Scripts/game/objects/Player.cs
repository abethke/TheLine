using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ALine
{
    public class Player : MonoBehaviour
    {
        protected void Start()
        {
            _game = ServiceManager.instance.Get<IGameController>();
            _camera = Camera.main;
        }
        public void Init(CalculatedValues in_values)
        {
            _calculated = in_values;
            ResizePlayer(_calculated.playerSize);
        }
        void OnDestroy()
        {
            if (_playerPowerRoutine != null)
            {
                StopCoroutine(_playerPowerRoutine);
                _playerPowerRoutine = null;
            }
        }
        void Update()
        {
#if UNITY_EDITOR
            UpdateForDevCheats();
#endif
            if (_game.state != GameStates.ActiveGame)
                return;

            // only allow user input in the valid input region at the bottom of the screen
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && _game.IsInputValid())
            {
                // check if the player is interacting with a UI element
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                {
                    Utils.Log($"User interacting with UI", GameDebugger.instance.debugUserInput);
                    return;
                }
                Vector3 worldMouse = _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = worldMouse.SetY(_calculated.playerY).SetZ(0);
            }
        }
#if UNITY_EDITOR
        protected void UpdateForDevCheats()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Time.timeScale = 5f;
            }
            else
            {
                Time.timeScale = 1f;
            }

            if (Input.GetKey(KeyCode.H))
            {
                Time.timeScale = 0.5f;
            }

            // dev cheat for invincible
            if (Input.GetKey(KeyCode.I))
            {
                _playerPowerRoutine = StartCoroutine(InvincibleRoutine());
            }
            // dev cheat for small
            if (Input.GetKey(KeyCode.S))
            {
                _playerPowerRoutine = StartCoroutine(SmallRoutine());
            }
        }
#endif
        public void Reset()
        {
            // clean up any active powerup routines
            if (_playerPowerRoutine != null)
            {
                StopCoroutine(_playerPowerRoutine);
                _playerPowerRoutine = null;
            }

            _playerImage.color = GameConfiguration.instance.playerColour;

            // reset the player size
            ResizePlayer(_calculated.playerSize);
        }
        public void PowerUpInvincible()
        {
            _playerPowerRoutine = StartCoroutine(InvincibleRoutine());
        }
        public void PowerUpSmall()
        {
            _playerPowerRoutine = StartCoroutine(SmallRoutine());
        }
        protected IEnumerator InvincibleRoutine()
        {
            Utils.Log("[Power Up] INVINCIBLE!", GameDebugger.instance.debugAppLogic);
            float startedAt = Time.time;
            float blinkDuration = 0.15f;

            _game.invincible = true;
            _powerDisplay.SetText("Break the walls!");
            _powerDisplay.SetTextColor(GameConfiguration.instance.textYellow);
            _powerDisplay.gameObject.SetActive(true);

            while (Time.time - startedAt <= GameConfiguration.instance.invincibleDurationInSeconds)
            {
                // blink player
                // allows for percent over 100% by design
                float percent = (Time.time - startedAt) / blinkDuration;
                float lerp = Mathf.Abs(Mathf.Sin(percent));
                _playerImage.color = Color.Lerp(GameConfiguration.instance.playerColour, GameConfiguration.instance.secondaryYellow, lerp);

                yield return new WaitForEndOfFrame();
            }

            _powerDisplay.SetText("0");
            _powerDisplay.gameObject.SetActive(false);
            _game.invincible = false;

            _playerImage.color = GameConfiguration.instance.playerColour;

            _playerPowerRoutine = null;
        }
        protected IEnumerator SmallRoutine()
        {
            Utils.Log("[Power Up] Small!", GameDebugger.instance.debugAppLogic);
            float playerSmallSize = _calculated.wallWidth * GameConfiguration.instance.playerSizeAsPercentOfWallWidth * 0.5f;
            ResizePlayer(playerSmallSize);

            _powerDisplay.SetText(string.Empty);
            _powerDisplay.gameObject.SetActive(true);

            float startedAt = Time.time;
            while (Time.time - startedAt <= GameConfiguration.instance.smallDurationInSeconds)
            {
                int displayTime = Mathf.FloorToInt(GameConfiguration.instance.smallDurationInSeconds - (Time.time - startedAt));
                _powerDisplay.SetTextColor((displayTime > 4) ? GameConfiguration.instance.textYellow : GameConfiguration.instance.textRed);
                _powerDisplay.SetText($"{displayTime}sec");

                if (_game.state == GameStates.GameOver)
                {
                    yield break;
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            _powerDisplay.SetText(string.Empty);
            _powerDisplay.gameObject.SetActive(false);

            ResizePlayer(_calculated.playerSize);

            _playerPowerRoutine = null;
        }
        protected void ResizePlayer(float in_size)
        {
            transform.localScale = in_size * Vector3.one;
        }
        public bool isPowerActive
        {
            get
            {
                return (_playerPowerRoutine != null);
            }
        }

        protected CalculatedValues _calculated;

        protected Coroutine _playerPowerRoutine;

        protected IGameController _game;
        protected Camera _camera;

        [Header("References")]
        [SerializeField]
        protected SpriteRenderer _playerImage;
        [SerializeField]
        protected PowerInstructions _powerDisplay;
    }
}