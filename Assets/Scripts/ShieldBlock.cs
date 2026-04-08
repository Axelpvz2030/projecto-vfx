using UnityEngine;

public class ShieldBlock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hitting the shield is a Hurtbox
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        
        // If it is a Hurtbox AND it is set to destroy on contact (like a projectile)
        if (hurtbox != null && hurtbox.destroyOnContact)
        {
            Debug.Log("Shield blocked a projectile!");
            Destroy(other.gameObject);
        }
    }
}