using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [Header("Hurtbox Settings")]
    public bool isActive = true;
    public float damage = 10f;
    
    [Tooltip("Should the object be destroyed upon hitting the player? (Ideal for projectiles)")]
    public bool destroyOnContact = false;

    // OnTriggerStay is better than OnTriggerEnter here. 
    // If a player stands in poison, gets hit, loses their i-frames, 
    // and is STILL standing in the poison, they will take damage again.
    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;

        // Ensure your Player GameObject has the tag "Player" in the Inspector!
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                // Attempt to deal damage. TakeDamage returns true if it worked, false if the player was invincible.
                bool damageDealt = playerHealth.TakeDamage(damage);

                // If damage was successfully dealt and this is a projectile, destroy it
                if (damageDealt && destroyOnContact)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}