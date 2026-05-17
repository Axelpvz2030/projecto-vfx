using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;
    public BoxCollider arenaBounds; 
    public BossAfterImage afterImageEffect;
    private BossHealth bossHealth;

    public float minDis = 5f;
    public float maxDis = 15f;
    public float moveSpeed = 8f; 
    public float waitBeforeMove = 1f;
    public float waitAfterMove = 1f;

    public float waitTimeAfterAttack = 2f;
    public List<BossAttack> attackList; 

    public bool isAttacking = false;
    public bool lookAtPlayer = true;
    
    private Coroutine currentCycle;
    private BossAttack currentAttackScript;

    private void Start()
    {
        bossHealth = GetComponent<BossHealth>();
        currentCycle = StartCoroutine(CombatCycle());
    }

    private void Update()
    {
        if (lookAtPlayer && player != null)
        {
            LookAtPlayer();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private IEnumerator CombatCycle()
    {
        while (true) 
        {
            yield return StartCoroutine(MoveSequence());

            if (attackList.Count > 0)
            {
                isAttacking = true;
                
                int randomIndex = Random.Range(0, attackList.Count);
                currentAttackScript = attackList[randomIndex];

                yield return StartCoroutine(currentAttackScript.ExecuteAttack(this));
                
                isAttacking = false;
                currentAttackScript = null;
            }

            yield return new WaitForSeconds(waitTimeAfterAttack);
        }
    }

    private IEnumerator MoveSequence()
    {
        yield return new WaitForSeconds(waitBeforeMove);

        Vector3 targetPos = GetValidTargetPosition();

        if (afterImageEffect != null) afterImageEffect.StartSpawningAfterImages(); 

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos; 

        if (afterImageEffect != null) afterImageEffect.StopSpawningAfterImages();

        yield return new WaitForSeconds(waitAfterMove);

        lookAtPlayer = true;
        if (bossHealth != null)
        {
            bossHealth.canBeHarmed = true; 
        }
    }

    private Vector3 GetValidTargetPosition()
    {
        if (player == null || arenaBounds == null) return transform.position;

        Vector3 bestPos = transform.position;
        Bounds bounds = arenaBounds.bounds;

        for (int i = 0; i < 30; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(minDis, maxDis);
            
            Vector3 offset = new Vector3(Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0, Mathf.Cos(randomAngle * Mathf.Deg2Rad)) * randomDistance;
            Vector3 potentialPos = player.position + offset;
            potentialPos.y = bounds.center.y;

            if (bounds.Contains(potentialPos))
            {
                bestPos = potentialPos;
                bestPos.y = transform.position.y; 
                return bestPos;
            }
        }
        
        return bestPos;
    }
    
    public void InterruptAndForceTeleport()
    {
        if (currentCycle != null) StopCoroutine(currentCycle);
        
        if (currentAttackScript != null)
        {
            currentAttackScript.CancelAttack();
            currentAttackScript = null;
        }

        isAttacking = false;
        
        currentCycle = StartCoroutine(CombatCycle());
    }

    public void HandleDeath()
    {
        StopAllCoroutines(); 
        
        if (currentAttackScript != null)
        {
            currentAttackScript.CancelAttack();
            currentAttackScript = null;
        }

        isAttacking = false;
        lookAtPlayer = false;
    }
}