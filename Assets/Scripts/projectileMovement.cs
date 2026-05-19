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
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (timer >= flightTime)
            {
                isFlying = false;
                timer = 0f; 
                
                if (lifetimeAfterFlight <= 0f) 
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (timer >= lifetimeAfterFlight)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Shoot()
    {
        timer = 0f;
        isFlying = true;
    }
}