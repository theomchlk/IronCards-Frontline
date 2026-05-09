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
    
    public override void Action(Soldier target) {
        if (IsInRange(target) && target.IsAlive() && Time.time >= GetLastActionTime() + GetAttackSpeed()) {
            SetLastActionTime(Time.time);
            //AudioSource.PlayClipAtPoint(sound, transform.position);
            target.Heal(GetDamage());
        }
    }

}