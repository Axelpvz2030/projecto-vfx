using UnityEngine;

public class ShieldBlock : MonoBehaviour
{
    [Header("Knockback Settings")]
    [Tooltip("How fast the player is pushed back.")]
    public float knockbackForce = 15f;
    [Tooltip("How long the push lasts in seconds.")]
    public float knockbackTime = 0.15f;

    private PlayerMovement playerMovement;

    private void Start()
    {
        // The shield is a child of the player, so we grab the movement script from the parent
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        
        if (hurtbox != null && hurtbox.destroyOnContact)
        {
            Debug.Log("Shield blocked a projectile!");

            if (playerMovement != null)
            {
                // Calculate the push direction: directly away from the projectile
                Vector3 pushDirection = (playerMovement.transform.position - other.transform.position).normalized;
                
                // Force the Y to 0 so the player doesn't get pushed into the floor or launched into the air
                pushDirection.y = 0f; 
                pushDirection.Normalize(); 

                // Send the knockback command to the player
                playerMovement.ApplyKnockback(pushDirection, knockbackForce, knockbackTime);
            }

            Destroy(other.gameObject);
        }
    }
}