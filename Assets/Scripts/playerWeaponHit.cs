using UnityEngine;

public class PlayerWeaponHit : MonoBehaviour
{
    // Using OnTriggerEnter is usually better for melee strikes so it only registers once per swing
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            
            if (bossHealth != null)
            {
                bossHealth.TakeDamage();
            }
        }
    }
}