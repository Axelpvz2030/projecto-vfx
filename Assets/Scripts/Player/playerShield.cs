using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShield : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("Drag your Shield child GameObject here.")]
    public GameObject shieldHitbox; 

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        // Make sure it starts turned off
        if (shieldHitbox != null)
        {
            shieldHitbox.SetActive(false);
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        bool isRightClickHeld = Mouse.current.rightButton.isPressed;
        
        // You cannot shield if you are currently dashing or currently attacking
        bool canUseShield = !playerMovement.isDashing && (playerAttack == null || !playerAttack.isAttacking);

        if (isRightClickHeld && canUseShield)
        {
            ActivateShield();
        }
        else
        {
            DeactivateShield();
        }
    }

    private void ActivateShield()
    {
        if (shieldHitbox != null) shieldHitbox.SetActive(true);
        if (playerMovement != null) playerMovement.isShielding = true;
    }

    private void DeactivateShield()
    {
        if (shieldHitbox != null) shieldHitbox.SetActive(false);
        if (playerMovement != null) playerMovement.isShielding = false;
    }
}