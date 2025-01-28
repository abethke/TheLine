using UnityEngine;

public class PowerPickup : MonoBehaviour
{
    void Start() { }
    protected void OnTriggerEnter2D(Collider2D in_collider)
    {
        if (in_collider.name != Constants.PLAYER_INSTANCE_NAME)
            return;

        refs.game.ActivatePower();
    }
    public enum Modes
    {
        Invincible,
        Small
    }

    [Header("Dynamic")]
    public Modes mode = Modes.Invincible;

    [Header("References")]
    public SharedReferences refs;
    public RectTransform rectTransform;
    public new BoxCollider2D collider;
}
