using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    // Getters
    public abstract string GetName();
    public abstract int GetMaxHealth();
    public abstract int GetHealth();
    public abstract double GetMoveSpeed();
    public abstract int GetDamage();
    public abstract double GetAttackSpeed();
    public abstract double GetRange();
    public abstract bool IsAlive();
    public abstract Soldier GetTarget();
    public abstract Vector3 GetPosition();
    public abstract TargetType GetTargetType();
    public abstract Soldier GetNearestTarget();
   

    // Setters
    public abstract void SetPosition(Vector3 position);
    public abstract void TakeDamage(Soldier source, int damage);
    public abstract void Heal(int amount);


    // Methods
    public abstract bool IsInRange(Soldier target);
    public abstract void Action(Soldier target);
    public abstract void Move(Vector3 destination);
    public abstract void Die();
}
