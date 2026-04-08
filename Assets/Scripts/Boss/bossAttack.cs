using System.Collections;
using UnityEngine;

// This is an abstract class. We don't attach this directly; 
// we inherit from it to make specific attacks.
public abstract class BossAttack : MonoBehaviour
{
    // The main routine for the attack
    public abstract IEnumerator ExecuteAttack(BossAI bossAI);
    
    // Called if the boss is interrupted (hit) during the attack
    public abstract void CancelAttack();
}