using UnityEngine;

public class PowerPickup : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D in_collider)
    {
        if (!in_collider.gameObject == _player.gameObject)
            return;

        powerUpController.ActivatePower();
    }
    public enum Modes
    {
        Invincible,
        Small
    }

    [Header("Dynamic")]
    public Modes mode = Modes.Invincible;

    [Header("References")]
    public PowerUpController powerUpController;
    [SerializeField]
    protected Player _player;
}
