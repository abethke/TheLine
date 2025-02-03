using UnityEngine;

[CreateAssetMenu(fileName = "GameConfiguration", menuName = "Line/Create Game Configuration")]
public class GameConfiguration : ScriptableObject
{
    static public GameConfiguration instance;

    // These values were taken from the original game based on observation
    public int wallRows = 7;
    public int wallColumns = 7;
    public Color playerColour = new Color().FromHex("0088F6");
    public Color blockColour = new Color().FromHex("f71a3f");
    public Color blockHitColour = Color.yellow;
    public Color secondaryYellow = new Color().FromHex("f4bd00");
    public Color textRed = new Color().FromHex("f71a3f");
    public Color textYellow = new Color().FromHex("f4bd00");

    // the initial wall layout
    public string[] wallLayoutAtStart =
        new string[] { "1000001", "1000001", "1100011", "1100011", "1110111", "1110111" };

    // this data represents how far left/right you can move from any given position on the playfield
    public Vector2Int[] pathsByPosition = new Vector2Int[]
    {
        new(0,4),
        new(-1,3),
        new(-2,2),
        new(-2,2),
        new(-3,1),
        new(-4,0)
    };


    // Used by calculations based on wall size 
    public float instructionsOffsetAsPercentOfWallHeight = 1f;
    public float instructionsHeightAsPercentOfWallHeight = 0.8f;
    public float playerSizeAsPercentOfWallHeight = 0.5f;
    public float movementSpeedAsPercentOfWallHeight = 2.5f;

    // timer
    public float fadeDurationInSeconds = 0.3f;
    public float smallDurationInSeconds = 10f;
    public float invincibleDurationInSeconds = 6f;

}
