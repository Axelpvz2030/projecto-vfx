using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Drag the child GameObject with the trigger collider here.")]
    public GameObject attackHurtbox; 
    public float attackDuration = 0.5f;

    // Made public so PlayerShield.cs can read it
    public bool isAttacking = false; 
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        
        if (attackHurtbox != null)
        {
            attackHurtbox.SetActive(false);
        }
    }

    private void Update()
    {
        // Ensure you cannot attack while shielding
        bool isShielding = (playerMovement != null && playerMovement.isShielding);

        if (!isAttacking && !isShielding && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        
        if (playerMovement != null) playerMovement.canMove = false;
        
        if (attackHurtbox != null) attackHurtbox.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        if (attackHurtbox != null) attackHurtbox.SetActive(false);
        
        if (playerMovement != null) playerMovement.canMove = true;
        
        isAttacking = false;
    }
}