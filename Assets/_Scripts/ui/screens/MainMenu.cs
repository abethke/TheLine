using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : OverlayScreenBase
{
    void Start() { }
    public void OnEnable()
    {
        FadeIn();
    }
    public void ClickedKeepGoing()
    {
        gameObject.SetActive(false);
        refs.game.StartGame();
    }
    public void ClickedTryAgain()
    {
        refs.game.ResetGame();
        gameObject.SetActive(false);
    }

    [Header("References")]
    [SerializeField]
    private SharedReferences refs;
}
