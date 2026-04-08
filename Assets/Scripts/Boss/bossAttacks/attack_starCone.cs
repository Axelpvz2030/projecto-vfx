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

    private bool isCancelled = false;
    private Quaternion originalLocalRotation;

    public override IEnumerator ExecuteAttack(BossAI bossAI)
    {
        isCancelled = false;
        
        // Save the spawner's default rotation so we don't permanently break its aim
        if (spawner != null) 
        {
            originalLocalRotation = spawner.transform.localRotation;
        }

        // Freeze the boss's rotation so the cone stays focused on where the player WAS 
        // when the attack started, rather than perfectly tracking them the whole time.
        bossAI.lookAtPlayer = false; 

        // 1. Initial wind-up delay
        yield return new WaitForSeconds(startDelay);

        // 2. Fire the projectiles
        for (int i = 0; i < projectilesToFire; i++)
        {
            if (isCancelled) yield break;

            if (spawner != null)
            {
                // Calculate a random angle between -spreadAngle and +spreadAngle
                float randomOffset = Random.Range(-spreadAngle, spreadAngle);

                // Apply rotation on the Y axis (left/right) relative to its original rotation
                spawner.transform.localRotation = originalLocalRotation * Quaternion.Euler(0f, randomOffset, 0f);

                // Fire
                spawner.SetSpawnerActive(true);
            }

            // Wait for next shot
            yield return new WaitForSeconds(timeBetweenShots);
        }

        // 3. Reset the spawner's rotation and allow the boss to track the player again
        if (spawner != null)
        {
            spawner.transform.localRotation = originalLocalRotation;
        }
        bossAI.lookAtPlayer = true;
    }

    public override void CancelAttack()
    {
        isCancelled = true;
        
        if (spawner != null)
        {
            spawner.SetSpawnerActive(false);
            
            // Crucial: Reset the rotation here too, in case the boss gets hit 
            // and interrupted right in the middle of a rotated shot!
            spawner.transform.localRotation = originalLocalRotation; 
        }
    }
}