using UnityEngine;

public class Tank : Soldier
{
    [SerializeField] private float shieldProtection;
    [SerializeField] private AudioClip shieldSound;


    public override bool CompareOwnerId(Soldier other)
    {
        return GetOwnerId() != other.GetOwnerId();
    }


    public override void TakeDamage(Soldier source, float damage) {
        if (Random.value < shieldProtection)
        {
            AudioSource.PlayClipAtPoint(shieldSound, transform.position);
            return;
        }

        SetHealth(GetHealth() - damage);
        if (GetHealth() <= 0) {
            Die();
        }
    }

    
    public override void Action(Soldier target) {
        if (IsInRange(target) && Time.time >= GetLastActionTime() + GetAttackSpeed()) {
            SetLastActionTime(Time.time);
            //AudioSource.PlayClipAtPoint(sound, transform.position);
            target.TakeDamage(this, GetDamage());
        }
    }

}