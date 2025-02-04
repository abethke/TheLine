using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    void Start()
    {
        _game = ServiceManager.instance.Get(Services.GAME) as IGameController;
        _display.text = "0";
    }
    void Update()
    {
        _display.text = Utils.ScoreForDisplay(_game.score);
    }

    protected IGameController _game;
    [Header("References")]
    [SerializeField]
    protected TMP_Text _display;
}
