using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using static SavedValues;
using static Constants;

public class GameController : MonoBehaviour
{
    #region Life Cycle
    void Start()
    {
        Utils.Log("Starting Application", debugAppLogic);
        refs.mainMenu.gameObject.SetActive(false);
        refs.gameOver.gameObject.SetActive(false);
        instructions.gameObject.SetActive(true);

        // get references
        _playerRect = refs.player.GetComponent<RectTransform>();

        CalculateValuesBasedOnScreenResolution();
        ConfigureElementsBasedOnScreenResolution();

        InitObjectPool();
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

        _playerRect.sizeDelta = playerSize * Vector2.one;
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

        CircleCollider2D playerCollider = _playerRect.GetComponent<CircleCollider2D>();
        playerCollider.offset = playerSize * 0.5f * Vector2.one;
        playerCollider.radius = playerSize * 0.5f;
    }
    protected void ConfigureElementsBasedOnScreenResolution()
    {
        // position player at start
        _playerRect.anchoredPosition = new Vector2(_playerStartX, _playerY);

        RectTransform instructionsRect = instructions.gameObject.RectTransform();
        instructionsRect.sizeDelta = instructionsRect.sizeDelta.SetY(INSTRUCTIONS_HEIGHT * _wallHeight);
        instructionsRect.anchoredPosition = new Vector2(0, INSTRUCTIONS_OFFSET * _wallHeight);
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
        // the one we're moving to plus the build row, as the absolute maximum needed
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

                }
                score += Time.deltaTime * 10f;
                scoreDisplay.text = $"{ScoreForDisplay}";
                break;
            case GameStates.GameOver:
                // do nothing
                break;
        }
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

        _playerRect.anchoredPosition = new Vector2(_playerStartX, _playerY);
        pathConnection = 3;
        GenerateStartWalls();
    }
    #endregion Game Loop
    #region Road Generation
    protected void UpdateForNewLineCreation()
    {
        if (walls.Count == 0)
            return;

        WallSegement lastWall = walls[walls.Count - 1];
        if (lastWall.anchoredPosition.y > 0)
            return;

        _buildY = lastWall.anchoredPosition.y + _wallHeight;
        GenerateRandomLine();
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
    }
    protected void GenerateRandomLine()
    {
        if (_forceStraightSpawn)
        {
            _forceStraightSpawn = false;
            // build a straight path from the current connection
            for (int i = 0; i < 7; i++)
            {
                if (i == pathConnection)
                    continue;

                WallSegement wall = _wallPool.Get();
                wall.anchoredPosition = new Vector2(_wallWidth * 0.5f + i * _wallWidth, _buildY);
            }
        }
        else
        {
            Debug.Log($"Connecting from: " + pathConnection);
            Vector2Int pathways = PATHS_BY_POSITION[pathConnection - 1];
            Debug.Log($"segment paths: {pathways}");
            int offset = Random.Range(pathways.x, pathways.y);
            Debug.Log($"offset: {offset}");

            int start = pathConnection;
            int end = pathConnection + offset;

            // generate the segment
            for (int i = 0; i < 7; i++)
            {
                if (i < Mathf.Min(start, end) || i > Mathf.Max(start, end))
                {
                    WallSegement wall = _wallPool.Get();
                    wall.anchoredPosition = new Vector2(_wallWidth * 0.5f + i * _wallWidth, _buildY);
                }
            }

            if (pathConnection != end)
            {
                Debug.Log("random spawn straight");
                _forceStraightSpawn = true;
            }

            pathConnection = end;
        }
    }
    #endregion Road Generation
    public void ShowMenu()
    {
        // stop overlap of screens if game over is already occuring
        if (state == GameStates.GameOver)
            return;

        Debug.Log("Show main menu");
        state = GameStates.WaitingToStart;
        refs.mainMenu.gameObject.SetActive(true);
    }
    public void RemoveWall(WallSegement in_wall)
    {
        _wallPool.Release(in_wall);
    }
    public string ScoreForDisplay
    {
        get
        {
            return Mathf.FloorToInt(score).ToString();
        }
    }

    protected RectTransform _playerRect;
    protected float _scaleFactorForResolution;
    protected float _playerStartX;
    protected float _playerY;
    protected float _buildYStart;
    protected float _buildY;
    protected float _removeY;
    protected float _ignoreInputAboveY;

    protected IObjectPool<WallSegement> _wallPool;

     protected bool _forceStraightSpawn;
    
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
    public bool debugUserInput;

    [Header("Configuration")]
    public GameObject _wallPrefab;

    [Header("Dynamic")]
    public GameStates state = GameStates.WaitingToStart;
    public float score;
    public int segmentsUntilPowerUp = 10;
    public bool invincible;
    public int pathConnection = 3;
    public List<WallSegement> walls = new List<WallSegement>();
    [Space(20)]
    public float _playerHalfWidth;
    public float _wallWidth;
    public float _wallHeight;
    public float _moveSpeed;

    [Header("References")]
    public Canvas canvas;
    [SerializeField]
    protected SharedReferences refs;
    [SerializeField]
    protected OverlayScreenBase instructions;
    [SerializeField]
    protected TMP_Text scoreDisplay;
    [SerializeField]
    protected Transform _wallContainer;
}
