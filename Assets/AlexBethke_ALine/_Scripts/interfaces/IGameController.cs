using UnityEngine;

namespace ALine
{
    public interface IGameController
    {
        public void StartGame();
        public void GameOver();
        public void ResetGame();
        public bool IsInputValid();
        public void OnWallCollision(WallSegment in_wall, Collider2D in_other);
        public GameStates state { get; }
        public float score { get; }
        public bool invincible { get; set; }
    }
}