using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using static SavedValues;
using static Constants;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region Life Cycle
    void Start()
    {
        Utils.Log("Starting Application", debugAppLogic);
        refs.mainMenu.gameObject.SetActive(false);
        refs.gameOver.gameObject.SetActive(false);
        _powerPickup.gameObject.SetActive(false);
        _powerDisplayContainer.gameObject.SetActive(false);
        instructions.gameObject.SetActive(true);
        scoreDisplay.text = "0";

        // get references
        _playerRect = refs.player.GetComponent<RectTransform>();
        _playerCollider = refs.player.GetComponent<CircleCollider2D>();
        _playerImage = refs.player.GetComponent<Image>();

        CalculateValuesBasedOnScreenResolution();
        ConfigureElementsBasedOnScreenResolution();

        InitObjectPool();
        CreateRoadGenerationData();
        GenerateStartWalls();
    }
    protected void CalculateValuesBasedOnScreenResolution()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        Utils.Log($"Screen dimensions: {Screen.width}, {Screen.height}", debugResolutionCalculations);
        Utils.Log($"Canvas dimensions: {canvasWidth}, {canvasHeight}", debugResolutionCalculations);
        _scaleFactorForResolution = canvasWidth / Screen.width;
        Utils.Log($"Scale factor based on canvas vs screen resolution: {_scaleFactorForResolution}", debugResolutionCalculations);
        _wallWidth = canvasWidth / (float)WALL_COLS;
        _wallHeight = canvasHeight / (float)WALL_ROWS;

        float playerSize = _wallWidth * PLAYER_SIZE;

        _playerHalfWidth = playerSize * 0.5f;
        _playerStartX = canvasWidth * 0.5f - _playerHalfWidth;
        _playerY = _wallHeight * 2.5f;

        _removeY = -canvasHeight - _wallHeight * 0.5f;
        _buildYStart = -canvasHeight + _wallHeight * 2.5f;

        _ignoreInputAboveY = INSTRUCTIONS_OFFSET * _wallHeight + INSTRUCTIONS_HEIGHT * _wallHeight;

        _moveSpeed = _wallHeight * -MOVEMENT_SPEED;

        Utils.Log($"Wall Width: {_wallWidth}", debugResolutionCalculations);
        Utils.Log($"Wall Height: {_wallHeight}", debugResolutionCalculations);
        Utils.Log($"Player half width: {_playerHalfWidth}", debugResolutionCalculations);
        Utils.Log($"Player start x: {_playerStartX}", debugResolutionCalculations);
        Utils.Log($"Walls build from: {_buildY}", debugResolutionCalculations);
        Utils.Log($"Walls removed at: {_removeY}", debugResolutionCalculations);

        // resize the player
        _playerRect.sizeDelta = playerSize * Vector2.one;
        _playerCollider.offset = (playerSize - PLAYER_COLLIDER_SIZE_REDUCTION) * 0.5f * Vector2.one;
        _playerCollider.radius = (playerSize - PLAYER_COLLIDER_SIZE_REDUCTION) * 0.5f;

        // resize the power pickup
        _powerPickup.rectTransform.sizeDelta = playerSize * 0.5f * Vector2.one;
        _powerPickup.collider.offset = playerSize * 0.5f * Vector2.one;
        _powerPickup.collider.size = playerSize * Vector2.one;
    }
    protected void ConfigureElementsBasedOnScreenResolution()
    {
        // position player at start
        _playerRect.anchoredPosition = new Vector2(_playerStartX, _playerY);

        float instructionsHeight = INSTRUCTIONS_HEIGHT * _wallHeight;
        float instructionsY = instructionsHeight;
        RectTransform instructionsRect = instructions.gameObject.RectTransform();
        instructionsRect.sizeDelta = instructionsRect.sizeDelta.SetY(instructionsY);
        instructionsRect.anchoredPosition = new Vector2(0, instructionsY);

        RectTransform powerDisplayRect = _powerDisplayContainer.RectTransform();
        powerDisplayRect.sizeDelta = powerDisplayRect.sizeDelta.SetY(instructionsHeight * 0.5f);
        powerDisplayRect.anchoredPosition = new Vector2(0, instructionsY + instructionsHeight);
    }
    void OnDestroy()
    {
        _wallPool.Clear();
    }
    #endregion Life Cycle
    #region Wall object pooling
    protected void InitObjectPool()
    {
        // max pool is a full screen of wall segments minus the one we're in and
        // the one we're moving to, plus the build row
        int maxPoolSize = (WALL_ROWS + 1) * WALL_COLS - 2;
        Utils.Log("Initializing object pool for walls", debugAppLogic);
        _wallPool = new ObjectPool<WallSegement>(CreateWall, OnTakeFromPool, OnReturnToPool, OnDestroyPooledObject, true, maxPoolSize);
    }
    protected WallSegement CreateWall()
    {
        GameObject instance = Instantiate(_wallPrefab, _wallContainer);

        RectTransform rectTransform = instance.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(_wallWidth, _wallHeight);

        BoxCollider2D collider = instance.GetComponent<BoxCollider2D>();
        collider.size = rectTransform.sizeDelta;

        WallSegement wall = instance.GetComponent<WallSegement>();
        wall.refs = refs;
        instance.SetActive(false);
        return wall;
    }
    protected void OnTakeFromPool(WallSegement in_wall)
    {
        in_wall.Reset();
        walls.Add(in_wall);
        in_wall.gameObject.SetActive(true);
    }
    protected void OnReturnToPool(WallSegement in_wall)
    {
        walls.Remove(in_wall);
        in_wall.gameObject.SetActive(false);
    }
    protected void OnDestroyPooledObject(WallSegement in_wall)
    {
        walls.Remove(in_wall);
        Destroy(in_wall.gameObject);
    }
    #endregion Wall object pooling
    #region Game Loop
    void Update()
    {
        switch (state)
        {
            case GameStates.WaitingToStart:
                if (Input.GetMouseButtonDown(0))
                {
                    StartGame();
                }
                break;
            case GameStates.ActiveGame:
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    // check if the player is interacting with a UI element
                    if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                    {
                        Utils.Log($"User interacting with UI", debugUserInput);
                        return;
                    }
                    Vector3 scaledMousePosition = Input.mousePosition * _scaleFactorForResolution;
                    Utils.Log($"mousePosition position: {Input.mousePosition}", debugUserInput);
                    Utils.Log($"scaled position: {scaledMousePosition}", debugUserInput);
                    // only allow user input in the valid input region at the bottom of the screen
                    if (scaledMousePosition.y < _ignoreInputAboveY)
                    {
                        _playerRect.anchoredPosition = scaledMousePosition.SetY(_playerY).PlusX(-_playerHalfWidth);
                    }

                    if (_powerDisplayContainer.activeSelf)
                    {
                        float newX = _playerRect.anchoredPosition.x - Screen.width * _scaleFactorForResolution * 0.5f;
                        _powerDisplay.rectTransform.anchoredPosition = _powerDisplay.rectTransform.anchoredPosition.SetX(newX);
                    }
                }
                score += Time.deltaTime * 10f;
                scoreDisplay.text = $"{ScoreForDisplay}";

                break;
            case GameStates.GameOver:
                // do nothing
                break;
        }

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Space))
        {
            Time.timeScale = 5f;
        }
        else
        {
            Time.timeScale = 1f;
        }
#endif
    }
    void FixedUpdate()
    {
        if (state != GameStates.ActiveGame)
            return;

        //update Move to update for smoother outcome?
        foreach (WallSegement wall in walls)
        {
            wall.rectTransform.anchoredPosition = wall.rectTransform.anchoredPosition.PlusY(_moveSpeed * Time.fixedDeltaTime);
        }

        if (_powerPickup.gameObject.activeSelf)
        {
            _powerPickup.rectTransform.anchoredPosition = _powerPickup.rectTransform.anchoredPosition.PlusY(_moveSpeed * Time.fixedDeltaTime);
        }

        UpdateForLineRemoval();
        UpdateForNewLineCreation();
    }
    public void StartGame()
    {
        Debug.Log("Start game");
        scoreDisplay.text = "0";
        instructions.FadeOut();
        state = GameStates.ActiveGame;
    }
    public void GameOver()
    {
        Debug.Log("GAME OVER");
        state = GameStates.GameOver;

        int finalScore = Mathf.FloorToInt(score);
        int bestScore = PlayerPrefs.HasKey(SAVED_BEST_SCORE) ? PlayerPrefs.GetInt(SAVED_BEST_SCORE) : 0;
        Debug.Log($"final score {finalScore} vs best score {bestScore}");
        if (finalScore > bestScore)
        {
            PlayerPrefs.SetInt(SAVED_BEST_SCORE, finalScore);
        }

        StartCoroutine(GameOverRoutine());
    }
    protected IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(3f);

        refs.gameOver.gameObject.SetActive(true);
        instructions.FadeIn();
    }
    public void ResetGame()
    {
        score = 0;
        while (walls.Count > 0)
        {
            WallSegement wall = walls[0];
            _wallPool.Release(wall);
        }

        if (_playerPowerRoutine != null)
        {
            StopCoroutine(_playerPowerRoutine);
            _playerPowerRoutine = null;
        }

        _playerRect.anchoredPosition = new Vector2(_playerStartX, _playerY);
        _playerImage.color = PLAYER_COLOUR;
        _powerDisplayContainer.gameObject.SetActive(false);

        pathConnection = 3;
        segmentsUntilPowerUp = Random.Range(_numSegmentsUntilPowerUpMin, _numSegmentsUntilPowerUpMax);
        GenerateStartWalls();
    }
    #endregion Game Loop
    #region Road Generation
    protected void CreateRoadGenerationData()
    {
        // create the data for road generation 
        int index = 0;
        _segmentQueue = new int[numBendsInQueue + numStraightsInQueue];
        for (int i = 0; i < numBendsInQueue; i++)
        {
            _segmentQueue[index] = BEND_SEGMENT;
            index++;
        }
        for (int i = 0; i < numStraightsInQueue; i++)
        {
            _segmentQueue[index] = STRAIGHT_SEGMENT;
            index++;
        }
    }
    protected void UpdateForNewLineCreation()
    {
        if (_powerPickup.rectTransform.anchoredPosition.y < _removeY)
        {
            _powerPickup.gameObject.SetActive(false);
        }

        if (walls.Count == 0)
            return;

        WallSegement lastWall = walls[walls.Count - 1];
        if (lastWall.anchoredPosition.y > 0)
            return;

        _buildY = lastWall.anchoredPosition.y + _wallHeight;
        GenerateNextRoadSegment();
    }
    protected void UpdateForLineRemoval()
    {
        if (walls.Count == 0)
            return;

        bool clearing = true;
        while (clearing)
        {
            if (walls.Count == 0)
                break;

            WallSegement wall = walls[0];
            if (wall.anchoredPosition.y < _removeY)
            {
                _wallPool.Release(wall);
            }
            else
            {
                clearing = false;
            }
        }
    }
    protected void GenerateStartWalls()
    {
        // reset  the build position to default
        _buildY = _buildYStart;

        _segmentQueue.Shuffle();
        _segmentIndex = 0;
        Utils.Log($"Shuffled road segment queue: {_segmentQueue.ToStringForReal()}", debugRoadGeneration);

        for (int i = 0; i < LAYOUT_AT_START.Length; i++)
        {
            for (int j = 0; j < LAYOUT_AT_START[i].Length; j++)
            {
                float x = _wallWidth * 0.5f + j * _wallWidth;
                if (LAYOUT_AT_START[i][j] == '0')
                    continue;

                WallSegement wall = _wallPool.Get();
                wall.anchoredPosition = new Vector2(x, _buildY);
            }
            _buildY += _wallHeight;
        }

        lastPathDelta = 0;
    }
    protected void GenerateNextRoadSegment()
    {
        bool isStraight = (_segmentQueue[_segmentIndex] == STRAIGHT_SEGMENT);
        Utils.Log($"Generating next road segment: {(isStraight ? "Straight" : "Bend")}", debugRoadGeneration);
        if (isStraight)
        {
            // build a straight path from the current connection
            for (int i = 0; i < WALL_COLS; i++)
            {
                if (i == pathConnection)
                    continue;

                WallSegement wall = _wallPool.Get();
                wall.anchoredPosition = new Vector2(_wallWidth * 0.5f + i * _wallWidth, _buildY);
            }

            lastPathDelta = 0;
        }
        else
        {
            // build a bend segment
            Vector2Int pathways = PATHS_BY_POSITION[pathConnection - 1];
            int offset = Random.Range(pathways.x, pathways.y);
            // stop unintended straight spawns
            while (offset == 0)
            {
                offset = Random.Range(pathways.x, pathways.y);
            }

            int start = pathConnection;
            int end = pathConnection + offset;

            int pathDelta = end - start;
            // check for potential bend overlaps
            if ((pathDelta < 0 && lastPathDelta > 0) || (pathDelta > 0 && lastPathDelta < 0))
            {
                if (pathConnection == 1 || pathConnection == WALL_COLS - 2)
                {
                    Utils.Log("Road special case: Overlapping bend at edge switching to straight", debugRoadGeneration);
                    end = start;
                }
                else
                {
                    Utils.Log("Road special case: Overlapping bend", debugRoadGeneration);
                    end = start + lastPathDelta / Mathf.Abs(lastPathDelta);
                    pathDelta = end - start;
                }
            }
            lastPathDelta = pathDelta;

            // generate the segment
            for (int i = 0; i < WALL_COLS; i++)
            {
                if (i < Mathf.Min(start, end) || i > Mathf.Max(start, end))
                {
                    WallSegement wall = _wallPool.Get();
                    wall.anchoredPosition = new Vector2(_wallWidth * 0.5f + i * _wallWidth, _buildY);
                }
            }

            pathConnection = end;
        }

        // check for queue roll over
        _segmentIndex++;
        if (_segmentIndex >= _segmentQueue.Length)
        {
            _segmentQueue.Shuffle();
            _segmentIndex = 0;
            Utils.Log($"Shuffled road segment queue: {_segmentQueue.ToStringForReal()}", debugRoadGeneration);
        }

        // no power spawning while one is active
        if (_playerPowerRoutine != null || _powerPickup.gameObject.activeSelf)
            return;

        segmentsUntilPowerUp--;
        if (segmentsUntilPowerUp > 0)
            return;

        segmentsUntilPowerUp = Random.Range(_numSegmentsUntilPowerUpMin, _numSegmentsUntilPowerUpMax);
        SpawnPowerUpAt(pathConnection, _buildY);
    }
    protected void SpawnPowerUpAt(int in_position, float in_y)
    {
        _powerPickup.rectTransform.anchoredPosition = new Vector2(_wallWidth * 0.5f + in_position * _wallWidth - _powerPickup.rectTransform.sizeDelta.x * 0.5f, in_y);
        _powerPickup.mode = (Random.Range(0, 2) == 0) ? PowerPickup.Modes.Invincible : PowerPickup.Modes.Small;
        _powerPickup.gameObject.SetActive(true);
        Utils.Log($"Spawning powerup[{_powerPickup.mode}] at: {in_position}", debugRoadGeneration);
    }
    #endregion Road Generation
    #region Powerups
    public void ActivatePower()
    {
        Debug.Log("Activating power: " + _powerPickup.mode);
        _powerPickup.gameObject.SetActive(false);
        _powerDisplay.color = TEXT_YELLOW;

        if (_powerPickup.mode == PowerPickup.Modes.Invincible)
        {
            _playerPowerRoutine = StartCoroutine(InvincibleRoutine());
        }
        else
        {
            _playerPowerRoutine = StartCoroutine(SmallRoutine());
        }
    }
    protected IEnumerator InvincibleRoutine()
    {
        Utils.Log("[Power Up] INVINCIBLE!", debugAppLogic);
        float startedAt = Time.time;
        float blinkDuration = 0.15f;

        invincible = true;
        _powerDisplay.text = "Break the walls!";
        _powerDisplayContainer.SetActive(true);

        while (Time.time - startedAt <= INVINCIBLE_DURATION_IN_SECONDS)
        {
            // blink player
            // allows for percent over 100% by design
            float percent = (Time.time - startedAt) / blinkDuration;
            float lerp = Mathf.Abs(Mathf.Sin(percent));
            _playerImage.color = Color.Lerp(PLAYER_COLOUR, SECONDARY_YELLOW, lerp);

            yield return new WaitForFixedUpdate();
        }

        _powerDisplay.text = "0";
        _powerDisplayContainer.SetActive(false);
        invincible = false;

        _playerImage.color = PLAYER_COLOUR;

        _playerPowerRoutine = null;
    }
    protected IEnumerator SmallRoutine()
    {
        Utils.Log("[Power Up] Small!", debugAppLogic);

        float playerSize = _wallWidth * PLAYER_SIZE * 0.5f;

        _playerRect.sizeDelta = playerSize * Vector2.one;
        _playerCollider.offset = (playerSize - PLAYER_COLLIDER_SIZE_REDUCTION) * 0.5f * Vector2.one;
        _playerCollider.radius = (playerSize - PLAYER_COLLIDER_SIZE_REDUCTION) * 0.5f;
        _playerHalfWidth = playerSize * 0.5f;

        _powerDisplayContainer.SetActive(true);

        float startedAt = Time.time;
        while (Time.time - startedAt <= SMALL_DURATION_IN_SECONDS)
        {
            int displayTime = Mathf.FloorToInt(SMALL_DURATION_IN_SECONDS - (Time.time - startedAt));
            _powerDisplay.text = $"{displayTime}sec";
            _powerDisplay.color = (displayTime > 4) ? TEXT_YELLOW : TEXT_RED;
            yield return new WaitForFixedUpdate();
        }

        _powerDisplay.text = string.Empty;
        _powerDisplayContainer.SetActive(false);

        playerSize = _wallWidth * PLAYER_SIZE;

        _playerRect.sizeDelta = playerSize * Vector2.one;
        _playerCollider.offset = (playerSize - PLAYER_COLLIDER_SIZE_REDUCTION) * 0.5f * Vector2.one;
        _playerCollider.radius = (playerSize - PLAYER_COLLIDER_SIZE_REDUCTION) * 0.5f;
        _playerHalfWidth = playerSize * 0.5f;

        _playerPowerRoutine = null;
    }
    #endregion Powerups
    public void RemoveWall(WallSegement in_wall)
    {
        _wallPool.Release(in_wall);
    }
    public void ShowMenu()
    {
        // stop overlap of screens if game over is already occuring
        if (state == GameStates.GameOver)
            return;

        Debug.Log("Show main menu");
        state = GameStates.WaitingToStart;
        refs.mainMenu.gameObject.SetActive(true);
    }
    public string ScoreForDisplay
    {
        get
        {
            return Mathf.FloorToInt(score).ToString();
        }
    }

    protected RectTransform _playerRect;
    protected CircleCollider2D _playerCollider;
    protected Image _playerImage;

    protected float _scaleFactorForResolution;
    protected float _playerStartX;
    protected float _playerY;
    protected float _buildYStart;
    protected float _buildY;
    protected float _removeY;
    protected float _ignoreInputAboveY;

    protected IObjectPool<WallSegement> _wallPool;

    protected int[] _segmentQueue;
    protected int _segmentIndex = 0;
    protected Coroutine _playerPowerRoutine;

    public enum GameStates
    {
        WaitingToStart,
        ActiveGame,
        GameOver
    }

    [Header("Debug")]
    public bool debugResolutionCalculations;
    public bool debugAppLogic;
    public bool debugRoadGeneration;
    public bool debugUserInput;

    [Header("Configuration")]
    [SerializeField]
    protected GameObject _wallPrefab;
    [SerializeField]
    protected int numStraightsInQueue = 10;
    [SerializeField]
    protected int numBendsInQueue = 15;
    [SerializeField]
    protected int _numSegmentsUntilPowerUpMin = 17;
    [SerializeField]
    protected int _numSegmentsUntilPowerUpMax = 35;

    [Header("Dynamic")]
    public GameStates state = GameStates.WaitingToStart;
    public float score;
    public bool invincible;
    [Space(20)]
    public int pathConnection = 3;
    public int lastPathDelta;
    public int segmentsUntilPowerUp;
    public List<WallSegement> walls = new List<WallSegement>();
    [Space(20)]
    public float _playerHalfWidth;
    public float _wallWidth;
    public float _wallHeight;
    public float _moveSpeed;

    [Header("References")]
    [SerializeField]
    protected Canvas canvas;
    [SerializeField]
    protected SharedReferences refs;
    [SerializeField]
    protected OverlayScreenBase instructions;
    [SerializeField]
    protected TMP_Text scoreDisplay;
    [SerializeField]
    protected Transform _wallContainer;
    [SerializeField]
    protected PowerPickup _powerPickup;
    [SerializeField]
    protected GameObject _powerDisplayContainer;
    [SerializeField]
    protected TMP_Text _powerDisplay;
}
