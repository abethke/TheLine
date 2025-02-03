using UnityEngine;
using TMPro;
using static PersistentData;
public class GameOver : OverlayScreenBase
{
    void OnEnable()
    {
        _scoreDisplay.text = Utils.ScoreForDisplay(game.score);
        int bestScore = PlayerPrefs.HasKey(BEST_SCORE) ? PlayerPrefs.GetInt(BEST_SCORE) : 0;
        _bestScoreDisplay.text = bestScore.ToString();
        FadeIn();
    }
    public void ClickedTryAgain()
    {
        game.ResetGame();
        gameObject.SetActive(false);
    }

    public IGameController game;

    [Header("References")]    
    [SerializeField]
    protected TMP_Text _scoreDisplay;
    [SerializeField]
    protected TMP_Text _bestScoreDisplay;
}
