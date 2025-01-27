using UnityEngine;

static public class Constants
{
    // These values were taken from the original game based on observation
    public const int WALL_ROWS = 7;
    public const int WALL_COLS = 7;
    public static readonly Color PLAYER_COLOUR = new Color().FromHex("0088F6");
    public static readonly Color BLOCK_COLOUR = new Color().FromHex("f71a3f");
    public static readonly Color BLOCK_HIT_COLOUR = Color.yellow;
    public static readonly Color SECONDARY_YELLOW = new Color().FromHex("f4bd00");
    public static readonly Color TEXT_RED = new Color().FromHex("f71a3f");
    public static readonly Color TEXT_YELLOW = new Color().FromHex("f4bd00");

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

    // magic number for  player collider
    public const float PLAYER_COLLIDER_SIZE_REDUCTION = 1f;

    // timer
    public const float FADE_DURATION_IN_SECONDS = 0.3f;
    public const float SMALL_DURATION_IN_SECONDS = 10f;
    public const float INVINCIBLE_DURATION_IN_SECONDS = 6f;

    public const string PLAYER_INSTANCE_NAME = "Player";
}