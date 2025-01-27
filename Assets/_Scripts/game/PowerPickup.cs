using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPickup : MonoBehaviour
{
    //void Start() { }
    protected void OnTriggerEnter2D(Collider2D in_collider)
    {
        Debug.Log("hit: " + in_collider);
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
    [SerializeField]
    protected SharedReferences refs;
    public RectTransform rectTransform;
    public new BoxCollider2D collider;
}
