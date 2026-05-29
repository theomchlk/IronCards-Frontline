using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Soldier
{
    public override bool ChangeTargetCondition()
    {
        return GetTarget() == null || !GetTarget().IsAlive() || !IsInRange(GetTarget()) || GetTarget().GetHealth() >= GetTarget().GetMaxHealth();
    }

    public override bool CompareOwnerId(Soldier other)
    {
        return GetOwnerId() == other.GetOwnerId();
    }

    public override Soldier GetNearestTarget()
    {
        if (SoldierRegistry.Instance == null)
            return null;

        Soldier mostWounded = SoldierRegistry.Instance.GetMostWoundedAlly(this);

        if (mostWounded != null && IsInRange(mostWounded))
            return mostWounded;

        return mostWounded;
    }

    protected override IEnumerator DelayedNetworkAction(Soldier target)
    {
        yield return new WaitForSeconds(GetAttackSpeed() - 0.1f);
        if (target == null || !target.IsAlive()) yield break;
        FightManager.Instance?.CmdApplyHeal(target.GetNetId(), GetDamage());
    }

    private IEnumerator DelayedHeal(Soldier target) {
        yield return new WaitForSeconds(GetAttackSpeed()-0.1f);

        CombatActionSO action = GetCombatAction();
        if (action != null) {
            action.Execute(this.gameObject, target.gameObject);
        }
        if (IsInRange(target))
        {
            target.Heal(GetDamage());
        }
    }

}
