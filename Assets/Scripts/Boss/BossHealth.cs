using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    
    [Tooltip("The set amount of damage the boss takes from any player attack.")]
    public float damagePerHit = 10f;

    [Header("State")]
    [Tooltip("Controlled by AI later to determine if the boss can take damage.")]
    public bool canBeHarmed = true;

    private void Start()
    {
        currentHealth = maxHealth;
        canBeHarmed = true;
    }

    public void TakeDamage()
    {
        if (!canBeHarmed || currentHealth <= 0) return;

        currentHealth -= damagePerHit;
        canBeHarmed = false; 
        
        Debug.Log($"Boss hit! Took {damagePerHit} damage. Current HP: {currentHealth}");

        // --- NEW CODE HERE ---
        BossAI ai = GetComponent<BossAI>();
        if (ai != null) ai.InterruptAndForceTeleport();
        // ---------------------

        if (currentHealth <= 0)
        {
            Debug.Log("Boss Defeated!");
        }
    }
}