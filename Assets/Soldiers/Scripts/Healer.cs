using UnityEngine;

public class Healer : Soldier
{
    public override bool CompareOwnerId(Soldier other)
    {
        return GetOwnerId() == other.GetOwnerId();
    }


    public override void TakeDamage(Soldier source, float damage) {
        SetHealth(GetHealth() - damage);
        if (GetHealth() <= 0) {
            Die();
        }
    }

    
    public override void Action(Soldier target) {
        if (IsInRange(target) && Time.time >= GetLastActionTime() + GetAttackSpeed()) {
            SetLastActionTime(Time.time);
            //AudioSource.PlayClipAtPoint(sound, transform.position);
            target.Heal(GetDamage());
        }
    }

}