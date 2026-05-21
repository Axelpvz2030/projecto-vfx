using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_SwordAttack1 : BossAttack
{
    public List<ProjectileSpawner> spawners;
    public int numberOfSpawns = 4;
    public float timeBetweenSpawns = 1.5f;
    public Animator bossAnimator;

    private bool isAttacking;
    private int currentAttackID = 0;

    public override IEnumerator ExecuteAttack(BossAI bossAI)
    {
        isAttacking = true;
        currentAttackID++;
        int myAttackID = currentAttackID; 

        if (bossAnimator == null)
        {
            bossAnimator = bossAI.GetComponent<Animator>();
        }

        for (int i = 0; i < numberOfSpawns; i++)
        {
            
            if (!isAttacking || myAttackID != currentAttackID) break;

            List<ProjectileSpawner> availableSpawners = new List<ProjectileSpawner>();
            foreach (ProjectileSpawner spawner in spawners)
            {
                if (spawner != null && !spawner.isActive)
                {
                    availableSpawners.Add(spawner);
                }
            }

            if (availableSpawners.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSpawners.Count);
                availableSpawners[randomIndex].SetSpawnerActive(true);

                if (bossAnimator != null)
                {
                    bossAnimator.SetTrigger("swingArm");
                }
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (myAttackID == currentAttackID)
        {
            isAttacking = false;
        }
    }

    public override void CancelAttack()
    {
        isAttacking = false;
        currentAttackID++; 
        
        foreach (ProjectileSpawner spawner in spawners)
        {
            if (spawner != null)
            {
                
                spawner.SetSpawnerActive(false);
            }
        }
    }
}