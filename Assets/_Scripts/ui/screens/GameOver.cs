using UnityEngine;
using TMPro;
using static PersistentData;
public class GameOver : OverlayScreenBase
{
    void OnEnable()
    {
        _game ??= ServiceManager.instance.Get(Services.GAME) as IGameController;

        _scoreDisplay.text = Utils.ScoreForDisplay(_game.score);
        int bestScore = PlayerPrefs.HasKey(BEST_SCORE) ? PlayerPrefs.GetInt(BEST_SCORE) : 0;
        _bestScoreDisplay.text = bestScore.ToString();
        FadeIn();
    }
    public void ClickedTryAgain()
    {
        _game.ResetGame();
        gameObject.SetActive(false);
    }

    protected IGameController _game;

    [Header("References")]
    [SerializeField]
    protected TMP_Text _scoreDisplay;
    [SerializeField]
    protected TMP_Text _bestScoreDisplay;
}
