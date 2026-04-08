using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 10f;
    public float flightTime = 2f;
    public float lifetimeAfterFlight = 0f;

    private float timer = 0f;
    private bool isFlying = true;

    private void Update()
    {
        timer += Time.deltaTime;

        if (isFlying)
        {
            // Moves the projectile strictly forward based on its local rotation
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (timer >= flightTime)
            {
                isFlying = false;
                timer = 0f; // Reset timer for the stationary phase
                
                if (lifetimeAfterFlight <= 0f) 
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (timer >= lifetimeAfterFlight)
            {
                Destroy(gameObject);
            }
        }
    }
}