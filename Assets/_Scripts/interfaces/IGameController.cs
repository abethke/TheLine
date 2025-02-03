using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameController
{
    public void StartGame();
    public void GameOver();
    public void ResetGame();
    public bool IsInputValid();
    public GameStates state { get; }
    public float score { get; }
    public bool invincible { get; set; }
}
