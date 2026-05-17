using System.Collections;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    public abstract IEnumerator ExecuteAttack(BossAI bossAI);
    
    public abstract void CancelAttack();
}