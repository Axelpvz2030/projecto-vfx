using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public BoxCollider arenaBounds; // Defines the room
    private BossHealth bossHealth;

    [Header("Teleport Settings")]
    public float minDis = 5f;
    public float maxDis = 15f;
    public float teleportOutTime = 1f;
    public float teleportInTime = 1f;

    [Header("Attack Settings")]
    public float waitTimeAfterAttack = 2f;
    public List<BossAttack> attackList; // Add your modular attack scripts here in the inspector

    [Header("State Flags")]
    public bool isAttacking = false;
    public bool lookAtPlayer = true;
    
    // Coroutine tracking so we can interrupt cleanly
    private Coroutine currentCycle;
    private BossAttack currentAttackScript;

    private void Start()
    {
        bossHealth = GetComponent<BossHealth>();
        
        // Start the infinite combat loop
        currentCycle = StartCoroutine(CombatCycle());
    }

    private void Update()
    {
        if (lookAtPlayer && player != null ) // Or keep looking while attacking, up to you!
        {
            LookAtPlayer();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep the boss from tilting up/down
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    // This is the main loop
    private IEnumerator CombatCycle()
    {
        while (true) // Infinite loop
        {
            yield return StartCoroutine(TeleportSequence());

            if (attackList.Count > 0)
            {
                isAttacking = true;
                
                // Pick a random attack
                int randomIndex = Random.Range(0, attackList.Count);
                currentAttackScript = attackList[randomIndex];

                // Wait for the attack to finish
                yield return StartCoroutine(currentAttackScript.ExecuteAttack(this));
                
                isAttacking = false;
                currentAttackScript = null;
            }

            yield return new WaitForSeconds(waitTimeAfterAttack);
        }
    }

    private IEnumerator TeleportSequence()
    {
        // 1. Teleport Out (Play animation/particles here later)
        yield return new WaitForSeconds(teleportOutTime);

        // 2. Move to new location
        transform.position = GetValidTeleportPosition();

        // 3. Teleport In (Play animation/particles here later)
        yield return new WaitForSeconds(teleportInTime);

        // Reset state
        lookAtPlayer = true;
        if (bossHealth != null)
        {
            bossHealth.canBeHarmed = true; // Boss is vulnerable again
        }
    }

    private Vector3 GetValidTeleportPosition()
    {
        // 1. Check if references are missing
        if (player == null)
        {
            Debug.LogWarning("BossAI: Teleport failed! The Player reference is not assigned in the Inspector.");
            return transform.position;
        }
        if (arenaBounds == null)
        {
            Debug.LogWarning("BossAI: Teleport failed! The Arena Bounds reference is not assigned in the Inspector.");
            return transform.position;
        }

        Vector3 bestPos = transform.position;
        Bounds bounds = arenaBounds.bounds;

        // Try up to 30 times to find a valid spot
        for (int i = 0; i < 30; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(minDis, maxDis);
            
            Vector3 offset = new Vector3(Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0, Mathf.Cos(randomAngle * Mathf.Deg2Rad)) * randomDistance;
            Vector3 potentialPos = player.position + offset;

            // FIX: Force the Y coordinate to match the center of the arena's bounds.
            // This prevents the check from failing just because the player is standing slightly above/below the arena floor.
            potentialPos.y = bounds.center.y;

            if (bounds.Contains(potentialPos))
            {
                bestPos = potentialPos;
                // Once a valid X/Z spot is found, reset the Y to the boss's actual height
                bestPos.y = transform.position.y; 
                return bestPos;
            }
        }
        
        // 2. Check if bounds are too small or minDis is too large
        Debug.LogWarning("BossAI: Teleport failed! Could not find a spot inside the arena bounds after 30 tries. Your minDis/maxDis might be pushing the point outside the walls, or your BoxCollider is too small.");
        
        return bestPos;
    }
    
    public void InterruptAndForceTeleport()
    {
        // Stop whatever the boss is currently doing
        if (currentCycle != null) StopCoroutine(currentCycle);
        
        // Cancel the specific attack script if it's running
        if (currentAttackScript != null)
        {
            currentAttackScript.CancelAttack();
            currentAttackScript = null;
        }

        isAttacking = false;
        
        // Restart the cycle starting with a teleport
        currentCycle = StartCoroutine(CombatCycle());
    }

    public void HandleDeath()
    {
        // Stop the CombatCycle and any Teleport sequences
        StopAllCoroutines(); 
        
        // Cancel the specific attack script if one is currently firing
        if (currentAttackScript != null)
        {
            currentAttackScript.CancelAttack();
            currentAttackScript = null;
        }

        isAttacking = false;
        lookAtPlayer = false;
    }
}