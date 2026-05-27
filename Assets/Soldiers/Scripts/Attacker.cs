using System.Collections;
using UnityEngine;

public class Attacker : Soldier
{
    public override bool ChangeTargetCondition()
    {
        return GetTarget() == null || !GetTarget().IsAlive() || !IsInRange(GetTarget());
    }

    public override bool CompareOwnerId(Soldier other)
    {
        return GetOwnerId() != other.GetOwnerId();
    }
    
    private IEnumerator DelayedDamage(Soldier target) {
        yield return new WaitForSeconds(GetAttackSpeed()-0.1f);
        
        CombatActionSO action = GetCombatAction();
        if (action != null) {
            action.Execute(this, target);
        }

        if ((action == null || !action.HandlesDamage()) && IsInRange(target))
        {
            target.TakeDamage(this, GetDamage());
        }
    }

}