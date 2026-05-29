using UnityEngine;

public abstract class CombatActionSO : ScriptableObject
{
    public abstract void Execute(GameObject source, GameObject target);
}