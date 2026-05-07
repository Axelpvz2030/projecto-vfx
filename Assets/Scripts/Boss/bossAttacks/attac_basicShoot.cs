using System.Collections;
using UnityEngine;

public class Attack_BasicShoot : BossAttack
{
    [Header("Attack Settings")]
    [Tooltip("Time to wait before firing the first projectile.")]
    public float startDelay = 1.0f; // <--- NEW VARIABLE
    public ProjectileSpawner spawner;
    public int projectilesToFire = 5;
    [Tooltip("Time between each projectile.")]
    public float timeBetweenShots = 5f;

    [Header("Animation Settings")]
    public Animator animator;

    private bool isCancelled = false;

    public override IEnumerator ExecuteAttack(BossAI bossAI)
    {
        isCancelled = false;
        
        // 1. Start the animation so the boss strikes a pose
        if (animator != null)
        {
            animator.SetBool("isShooting", true);
        }

        // 2. Wind-up delay before the first shot
        yield return new WaitForSeconds(startDelay);

        // 3. Fire the projectiles
        for (int i = 0; i < projectilesToFire; i++)
        {
            if (isCancelled) yield break; // Stop immediately if we got hit

            if (spawner != null)
            {
                spawner.SetSpawnerActive(true);
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }

        // 4. Stop the animation when finished
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
        }

        // Stop the animation if the boss gets interrupted!
        if (animator != null)
        {
            animator.SetBool("isShooting", false);
        }
    }
}