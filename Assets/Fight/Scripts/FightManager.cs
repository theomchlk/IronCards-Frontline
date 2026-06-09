using System;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Scened;
using Unity.VisualScripting;

public class FightManager : NetworkBehaviour
{
    [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private SoldierRegistry soldierRegistry;

    [Header("Ground")]
    [SerializeField] private Renderer leftGroundRenderer;
    [SerializeField] private Renderer rightGroundRenderer;

    [Header("Camp Canvases")]
    [SerializeField] private Canvas canvasLeft;
    [SerializeField] private Canvas canvasRight;
    [SerializeField] private GameObject cardUIPrefab;

    private Material _leftGroundMaterial;
    private Material _rightGroundMaterial;
    private PlayerState localPlayerState;
    private Dictionary<Localisation, CardsSO> playerLeftCamp;
    private Dictionary<Localisation, CardsSO> playerRightCamp;
    public static FightManager Instance;

    private readonly Dictionary<int, Soldier> soldierByNetId = new();
    private readonly Dictionary<int, List<Soldier>> soldiersByGroupId = new();
    private readonly Dictionary<int, int> groupTargets = new(); // groupId allié -> groupId ciblé
    private int _nextSoldierNetId;

    public static int ComputeGroupId(int playerId, int row, int col) => playerId * 10000 + row * 100 + col;

    public int GetTargetGroupId(int groupId) => groupTargets.TryGetValue(groupId, out int t) ? t : -1;


    void Awake()
    {
        Instance = this;
    }

    public static void RegisterPlayerState(PlayerState psOwner, PlayerState psLeft, PlayerState psRight)
    {
        if (Instance == null)
        {
            Debug.LogError("FightManager instance is null. Cannot register local player.");
            return;
        }

        if (psOwner == null || psLeft == null || psRight == null)
        {
            Debug.LogError("One or more PlayerState references are null. Cannot register local player.");
            return;
        }


        Instance.localPlayerState = psOwner;
        Instance.soldierByNetId.Clear();
        Instance.soldiersByGroupId.Clear();
        Instance.groupTargets.Clear();
        Instance._nextSoldierNetId = 0;

        Instance.playerLeftCamp  = BuildCamp(psLeft.Camp);
        Instance.playerRightCamp = BuildCamp(psRight.Camp);

        // Cibles de groupe définies pendant la planification (ally groupId -> target groupId)
        BuildGroupTargets(psLeft);
        BuildGroupTargets(psRight);

        if (Instance.soldierRegistry == null)
            Instance.soldierRegistry = Instance.GetComponentInChildren<SoldierRegistry>();

        int nbRow = GameManager.Instance.nbRow.Value;
        int nbCol = GameManager.Instance.nbCol.Value;

        Instance.PopulateCanvas(Instance.canvasLeft,  Instance.playerLeftCamp,  nbRow, nbCol);
        Instance.PopulateCanvas(Instance.canvasRight, Instance.playerRightCamp, nbRow, nbCol);

        Canvas.ForceUpdateCanvases();

        Instance.SetGroundColor(Instance.leftGroundRenderer,  ref Instance._leftGroundMaterial,  psLeft.playerColor.Value);
        Instance.SetGroundColor(Instance.rightGroundRenderer, ref Instance._rightGroundMaterial, psRight.playerColor.Value);
        Instance.SpawnSoldiers(Instance.playerLeftCamp,  Instance.canvasLeft,  psLeft.IdPlayer,  psLeft.playerColor.Value);
        Instance.SpawnSoldiers(Instance.playerRightCamp, Instance.canvasRight, psRight.IdPlayer, psRight.playerColor.Value);
    }

    private static void BuildGroupTargets(PlayerState ps)
    {
        if (ps == null || ps.Camp == null) return;
        foreach (var kvp in ps.Camp.CardTargets)
        {
            int myGroupId = ComputeGroupId(ps.IdPlayer, kvp.Key.Row, kvp.Key.Col);
            Instance.groupTargets[myGroupId] = kvp.Value; // la valeur est déjà le groupId cible
        }
    }

    private static Dictionary<Localisation, CardsSO> BuildCamp(PlayerCamp camp)
    {
        var dict = new Dictionary<Localisation, CardsSO>();
        if (camp == null) return dict;
        foreach (var kvp in camp.CardsOnCamp)
        {
            if (string.IsNullOrEmpty(kvp.Value)) continue;
            var card = DataBaseItem.Instance.GetDataItem(kvp.Value) as CardsSO;
            if (card != null) dict[kvp.Key] = card;
        }
        return dict;
    }

    // VLG du canvas : nbRow HLG × nbCol cartes, cache les slots sans carte
    private void PopulateCanvas(Canvas canvas, Dictionary<Localisation, CardsSO> camp, int nbRow, int nbCol)
    {
        Transform vlg = canvas.GetComponentInChildren<UnityEngine.UI.VerticalLayoutGroup>().transform;

        for (int row = 0; row < nbRow; row++)
        {
            var hlgGo = new GameObject($"Row_{row}");
            hlgGo.transform.SetParent(vlg, false);
            var hlg = hlgGo.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            hlg.childControlWidth  = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth  = true;
            hlg.childForceExpandHeight = true;
            hlg.spacing = 1f;

            for (int col = 0; col < nbCol; col++)
            {
                var loc  = new Localisation(row, col);
                var slot = Instantiate(cardUIPrefab, hlgGo.transform);

                if (camp.TryGetValue(loc, out CardsSO card))
                    slot.GetComponent<CardUI>().SetCardUI(card);
                else
                {
                    var cg = slot.GetComponent<CanvasGroup>() ?? slot.AddComponent<CanvasGroup>();
                    cg.alpha = 0f;
                    cg.blocksRaycasts = false;
                    cg.interactable   = false;
                }
            }
        }
    }

    private void SpawnSoldiers(Dictionary<Localisation, CardsSO> camp, Canvas canvas, int playerId, Color playerColor)
    {
        Transform vlg = canvas.GetComponentInChildren<UnityEngine.UI.VerticalLayoutGroup>().transform;

        foreach (var kvp in camp)
        {
            Localisation loc = kvp.Key;
            CardsSO card     = kvp.Value;
            int groupId      = ComputeGroupId(playerId, loc.Row, loc.Col);

            Vector3 basePos = vlg.GetChild(loc.Row).GetChild(loc.Col).position + Vector3.up * spawnHeight;

            for (int j = 0; j < card.nbSoldiers; j++)
            {
                float angle = j * Mathf.PI * 2f / card.nbSoldiers;
                Vector3 circleOffset = new Vector3(Mathf.Cos(angle) * 2f, 0f, Mathf.Sin(angle) * 2f);
                SpawnSoldier(card.soldierPrefab, basePos + circleOffset, playerId, playerColor, groupId);
            }
        }
    }

    private void SpawnSoldier(GameObject soldierPrefab, Vector3 spawnPosition, int playerId, Color playerColor, int groupId)
    {
        GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
        Soldier sol = soldier.GetComponent<Soldier>();

        int netId = _nextSoldierNetId++;
        sol.SetNetId(netId);
        sol.SetOwnerId(playerId);
        sol.SetPlayerColor(playerColor);
        sol.SetGroupId(groupId);
        sol.SetFightManager(this);
        soldierByNetId[netId] = sol;

        if (!soldiersByGroupId.ContainsKey(groupId))
            soldiersByGroupId[groupId] = new List<Soldier>();
        soldiersByGroupId[groupId].Add(sol);

        if (playerId == localPlayerState.IdPlayer)
            sol.SetIsOwnerPlayer(true);

        soldier.transform.SetParent(transform);
    }

    public List<Soldier> GetGroup(int groupId)
    {
        soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group);
        return group;
    }

    private void SetGroundColor(Renderer groundRenderer, ref Material matInstance, Color playerColor)
    {
        if (matInstance == null)
        {
            matInstance = new Material(groundRenderer.sharedMaterial);
            groundRenderer.material = matInstance;
        }
        matInstance.color = playerColor * 0.35f;
    }

    public PlayerState GetLocalPlayerState() => localPlayerState;

    [ServerRpc(RequireOwnership = false)]
    public void RemoveSoldierFromGroupId(int groupId, int soldierNetId, NetworkConnection conn = null)
    {
        if (!soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group))
            return;

        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier))
            return;

        group.Remove(soldier);

        if (group.Count == 0)
        {
            soldiersByGroupId.Remove(groupId);

            int ownerId = soldier.GetOwnerId();

            if (IsPlayerDefeated(ownerId))
            {
                Debug.Log($"Player {ownerId} has no troops left!");
                OnPlayerDefeated(ownerId);
            }
        }
    }



    // Méthode pour le networking

    private bool ValidateOwnership(int soldierNetId, NetworkConnection conn, out Soldier soldier)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out soldier)) return false;
        PlayerState ps = conn?.FirstObject?.GetComponent<PlayerState>();
        return ps != null && soldier.GetOwnerId() == ps.IdPlayer;
    }


    [ServerRpc(RequireOwnership = false)]
    public void CmdSetSoldierControlled(int soldierNetId, bool controlled, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out _)) return;
        RpcSetSoldierControlled(soldierNetId, controlled);
    }

    [ObserversRpc]
    private void RpcSetSoldierControlled(int soldierNetId, bool controlled)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier)) return;
        soldier.SetIsControlledByPlayer(controlled);
    }

    // Déplacement vers un point

    [ServerRpc(RequireOwnership = false)]
    public void CmdMoveTo(int soldierNetId, Vector3 destination, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out Soldier soldier)) return;
        if (soldier.GetState() != SoldierState.PlayerControlled) return;
        RpcMoveTo(soldierNetId, destination);
    }

    [ObserversRpc]
    private void RpcMoveTo(int soldierNetId, Vector3 destination)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier)) return;
        soldier.HandleMovementRigidbody(destination);
        soldier.SetTarget(null);
    }

    // Assignation d'une cible ennemie

    [ServerRpc(RequireOwnership = false)]
    public void CmdSetTarget(int soldierNetId, int targetNetId, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out Soldier soldier)) return;
        if (soldier.GetState() != SoldierState.PlayerControlled) return;
        RpcSetTarget(soldierNetId, targetNetId);
    }

    [ObserversRpc]
    private void RpcSetTarget(int soldierNetId, int targetNetId)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier)) return;
        soldierByNetId.TryGetValue(targetNetId, out Soldier target);
        soldier.SetTarget(target);
    }

    // Commandes de groupe

    [ServerRpc(RequireOwnership = false)]
    public void CmdSetGroupControlled(int groupId, bool controlled, NetworkConnection conn = null)
    {
        PlayerState ps = conn?.FirstObject?.GetComponent<PlayerState>();
        if (ps == null || !soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group)) return;
        if (group.Count == 0 || group[0].GetOwnerId() != ps.IdPlayer) return;
        RpcSetGroupControlled(groupId, controlled);
    }

    [ObserversRpc]
    private void RpcSetGroupControlled(int groupId, bool controlled)
    {
        if (!soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group)) return;
        foreach (Soldier sol in group) sol.SetIsControlledByPlayer(controlled);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdMoveGroupTo(int groupId, Vector3 center, NetworkConnection conn = null)
    {
        PlayerState ps = conn?.FirstObject?.GetComponent<PlayerState>();
        if (ps == null || !soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group)) return;
        if (group.Count == 0 || group[0].GetOwnerId() != ps.IdPlayer) return;
        RpcMoveGroupTo(groupId, center);
    }

    [ObserversRpc]
    private void RpcMoveGroupTo(int groupId, Vector3 center)
    {
        if (!soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group)) return;
        int count = group.Count;
        for (int i = 0; i < count; i++)
        {
            if (!group[i].IsAlive()) continue;
            float angle = i * Mathf.PI * 2f / count;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * 1.5f, 0f, Mathf.Sin(angle) * 1.5f);
            group[i].HandleMovementRigidbody(center + offset);
            group[i].SetTarget(null);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdSetGroupTarget(int groupId, int targetNetId, NetworkConnection conn = null)
    {
        PlayerState ps = conn?.FirstObject?.GetComponent<PlayerState>();
        if (ps == null || !soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group)) return;
        if (group.Count == 0 || group[0].GetOwnerId() != ps.IdPlayer) return;
        RpcSetGroupTarget(groupId, targetNetId);
    }

    [ObserversRpc]
    private void RpcSetGroupTarget(int groupId, int targetNetId)
    {
        if (!soldiersByGroupId.TryGetValue(groupId, out List<Soldier> group)) return;
        soldierByNetId.TryGetValue(targetNetId, out Soldier target);
        foreach (Soldier sol in group) sol.SetTarget(target);
    }

    // Déplacement (IA + joueur)

    [ServerRpc(RequireOwnership = false)]
    public void CmdMoveSoldier(int soldierNetId, Vector3 destination, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out _)) return;
        RpcMoveSoldier(soldierNetId, destination);
    }

    [ObserversRpc]
    private void RpcMoveSoldier(int soldierNetId, Vector3 destination)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier)) return;
        soldier.HandleMovementRigidbody(destination);
    }

    // Arrêt

    [ServerRpc(RequireOwnership = false)]
    public void CmdStopSoldier(int soldierNetId, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out _)) return;
        RpcStopSoldier(soldierNetId);
    }

    [ObserversRpc]
    private void RpcStopSoldier(int soldierNetId)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier)) return;
        soldier.StopMovementRigidbody();
    }

    // Action (animation + dégâts via CmdApplyDamage)

    [ServerRpc(RequireOwnership = false)]
    public void CmdRequestAction(int soldierNetId, int targetNetId, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out _)) return;
        if (!soldierByNetId.TryGetValue(targetNetId, out _)) return;
        RpcExecuteAction(soldierNetId, targetNetId);
    }

    [ObserversRpc]
    private void RpcExecuteAction(int soldierNetId, int targetNetId)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier)) return;
        soldierByNetId.TryGetValue(targetNetId, out Soldier target);
        soldier.ExecuteNetworkAction(target);
    }

    // Dégâts (serveur résout le jet d'armure)

    [ServerRpc(RequireOwnership = false)]
    public void CmdApplyDamage(int targetNetId, float damage, NetworkConnection conn = null)
    {
        if (!soldierByNetId.TryGetValue(targetNetId, out Soldier target)) return;
        bool blocked = UnityEngine.Random.value < target.GetArmorProtection();
        RpcApplyDamage(targetNetId, damage, blocked);
    }

    [ObserversRpc]
    private void RpcApplyDamage(int targetNetId, float damage, bool blocked)
    {
        if (!soldierByNetId.TryGetValue(targetNetId, out Soldier soldier)) return;
        soldier.ApplyDamageLocal(damage, blocked);
    }

    // Correction de position (divergence physique)

    [ServerRpc(RequireOwnership = false)]
    public void CmdSyncPosition(int soldierNetId, Vector3 position, Quaternion rotation, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out _)) return;
        RpcSyncPosition(soldierNetId, position, rotation);
    }

    [ObserversRpc]
    private void RpcSyncPosition(int soldierNetId, Vector3 position, Quaternion rotation)
    {
        if (!soldierByNetId.TryGetValue(soldierNetId, out Soldier soldier)) return;
        if (soldier.IsOwnerPlayer) return;
        soldier.SnapToPosition(position, rotation);
    }

    // Soin (Healer)

    [ServerRpc(RequireOwnership = false)]
    public void CmdApplyHeal(int targetNetId, float amount, NetworkConnection conn = null)
    {
        if (!soldierByNetId.TryGetValue(targetNetId, out _)) return;
        RpcApplyHeal(targetNetId, amount);
    }

    [ObserversRpc]
    private void RpcApplyHeal(int targetNetId, float amount)
    {
        if (!soldierByNetId.TryGetValue(targetNetId, out Soldier soldier)) return;
        soldier.ApplyHealLocal(amount);
    }
    
    private bool IsPlayerDefeated(int playerId)
    {
        foreach (var kvp in soldiersByGroupId)
        {
            if (kvp.Value.Count > 0 && kvp.Value[0].GetOwnerId() == playerId)
                return false;
        }
        return true;
    }

    private void OnPlayerDefeated(int ownerId)
    {
        PlayerState ps = PlayerRegistry.GetPlayerState(ownerId);
        int nbGroupOpponentLeft = soldiersByGroupId.Count;
        ps.DecreaseHealth(nbGroupOpponentLeft);

        var duels = GameManager.Instance._duelDictionary;
        if (duels.TryGetValue(ownerId, out int winnerId) && winnerId != -1)
        {
            PlayerState winnerPs = PlayerRegistry.GetPlayerState(winnerId);
            string winnerName = winnerPs != null ? winnerPs.playerName.Value : "?";
            if (ps != null && ps.Owner != null) TargetShowBattleResult(ps.Owner, winnerName);
            if (winnerPs != null && winnerPs.Owner != null) TargetShowBattleResult(winnerPs.Owner, winnerName);
        }

        GameManager.Instance.SetEndDuel();
    }

    [TargetRpc]
    private void TargetShowBattleResult(NetworkConnection conn, string winnerName)
    {
        if (BattleMessageUI.Instance != null)
            BattleMessageUI.Instance.Show($"{winnerName} a gagné cette bataille");
    }

}