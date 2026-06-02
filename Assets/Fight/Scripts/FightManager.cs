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
    [SerializeField] private Canvas canvasLeft;
    [SerializeField] private Canvas canvasRight;
    [SerializeField] private SoldierRegistry soldierRegistry;
    [SerializeField] private AllCardsSO allCardsSO;
    [SerializeField] private Renderer leftGroundRenderer;
    [SerializeField] private Renderer rightGroundRenderer;
    private Material _leftGroundMaterial;
    private Material _rightGroundMaterial;
    private PlayerState localPlayerState;
    private CardsSO[] playerLeftCamp;
    private CardsSO[] playerRightCamp;
    public static FightManager Instance;

    private readonly Dictionary<int, Soldier> _soldierByNetId = new();
    private int _nextSoldierNetId;


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
        Instance.playerLeftCamp = new CardsSO[35];
        Instance.playerRightCamp = new CardsSO[35];
        Instance._soldierByNetId.Clear();
        Instance._nextSoldierNetId = 0;

        for (int i = 0; i < psLeft.playerCamp.Value.campCardsId.Length; i++)
        {
            int cardId = psLeft.playerCamp.Value.campCardsId[i];
            if (cardId >= 0 && cardId < Instance.allCardsSO.allCards.Length)
            {
                Instance.playerLeftCamp[i] = Instance.allCardsSO.allCards[cardId];
            }
        }

        for (int i = 0; i < psRight.playerCamp.Value.campCardsId.Length; i++)
        {
            int cardId = psRight.playerCamp.Value.campCardsId[i];
            if (cardId >= 0 && cardId < Instance.allCardsSO.allCards.Length)
            {
                Instance.playerRightCamp[i] = Instance.allCardsSO.allCards[cardId];
            }
        }


        if (Instance.soldierRegistry == null)
            Instance.soldierRegistry = Instance.GetComponentInChildren<SoldierRegistry>();

        Instance.HideCards();
        CardUI[] cardUILeft = Instance.canvasLeft.GetComponentsInChildren<CardUI>();
        CardUI[] cardUIRight = Instance.canvasRight.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < Instance.playerLeftCamp.Length; i++)
        {
            if (Instance.playerLeftCamp[i] != null)
            {
                cardUILeft[i].SetCardUI(Instance.playerLeftCamp[i]);
                CanvasGroup canvasGroup = cardUILeft[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        for (int i = 0; i < Instance.playerRightCamp.Length; i++)
        {
            if (Instance.playerRightCamp[i] != null)
            {
                cardUIRight[i].SetCardUI(Instance.playerRightCamp[i]);
                CanvasGroup canvasGroup = cardUIRight[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        Canvas.ForceUpdateCanvases();

        Instance.SetGroundColor(Instance.leftGroundRenderer, ref Instance._leftGroundMaterial, psLeft.playerColor.Value);
        Instance.SetGroundColor(Instance.rightGroundRenderer, ref Instance._rightGroundMaterial, psRight.playerColor.Value);
        Instance.SpawnSoldiers(Instance.playerLeftCamp, cardUILeft, psLeft.IdPlayer, psLeft.playerColor.Value);
        Instance.SpawnSoldiers(Instance.playerRightCamp, cardUIRight, psRight.IdPlayer, psRight.playerColor.Value);
    }

    private void HideCards()
    {
        CardUI[] cardUILeft = canvasLeft.GetComponentsInChildren<CardUI>();
        CardUI[] cardUIRight = canvasRight.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < cardUILeft.Length; i++)
        {
            CanvasGroup canvasGroup = cardUILeft[i].GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        for (int i = 0; i < cardUIRight.Length; i++)
        {
            CanvasGroup canvasGroup = cardUIRight[i].GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    private void SpawnSoldiers(CardsSO[] cards, CardUI[] cardUIs, int playerId, Color playerColor)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                GameObject soldierPrefab = cards[i].soldierPrefab;
                RectTransform cardRect = cardUIs[i].GetComponent<RectTransform>();
                for (int j = 0; j < cards[i].nbSoldiers; j++)
                {
                    float angle = j * Mathf.PI * 2 / cards[i].nbSoldiers;
                    float radius = 2f;
                    Vector3 circleOffset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                    Vector3 spawnPosition = cardRect.position + new Vector3(0, spawnHeight, 0) + circleOffset;
                    SpawnSoldier(soldierPrefab, spawnPosition, playerId, playerColor);
                }
            }
        }
    }

    private void SpawnSoldier(GameObject soldierPrefab, Vector3 spawnPosition, int playerId, Color playerColor)
    {
        GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
        Soldier sol = soldier.GetComponent<Soldier>();

        int netId = _nextSoldierNetId++;
        sol.SetNetId(netId);
        sol.SetOwnerId(playerId);
        sol.SetPlayerColor(playerColor);
        sol.SetFightManager(this);
        _soldierByNetId[netId] = sol;

        if (playerId == localPlayerState.IdPlayer)
            sol.SetIsOwnerPlayer(true);

        soldier.transform.SetParent(transform);
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

    // Méthode pour le networking

    private bool ValidateOwnership(int soldierNetId, NetworkConnection conn, out Soldier soldier)
    {
        if (!_soldierByNetId.TryGetValue(soldierNetId, out soldier)) return false;
        var ps = conn?.FirstObject?.GetComponent<PlayerState>();
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
        if (!_soldierByNetId.TryGetValue(soldierNetId, out var soldier)) return;
        soldier.SetIsControlledByPlayer(controlled);
    }

    // Déplacement vers un point

    [ServerRpc(RequireOwnership = false)]
    public void CmdMoveTo(int soldierNetId, Vector3 destination, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out var soldier)) return;
        if (soldier.GetState() != SoldierState.PlayerControlled) return;
        RpcMoveTo(soldierNetId, destination);
    }

    [ObserversRpc]
    private void RpcMoveTo(int soldierNetId, Vector3 destination)
    {
        if (!_soldierByNetId.TryGetValue(soldierNetId, out var soldier)) return;
        soldier.HandleMovementRigidbody(destination);
        soldier.SetTarget(null);
    }

    // Assignation d'une cible ennemie

    [ServerRpc(RequireOwnership = false)]
    public void CmdSetTarget(int soldierNetId, int targetNetId, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out var soldier)) return;
        if (soldier.GetState() != SoldierState.PlayerControlled) return;
        RpcSetTarget(soldierNetId, targetNetId);
    }

    [ObserversRpc]
    private void RpcSetTarget(int soldierNetId, int targetNetId)
    {
        if (!_soldierByNetId.TryGetValue(soldierNetId, out var soldier)) return;
        _soldierByNetId.TryGetValue(targetNetId, out var target);
        soldier.SetTarget(target);
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
        if (!_soldierByNetId.TryGetValue(soldierNetId, out var soldier)) return;
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
        if (!_soldierByNetId.TryGetValue(soldierNetId, out var soldier)) return;
        soldier.StopMovementRigidbody();
    }

    // Action (animation + dégâts via CmdApplyDamage)

    [ServerRpc(RequireOwnership = false)]
    public void CmdRequestAction(int soldierNetId, int targetNetId, NetworkConnection conn = null)
    {
        if (!ValidateOwnership(soldierNetId, conn, out _)) return;
        if (!_soldierByNetId.TryGetValue(targetNetId, out _)) return;
        RpcExecuteAction(soldierNetId, targetNetId);
    }

    [ObserversRpc]
    private void RpcExecuteAction(int soldierNetId, int targetNetId)
    {
        if (!_soldierByNetId.TryGetValue(soldierNetId, out var soldier)) return;
        _soldierByNetId.TryGetValue(targetNetId, out var target);
        soldier.ExecuteNetworkAction(target);
    }

    // Dégâts (serveur résout le jet d'armure)

    [ServerRpc(RequireOwnership = false)]
    public void CmdApplyDamage(int targetNetId, float damage, NetworkConnection conn = null)
    {
        if (!_soldierByNetId.TryGetValue(targetNetId, out var target)) return;
        bool blocked = UnityEngine.Random.value < target.GetArmorProtection();
        RpcApplyDamage(targetNetId, damage, blocked);
    }

    [ObserversRpc]
    private void RpcApplyDamage(int targetNetId, float damage, bool blocked)
    {
        if (!_soldierByNetId.TryGetValue(targetNetId, out var soldier)) return;
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
        if (!_soldierByNetId.TryGetValue(soldierNetId, out var soldier)) return;
        if (soldier.IsOwnerPlayer) return;
        soldier.SnapToPosition(position, rotation);
    }

    // Soin (Healer)

    [ServerRpc(RequireOwnership = false)]
    public void CmdApplyHeal(int targetNetId, float amount, NetworkConnection conn = null)
    {
        if (!_soldierByNetId.TryGetValue(targetNetId, out _)) return;
        RpcApplyHeal(targetNetId, amount);
    }

    [ObserversRpc]
    private void RpcApplyHeal(int targetNetId, float amount)
    {
        if (!_soldierByNetId.TryGetValue(targetNetId, out var soldier)) return;
        soldier.ApplyHealLocal(amount);
    }
}