using UnityEngine;
using TMPro;
using static SavedValues;
public class GameOver : OverlayScreenBase
{
    void Start() { }
    void OnEnable()
    {
        _scoreDisplay.text = refs.game.ScoreForDisplay;
        int bestScore = PlayerPrefs.HasKey(SAVED_BEST_SCORE) ? PlayerPrefs.GetInt(SAVED_BEST_SCORE) : 0;
        _bestScoreDisplay.text = bestScore.ToString();
        FadeIn();
    }
    public void ClickedTryAgain()
    {
        refs.game.ResetGame();
        refs.game.StartGame();
        gameObject.SetActive(false);
    }

    [Header("References")]
    public SharedReferences refs;
    [SerializeField]
    protected TMP_Text _scoreDisplay;
    [SerializeField]
    protected TMP_Text _bestScoreDisplay;
}
