using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [Header("Hurtbox Settings")]
    public bool isActive = true;
    public float damage = 10f;
    
    public bool destroyOnContact = false; 

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
  
                playerHealth.TakeDamage(damage);

                if (destroyOnContact)
                {
                    gameObject.SetActive(false); 
                }
            }
        }
    }
}