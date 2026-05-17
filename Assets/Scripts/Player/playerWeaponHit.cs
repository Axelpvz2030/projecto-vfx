using UnityEngine;

public class PlayerWeaponHit : MonoBehaviour
{
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