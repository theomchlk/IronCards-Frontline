using UnityEngine;

public abstract class CombatActionSO : ScriptableObject
{
    public abstract void Execute(Soldier source, Soldier target);

    public virtual bool HandlesDamage() => false;
}