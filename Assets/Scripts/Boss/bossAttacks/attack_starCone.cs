using System.Collections;
using UnityEngine;

public class Attack_StarCone : BossAttack
{
    [Header("References")]
    public ProjectileSpawner spawner;

    [Header("Attack Settings")]
    [Tooltip("Time to wait before firing the first projectile.")]
    public float startDelay = 1.0f;
    public int projectilesToFire = 10;
    public float timeBetweenShots = 0.2f;
    
    [Tooltip("Maximum angle (in degrees) the projectile can deviate left or right.")]
    public float spreadAngle = 30f;

    [Header("Animation Settings")]
    public Animator animator;

    private bool isCancelled = false;
    private Quaternion originalLocalRotation;

    public override IEnumerator ExecuteAttack(BossAI bossAI)
    {
        isCancelled = false;
        
        // Save the spawner's default rotation
        if (spawner != null) 
        {
            originalLocalRotation = spawner.transform.localRotation;
        }

        bossAI.lookAtPlayer = false; 

        // 1. Start the animation right at the beginning of the attack
        if (animator != null)
        {
            animator.SetBool("isShooting", true);
        }

        // Wind-up delay
        yield return new WaitForSeconds(startDelay);

        // Fire the projectiles
        for (int i = 0; i < projectilesToFire; i++)
        {
            if (isCancelled) yield break;

            if (spawner != null)
            {
                float randomOffset = Random.Range(-spreadAngle, spreadAngle);
                spawner.transform.localRotation = originalLocalRotation * Quaternion.Euler(0f, randomOffset, 0f);
                spawner.SetSpawnerActive(true);
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }

        // 2. Cleanup: Reset rotation, tracking, and turn off the animation
        if (spawner != null)
        {
            spawner.transform.localRotation = originalLocalRotation;
        }
        bossAI.lookAtPlayer = true;

        if (animator != null)
        {
            animator.SetBool("isShooting", false);
        }
    }

    public override void CancelAttack()
    {
        isCancelled = true;
        
        if (spawner != null)
        {
            spawner.SetSpawnerActive(false);
            spawner.transform.localRotation = originalLocalRotation; 
        }

        // 3. Stop the animation if interrupted
        if (animator != null)
        {
            animator.SetBool("isShooting", false);
        }
    }
}