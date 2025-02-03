using System.Collections;
using UnityEngine;
using static PersistentData;

public class GameController : MonoBehaviour, IGameController
{
    #region Life Cycle
    protected void Awake()
    {
        _mainMenu.game = this;
        _gameOver.game = this;
        _powerDisplay.game = this;
        _roadBuilder.game = this;
        _player.game = this;
        _powerUpController.game = this;
        _scoreDisplay.game = this;
    }
    protected void Start()
    {
        Utils.Log("Starting Application", GameDebugger.instance.debugAppLogic);
        GameConfiguration.instance = _config;
        InitScreenState();

        CalculateValuesBasedOnScreenResolution();
        ConfigureElementsBasedOnScreenResolution();

        _roadBuilder.Init(_calculated);
        _roadBuilder.GenerateStartWalls();

        _powerUpController.Init(_calculated, _roadBuilder);
    }
    protected void InitScreenState()
    {
        _mainMenu.gameObject.SetActive(false);
        _gameOver.gameObject.SetActive(false);
        _powerDisplay.gameObject.SetActive(false);
        _instructions.gameObject.SetActive(true);
    }
    protected void CalculateValuesBasedOnScreenResolution()
    {
        Utils.Log($"Screen dimensions: {Screen.width}, {Screen.height}", GameDebugger.instance.debugResolutionCalculations);
        float aspect = (float)Screen.width / (float)Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;
        _calculated.worldWidth = worldWidth;
        _calculated.worldHeight = worldHeight;
        Utils.Log($"World dimensions: {worldWidth}, {worldHeight}", GameDebugger.instance.debugResolutionCalculations);

        _calculated.wallWidth = worldWidth / (float)_config.wallColumns;
        _calculated.wallHeight = worldHeight / (float)_config.wallRows;
        Utils.Log($"Wall Width: {_calculated.wallWidth}", GameDebugger.instance.debugResolutionCalculations);
        Utils.Log($"Wall Height: {_calculated.wallHeight}", GameDebugger.instance.debugResolutionCalculations);

        _calculated.buildXStart = worldWidth * -0.5f;
        _calculated.buildYStart = worldHeight * -0.5f + _calculated.wallHeight * 2.5f;
        _calculated.addWallsAboveY = worldHeight * 0.5f + _calculated.wallHeight * 0.5f;
        _calculated.removeWallsBelowY = worldHeight * -0.5f - _calculated.wallHeight;
        Utils.Log($"Building layout from: {_calculated.buildXStart}, {_calculated.buildYStart}", GameDebugger.instance.debugResolutionCalculations);
        Utils.Log($"Walls removed at: {_calculated.removeWallsBelowY}", GameDebugger.instance.debugResolutionCalculations);

        _calculated.playerSize = _calculated.wallWidth * _config.playerSizeAsPercentOfWallHeight;
        _calculated.playerHalfWidth = _calculated.playerSize * 0.5f;
        _calculated.playerStartX = 0;
        _calculated.playerY = _calculated.buildYStart;
        Utils.Log($"Player size: {_calculated.playerSize}", GameDebugger.instance.debugResolutionCalculations);
        Utils.Log($"Player start position: {_calculated.playerStartX}, {_calculated.buildYStart}", GameDebugger.instance.debugResolutionCalculations);

        _calculated.moveSpeed = _calculated.wallHeight * -_config.movementSpeedAsPercentOfWallHeight;
        Utils.Log($"Move speed: {_calculated.moveSpeed}", GameDebugger.instance.debugResolutionCalculations);

        float instructionsBarHeight = _config.instructionsHeightAsPercentOfWallHeight * _calculated.wallHeight;
        Debug.Log("instructionsBarHeight:" + instructionsBarHeight);
        _calculated.ignoreInputAboveY = worldHeight * -0.5f
            + _config.instructionsOffsetAsPercentOfWallHeight * _calculated.wallHeight
            + instructionsBarHeight;
        _calculated.ignoreInputBelowY = _calculated.ignoreInputAboveY - instructionsBarHeight;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        Utils.Log($"Canvas dimensions: {canvasWidth}, {canvasHeight}", GameDebugger.instance.debugResolutionCalculations);
        _calculated.wallWidthOnCanvas = canvasWidth / (float)_config.wallColumns;
        _calculated.wallHeightOnCanvas = canvasHeight / (float)_config.wallRows;
    }
    protected void ConfigureElementsBasedOnScreenResolution()
    {
        // initialize player
        _player.Init(_calculated);
        _player.transform.position = new Vector2(_calculated.playerStartX, _calculated.playerY);

        float instructionsHeight = _config.instructionsHeightAsPercentOfWallHeight * _calculated.wallHeightOnCanvas;
        float instructionsY = _config.instructionsOffsetAsPercentOfWallHeight * _calculated.wallHeightOnCanvas;
        RectTransform instructionsRect = _instructions.gameObject.RectTransform();
        instructionsRect.sizeDelta = instructionsRect.sizeDelta.SetY(instructionsHeight);
        instructionsRect.anchoredPosition = new Vector2(0, instructionsY);

        RectTransform powerDisplayRect = _powerDisplay.gameObject.RectTransform();
        powerDisplayRect.sizeDelta = powerDisplayRect.sizeDelta.SetY(instructionsHeight * 0.5f);
        powerDisplayRect.anchoredPosition = new Vector2(0, instructionsY + instructionsHeight);
    }
    #endregion Life Cycle
    #region Game Loop
    void Update()
    {
        switch (_state)
        {
            case GameStates.WaitingToStart:
                if (Input.GetMouseButtonDown(0) && IsInputValid())
                {
                    StartGame();
                }
                break;
            case GameStates.ActiveGame:
                _score += Time.deltaTime * 10f;
                break;
            case GameStates.GameOver:
                // do nothing
                break;
        }
    }
    public bool IsInputValid()
    {
        Utils.Log($"screen mouse position: {Input.mousePosition}", GameDebugger.instance.debugUserInput);
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Utils.Log($"worldMouse: {worldMouse}", GameDebugger.instance.debugUserInput);
        return (worldMouse.y < _calculated.ignoreInputAboveY && worldMouse.y > _calculated.ignoreInputBelowY);
    }
    public void StartGame()
    {
        Utils.Log("Start game", GameDebugger.instance.debugAppLogic);
        _instructions.FadeOut();
        _state = GameStates.ActiveGame;
    }
    public void GameOver()
    {
        Utils.Log("GAME OVER", GameDebugger.instance.debugAppLogic);
        _state = GameStates.GameOver;

        SaveHighScore();

        StartCoroutine(GameOverRoutine());
    }
    protected void SaveHighScore()
    {
        int finalScore = Mathf.FloorToInt(_score);
        int bestScore = PlayerPrefs.HasKey(BEST_SCORE) ? PlayerPrefs.GetInt(BEST_SCORE) : 0;
        Utils.Log($"final score {finalScore} vs best score {bestScore}", GameDebugger.instance.debugAppLogic);
        if (finalScore > bestScore)
        {
            PlayerPrefs.SetInt(BEST_SCORE, finalScore);
        }
    }
    protected IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(3f);

        _gameOver.gameObject.SetActive(true);
        _instructions.FadeIn();
    }
    public void ResetGame()
    {
        Utils.Log("Resetting game", GameDebugger.instance.debugAppLogic);
        _score = 0;

        _player.Reset();
        _player.transform.position = new Vector2(_calculated.playerStartX, _calculated.playerY);

        _powerUpController.Reset();
        _powerDisplay.gameObject.SetActive(false);

        _roadBuilder.Reset();
        _roadBuilder.GenerateStartWalls();

        _state = GameStates.WaitingToStart;
    }
    #endregion Game Loop
    public void ShowMenu()
    {
        // stop overlap of screens if game over is already occuring
        if (_state == GameStates.GameOver)
            return;

        Utils.Log("Show main menu", GameDebugger.instance.debugAppLogic || GameDebugger.instance.debugUserInput);
        _state = GameStates.WaitingToStart;
        _mainMenu.gameObject.SetActive(true);
    }
    public float score
    {
        get { return _score; }
    }
    public GameStates state
    {
        get { return _state; }
    }
    public bool invincible
    {
        set { _invincible = value; }
        get { return _invincible; }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(_calculated.buildXStart, _calculated.ignoreInputBelowY),
            new Vector2(_calculated.buildXStart + _calculated.worldWidth, _calculated.ignoreInputBelowY));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(_calculated.buildXStart, _calculated.ignoreInputAboveY),
            new Vector2(_calculated.buildXStart + _calculated.worldWidth, _calculated.ignoreInputAboveY));
    }

    protected CalculatedValues _calculated = new CalculatedValues();
    protected float _score;

    [Header("Configuration")]
    [SerializeField]
    protected GameConfiguration _config;

    [Header("Dynamic - Game State")]
    [SerializeField]
    protected GameStates _state = GameStates.WaitingToStart;
    [SerializeField]
    protected bool _invincible;

    [Header("References")]
    [SerializeField]
    protected MainMenu _mainMenu;
    [SerializeField]
    protected GameOver _gameOver;
    [SerializeField]
    protected Player _player;
    [SerializeField]
    protected RoadBuilder _roadBuilder;
    [SerializeField]
    protected PowerUpController _powerUpController;
    [SerializeField]
    protected Canvas _canvas;
    [SerializeField]
    protected OverlayScreenBase _instructions;
    [SerializeField]
    protected PowerInstructions _powerDisplay;
    [SerializeField]
    protected ScoreDisplay _scoreDisplay;
}
