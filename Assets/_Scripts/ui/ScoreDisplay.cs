using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    void Start()
    {
        _display.text = "0";
    }
    void Update()
    {
        if (game.state == GameStates.WaitingToStart)
            return;

        _display.text = Utils.ScoreForDisplay(game.score);
    }

    public IGameController game;
    [Header("References")]
    [SerializeField]
    protected TMP_Text _display;
}
