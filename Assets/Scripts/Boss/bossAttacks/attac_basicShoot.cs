using System.Collections;
using UnityEngine;

public class Attack_BasicShoot : BossAttack
{
    [Header("Attack Settings")]
    public ProjectileSpawner spawner;
    public int projectilesToFire = 5;
    [Tooltip("Time between each projectile (you asked for 5s, but I made it a variable so you can tweak it to 0.5s if 5s is too slow!).")]
    public float timeBetweenShots = 5f;

    private bool isCancelled = false;

    public override IEnumerator ExecuteAttack(BossAI bossAI)
    {
        isCancelled = false;
        
        // Example: Maybe this attack locks the boss's rotation while firing
        //bossAI.lookAtPlayer = false; 

        for (int i = 0; i < projectilesToFire; i++)
        {
            if (isCancelled) yield break; // Stop immediately if we got hit

            // Force the spawner to fire instantly since it's an instant spawn bypass in your spawner script
            if (spawner != null)
            {
                spawner.SetSpawnerActive(true);
            }

            // Wait before the next shot
            yield return new WaitForSeconds(timeBetweenShots);
        }

        //bossAI.lookAtPlayer = true;
    }

    public override void CancelAttack()
    {
        isCancelled = true;
        
        // Ensure the spawner is turned off if interrupted mid-charge
        if (spawner != null)
        {
            spawner.SetSpawnerActive(false);
        }
    }
}