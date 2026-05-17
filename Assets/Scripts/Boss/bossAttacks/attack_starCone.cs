using System.Collections;
using UnityEngine;

public class Attack_StarCone : BossAttack
{
    [Header("References")]
    public ProjectileSpawner spawner;

    [Header("Attack Settings")]
    public float startDelay = 1.0f;
    public int projectilesToFire = 10;
    public float timeBetweenShots = 0.2f;
    public float spreadAngle = 30f;

    [Header("Animation Settings")]
    public Animator animator;

    private bool isCancelled = false;
    private Quaternion originalLocalRotation;

    public override IEnumerator ExecuteAttack(BossAI bossAI)
    {
        isCancelled = false;
        
        if (spawner != null) 
        {
            originalLocalRotation = spawner.transform.localRotation;
        }

        bossAI.lookAtPlayer = false; 

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
                float randomOffset = Random.Range(-spreadAngle, spreadAngle);
                spawner.transform.localRotation = originalLocalRotation * Quaternion.Euler(0f, randomOffset, 0f);
                spawner.SetSpawnerActive(true);
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }

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

        if (animator != null)
        {
            animator.SetBool("isShooting", false);
        }
    }
}