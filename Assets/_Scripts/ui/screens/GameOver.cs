using UnityEngine;
using TMPro;
using static SavedValues;
public class GameOver : OverlayScreenBase
{
    void Start() { }
    void OnEnable()
    {
        scoreDisplay.text = refs.game.ScoreForDisplay;
        int bestScore = PlayerPrefs.HasKey(SAVED_BEST_SCORE) ? PlayerPrefs.GetInt(SAVED_BEST_SCORE) : 0;
        bestScoreDisplay.text = bestScore.ToString();
        FadeIn();
    }
    public void ClickedTryAgain()
    {
        refs.game.ResetGame();
        refs.game.StartGame();
        gameObject.SetActive(false);
    }

    [Header("References")]
    [SerializeField]
    private SharedReferences refs;
    [SerializeField]
    private TMP_Text scoreDisplay;
    [SerializeField]
    private TMP_Text bestScoreDisplay;
}
