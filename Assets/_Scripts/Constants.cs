using UnityEngine;

static public class Constants
{
    // These values were taken from the original game based on observation
    public const int WALL_ROWS = 7;
    public const int WALL_COLS = 7;

    // the initial wall layout
    public static readonly string[] LAYOUT_AT_START =
        new string[] { "1000001", "1000001", "1100011", "1100011", "1110111", "1110111" };

    public static readonly Vector2Int[] PATHS_BY_POSITION = new Vector2Int[]
    {
        new(0,5),
        new(-1,4),
        new(-2,3),
        new(-3,2),
        new(-4,1),
        new(-5,0)
    };

    public const float MOVEMENT_SPEED = 2.5f;

    // Used by calculations based on wall size 
    public const float INSTRUCTIONS_OFFSET = 1;
    public const float INSTRUCTIONS_HEIGHT = 0.8f;
    public const float PLAYER_SIZE = 0.5f;

    // timer
    public const float FADE_DURATION_IN_SECONDS = 0.3f;
}