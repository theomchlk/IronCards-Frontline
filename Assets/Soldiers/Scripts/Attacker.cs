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
    
    public override void Action(Soldier target) {
        if (target == null || !target.IsAlive() || !IsInRange(target) || GetOwnerId() == target.GetOwnerId())
            return;
        
        if (IsInRange(target) && target.IsAlive() && Time.time >= GetLastActionTime() + GetAttackSpeed()) {
            SetLastActionTime(Time.time);

            Animator animator = GetAnimator();
            if (animator != null) {
                animator.SetTrigger("Action");
            }


            StartCoroutine(DelayedSound());
            StartCoroutine(DelayedDamage(target));
        }
    }

    private IEnumerator DelayedDamage(Soldier target) {
        yield return new WaitForSeconds(GetAttackSpeed()-0.1f);
        
        CombatActionSO action = GetCombatAction();
        if (action != null) {
            action.Execute(this, target);
        }
        if (IsInRange(target))
        {
            target.TakeDamage(this, GetDamage());
        }
    }

}