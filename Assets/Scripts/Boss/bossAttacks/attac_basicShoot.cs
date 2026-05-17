using System.Collections;
using UnityEngine;

public class Attack_BasicShoot : BossAttack
{
    [Header("Attack Settings")]
    public float startDelay = 1.0f; 
    public ProjectileSpawner spawner;
    public int projectilesToFire = 5;
    public float timeBetweenShots = 5f;

    [Header("Animation Settings")]
    public Animator animator;

    private bool isCancelled = false;

    public override IEnumerator ExecuteAttack(BossAI bossAI)
    {
        isCancelled = false;
        
        if (animator != null)
        {
            animator.SetBool("isShooting", true);
        }

        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < projectilesToFire; i++)
        {
            if (isCancelled) yield break; 

            if (spawner != null)
            {
                spawner.SetSpawnerActive(true);
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }

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

        if (animator != null)
        {
            animator.SetBool("isShooting", false);
        }
    }
}