using System;
using System.Collections.Generic;
using UnityEngine;

public class SoldierRegistry : MonoBehaviour
{
    public static SoldierRegistry Instance { get; private set; }

    private readonly List<Soldier> _soldiers = new List<Soldier>();

    public static event Action<Soldier> OnSoldierDied;
    public static event Action<int> OnTeamEliminated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            OnSoldierDied = null;
            OnTeamEliminated = null;
        }
    }

    public void Register(Soldier soldier)
    {
        if (!_soldiers.Contains(soldier))
            _soldiers.Add(soldier);
    }

    public void Unregister(Soldier soldier)
    {
        if (_soldiers.Remove(soldier))
        {
            OnSoldierDied?.Invoke(soldier);
            CheckTeamElimination(soldier.GetOwnerId());
        }
    }

    public IReadOnlyList<Soldier> GetAll() => _soldiers.AsReadOnly();

    public Soldier GetNearestTarget(Soldier requester)
    {
        Soldier nearest = null;
        float shortestSqDist = float.MaxValue;

        foreach (Soldier s in _soldiers)
        {
            if (s == requester || !s.IsAlive() || !requester.CompareOwnerId(s))
                continue;

            float sqDist = (requester.GetPosition() - s.GetPosition()).sqrMagnitude;
            if (sqDist < shortestSqDist)
            {
                shortestSqDist = sqDist;
                nearest = s;
            }
        }

        return nearest;
    }

    public Soldier GetMostWoundedAlly(Soldier requester)
    {
        Soldier mostWounded = null;
        float lowestRatio = float.MaxValue;

        foreach (Soldier s in _soldiers)
        {
            if (s == requester || !s.IsAlive() || s.GetOwnerId() != requester.GetOwnerId())
                continue;

            float maxHp = s.GetMaxHealth();
            if (maxHp <= 0f) continue;

            float ratio = s.GetHealth() / maxHp;
            if (ratio < 1f && ratio < lowestRatio)
            {
                lowestRatio = ratio;
                mostWounded = s;
            }
        }

        return mostWounded;
    }

    private void CheckTeamElimination(int teamId)
    {
        foreach (Soldier s in _soldiers)
        {
            if (s.GetOwnerId() == teamId)
                return;
        }

        OnTeamEliminated?.Invoke(teamId);
    }
}
