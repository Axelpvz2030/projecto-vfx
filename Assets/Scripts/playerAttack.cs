using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Drag the child GameObject with the trigger collider here.")]
    public GameObject attackHurtbox; 
    public float attackDuration = 0.5f;

    private bool isAttacking = false;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        
        // Ensure the hurtbox is off by default
        if (attackHurtbox != null)
        {
            attackHurtbox.SetActive(false);
        }
    }

    private void Update()
    {
        // Listen for Left Mouse Button
        if (!isAttacking && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        
        // Stop movement input, but keep the script running for gravity
        if (playerMovement != null) playerMovement.canMove = false;
        
        // Activate the hurtbox child object
        if (attackHurtbox != null) attackHurtbox.SetActive(true);

        // Wait for the duration of the attack
        yield return new WaitForSeconds(attackDuration);

        // Deactivate the hurtbox
        if (attackHurtbox != null) attackHurtbox.SetActive(false);
        
        // Allow movement input again
        if (playerMovement != null) playerMovement.canMove = true;
        
        isAttacking = false;
    }
}