using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    void OnDestroy()
    {
        _roadBuilder.RoadSegmentSpawned.RemoveListener(OnRoadSpawned);
    }
    public void Init(CalculatedValues in_values, RoadBuilder in_roadBuilder)
    {
        _calculated = in_values;
        _roadBuilder = in_roadBuilder;
        _roadBuilder.RoadSegmentSpawned.AddListener(OnRoadSpawned);

        // resize the power pickup
        _powerPickup.transform.localScale = _calculated.playerSize * 0.5f * Vector3.one;
        _powerPickup.gameObject.SetActive(false);
        // randomize spawn start
        _segmentsUntilPowerUp = Random.Range(_numSegmentsUntilPowerUpMin, _numSegmentsUntilPowerUpMax);
    }
    public void Reset()
    {
        _powerPickup.gameObject.SetActive(false);
        _powerPickup.transform.position = Vector3.one * -100;
        // randomize spawn start
        _segmentsUntilPowerUp = Random.Range(_numSegmentsUntilPowerUpMin, _numSegmentsUntilPowerUpMax);
    }
    protected void Update()
    {
        if (game.state != GameStates.ActiveGame)
            return;

        if (!_powerPickup.gameObject.activeSelf)
            return;

        _powerPickup.transform.position = _powerPickup.transform.position.PlusY(_calculated.moveSpeed * Time.deltaTime);
        if (_powerPickup.transform.position.y < _calculated.removeWallsBelowY)
        {
            _powerPickup.gameObject.SetActive(false);
        }
    }
    protected void OnRoadSpawned()
    {
        // no power spawning while one is active
        if (_player.isPowerActive || _powerPickup.gameObject.activeSelf)
            return;

        _segmentsUntilPowerUp--;
        if (_segmentsUntilPowerUp > 0)
            return;

        _segmentsUntilPowerUp = Random.Range(_numSegmentsUntilPowerUpMin, _numSegmentsUntilPowerUpMax);
        SpawnPowerUpAt(_roadBuilder.pathConnection, _calculated.buildY);
    }
    protected void SpawnPowerUpAt(int in_position, float in_y)
    {
        // broken up for readability
        float wallHalfWidth = _calculated.wallWidth * 0.5f;
        float objectHalfWidth = 0;// _powerPickup.transform.localScale.x * 0.5f;
        float x = _calculated.buildXStart + wallHalfWidth + in_position * _calculated.wallWidth - objectHalfWidth;
        _powerPickup.transform.position = new Vector3(x, in_y);
        _powerPickup.mode = (Random.Range(0, 2) == 0) ? PowerPickup.Modes.Invincible : PowerPickup.Modes.Small;
        _powerPickup.gameObject.SetActive(true);
        Utils.Log($"Spawning powerup[{_powerPickup.mode}] at: {in_position}", GameDebugger.instance.debugRoadGeneration);
    }
    public void ActivatePower()
    {
        if (game.state != GameStates.ActiveGame)
            return;

        Utils.Log("Activating power: " + _powerPickup.mode, GameDebugger.instance.debugAppLogic);
        _powerPickup.gameObject.SetActive(false);

        if (_powerPickup.mode == PowerPickup.Modes.Invincible)
        {
            _player.PowerUpInvincible();
        }
        else
        {
            _player.PowerUpSmall();
        }
    }

    protected CalculatedValues _calculated;
    protected RoadBuilder _roadBuilder;

    public IGameController game;

    [Header("Dynamic")]
    [SerializeField]
    protected int _segmentsUntilPowerUp;

    [Header("Configuration")]
    [SerializeField]
    protected int _numSegmentsUntilPowerUpMin = 17;
    [SerializeField]
    protected int _numSegmentsUntilPowerUpMax = 35;

    [Header("References")]
    [SerializeField]
    protected Player _player;
    [SerializeField]
    protected PowerPickup _powerPickup;
}
