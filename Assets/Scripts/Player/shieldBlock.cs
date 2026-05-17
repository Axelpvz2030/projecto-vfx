using UnityEngine;

public class ShieldBlock : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 15f;
    public float knockbackTime = 0.15f;

    private PlayerMovement playerMovement;

    private void Start()
    {
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
                Vector3 pushDirection = (playerMovement.transform.position - other.transform.position).normalized;
                
                pushDirection.y = 0f; 
                pushDirection.Normalize(); 

                playerMovement.ApplyKnockback(pushDirection, knockbackForce, knockbackTime);
            }

            Destroy(other.gameObject);
        }
    }
}