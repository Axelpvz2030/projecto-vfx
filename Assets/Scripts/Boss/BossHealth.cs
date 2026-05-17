using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    
    public float damagePerHit = 10f;

    [Header("State")]
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

        BossAI ai = GetComponent<BossAI>();

        if (currentHealth <= 0)
        {
            Debug.Log("Boss Defeated! Deactivating AI and Health scripts.");
            
            if (ai != null) 
            {
                ai.HandleDeath(); 
                ai.enabled = false;
            }

            this.enabled = false; 
        }
        else
        {
            if (ai != null) ai.InterruptAndForceTeleport();
        }
    }
}