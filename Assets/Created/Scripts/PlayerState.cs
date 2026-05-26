    using System;
    using UnityEngine;
    using FishNet.Connection;
    using FishNet.Object;
    using FishNet.Object.Synchronizing;
    using System.Collections.Generic;
    using FishNet;
    using FishNet.Managing.Scened;
    using Unity.VisualScripting;

    public class PlayerState : NetworkBehaviour
    {
        /*public static PlayerState Local;*/
        [SerializeField] private PlayerSO playerConfig;
        /*[SerializeField] private UIManager uiManager;
        
        public UIManager UIManager => uiManager;*/
            //Partie
        private readonly SyncVar<bool> isLobbyLeader = new ();
        private readonly SyncVar<int> idPlayer = new ();
        public int IdPlayer { get => idPlayer.Value; set => idPlayer.Value = value; }
        private readonly SyncVar<int> hp = new ();
        public int Hp => hp.Value;
        public readonly SyncVar<int> money = new ();
        private int _moneyPerMills;
        
        public readonly SyncVar<string> playerName = new();
        public readonly SyncVar<Color> playerColor = new();
        
        //Slot
        public readonly SyncVar<int> nbSlots = new ();
        public readonly SyncVar<int> slotCost = new();
        public readonly SyncVar<int> nbFreeSlots = new();
        //Cards
        public readonly SyncDictionary<string, int> cardsOwned = new();
        //Mill
        public readonly SyncVar<int> nbMills = new();
        public readonly SyncVar<int> millCost = new();

        
        private List<SlotItem> _slotItems = new();
        private int _slotsReadyCount = 0;
        
        private void Awake()
        {
            hp.Value = playerConfig.hpByDefault;
            money.Value = playerConfig.moneyByDefault;
            _moneyPerMills = playerConfig.moneyPerMills;
            idPlayer.OnChange += OnIdPlayerChange;
        }


        [Server]
        public void InitItemsFromDatabase()
        {
            var slotSo = DataBaseItem.Instance.GetDataItem("slot") as SlotSO;
            if (slotSo != null)
            {
                nbSlots.Value  = slotSo.nbSlotByDefault;
                slotCost.Value = slotSo.cost;
                InitDefaultSlots(slotSo, slotSo.nbSlotByDefault);
            }

            var millSo = DataBaseItem.Instance.GetDataItem("mill") as MillSO;
            if (millSo != null)
            {
                nbMills.Value  = millSo.nbMillsByDefault;
                millCost.Value = millSo.cost;
            }
        }
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("PlayerState::OnStartServer");
            PlayerRegistry.Register(OwnerId, this);
            /*GameStateController.Instance.CurrentState.OnPlayerEnter(this);*/
            Debug.Log(InstanceFinder.ClientManager.Connection);
            
            /*playerName.OnChange += OnSetPlayerName;
            playerColor.OnChange += OnSetPlayerColor;*/
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            if (GameStateController.Instance.CurrentState == null) return;
            /*GameStateController.Instance.CurrentState.OnPlayerExit(this);*/
            Debug.Log($"OnStopServer: {InstanceFinder.ClientManager.Connection}");
            
            /*playerName.OnChange -= OnSetPlayerName;
            playerColor.OnChange -= OnSetPlayerColor;*/
            
            /*foreach (var no in Owner.Objects)
            {
                if (no != null && no.IsSpawned)
                    Debug.Log($"no : {no} and isSpawned : {no.IsSpawned}");
                    InstanceFinder.ServerManager.Despawn(no);
            }*/

            PlayerRegistry.Unregister(OwnerId);
        }
        

        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log("Start of PlayerState::OnStartClient");
            if (!IsOwner) return;   
            if (IsHostStarted) isLobbyLeader.Value = true;
            InstanceFinder.SceneManager.OnLoadEnd += OnGameSceneLoad;
            ServerSetPlayerInfo(LocalPlayerPrefs.PlayerName,LocalPlayerPrefs.PlayerColor);
            GameStateController.Instance.OnPlayerEnter(this);
            Debug.Log("End of PlayerState::OnStartClient");
        }

        /*private void OnSetPlayerName(string previous, string next, bool asServer)
        {
            if (asServer) return;
            if (!string.IsNullOrEmpty(playerName.Value) && playerColor.Value != default)
            {
                Debug.Log("OnSetPlayerName");
                ObserversAddPanelPlayerInLobby(this);
            }
        }

        private void OnSetPlayerColor(Color previous, Color next, bool asServer)
        {
            if (asServer) return;
            if (!string.IsNullOrEmpty(playerName.Value) && next != default)
            {
                Debug.Log("OnSetPlayerColor");
                ObserversAddPanelPlayerInLobby(this);
            }
        }*/

        [ServerRpc]
        private void ServerSetPlayerInfo(string namePlayer, Color color, NetworkConnection conn = null)
        {
            playerName.Value = namePlayer;
            playerColor.Value = color;
            Debug.Log($"ServerSetPlayerInfo {namePlayer}");
            ObserversAddPanelPlayerInLobby(this, namePlayer, color);
            SetAllPanelPlayersInLobby(conn ,namePlayer,color);     
        }
        
        [Server]
        private void SetAllPanelPlayersInLobby(NetworkConnection conn, string namePlayer, Color color)
        {
            var i = 0;
            foreach (var ps in PlayerRegistry.GetAll)
            {
                i++;
                Debug.Log($"foreach ps = {ps} and name {ps.playerName.Value}");
                /*if (ps.Owner != conn)
                {*/
                    TargetSetPanelPlayerInLobby(conn, namePlayer, color,i == PlayerRegistry.PlayerCount);
                    Debug.Log($"TargetSetAllPanelPlayersInLobby ps {ps.playerName.Value}");
                /*}*/
            }
            
        }

        [TargetRpc]
        public void TargetSetPanelPlayerInLobby(NetworkConnection conn, string namePlayer, Color color, bool tryApplyNow)
        {
            Debug.Log("PlayerState::TargetSetAllPanelPlayersInLobby");
            LobbyCache.PendingPlayers.Add((this, namePlayer, color));
            Debug.Log($"TargetSetAllPanelPlayersInLobby name {namePlayer}");
            if (tryApplyNow) LobbyCache.TryApply();
        }

        [ObserversRpc(RunLocally = false)]
        public void ObserversAddPanelPlayerInLobby(PlayerState ps, string namePlayer, Color color)
        {
            if (IsOwner) return;
            Debug.Log($"ObserversAddPanelPlayerInLobby ps = {namePlayer}");
            LobbyCache.PendingPlayers.Add((ps, namePlayer, color));
            LobbyCache.TryApply();
        }

        private void OnGameSceneLoad(SceneLoadEndEventArgs args)
        {
            InstanceFinder.SceneManager.OnLoadEnd -= OnGameSceneLoad;
            UIManager.Instance.Bind(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            InstanceFinder.SceneManager.OnLoadEnd -= OnGameSceneLoad;
            if (!IsHostStarted) GameStateController.Instance.OnPlayerExit(idPlayer.Value);
            PlayerRegistry.Unregister(IdPlayer);
            /*GameStateController.Instance.CurrentState.OnPlayerExit(this);
            WhenClientLeave();*/
        }
        
        /*[ServerRpc(RequireOwnership = false)]
        public void WhenClientLeave(NetworkConnection conn = null)
        {
            GameStateController.Instance.CurrentState.OnPlayerExit(this);
        }*/
        
        [ObserversRpc]
        public void ObserversRemovePanelPlayerInLobby(PlayerState ps)
        {
            Debug.Log($"ObserversRemovePanelPlayerInLobby ps.name = {ps}");
            Debug.Log($"Player Conn = {ps}");
            InLobbyUI.Instance.RemovePlayer(ps);
        }

        
        [Server]
        private void InitDefaultSlots(SlotSO data, int count)
        {
            _slotsReadyCount = 0;

            for (int i = 0; i < count; i++)
            {
                var slot = Instantiate(data.goItem).GetComponent<SlotItem>();
                InstanceFinder.ServerManager.Spawn(slot.gameObject);
                slot.Init(data);
                slot.SetOwnerPlayerState(this);   // ← lie le slot à ce PlayerState
                _slotItems.Add(slot);
            }

            nbFreeSlots.Value = count;
        }

        [Server]
        public void SetNewMoney()
        {
            var newMoney = nbMills.Value * _moneyPerMills; 
            AddMoney(newMoney);
        }
        
        [Server]
        public void OnSlotClientReady(SlotItem slot, NetworkConnection conn)
        {
            _slotsReadyCount++;

            Debug.Log($"[Server] Slot prêt {_slotsReadyCount}/{_slotItems.Count}");

            if (_slotsReadyCount < _slotItems.Count) return;

            // Tous les slots sont confirmés côté client → on envoie les TargetRpc
            Debug.Log("[Server] Tous les slots prêts, envoi des TargetRpc");
            foreach (var s in _slotItems)
                s.TargetSpawnItem(Owner, s.Data.Id);
        }

        [Server]
        public void IncreaseNbItem(SyncVar<int> item) => item.Value++;

        
        [Server]
        public void NewCostItemByMultiplier(SyncVar<int> itemCost ,float multiplier)
        {
            itemCost.Value = (int) Math.Ceiling(itemCost.Value * multiplier);
        }

        [Server]
        public bool HaveFreeSlot()
        {
            return nbFreeSlots.Value > 0;
        }

        [Server]
        public void DecrementFreeSlot()
        {
            nbFreeSlots.Value--;
        }
        
        [Server]
        public bool CanAfford(int price) => money.Value >= price ;
        
        [Server]
        public void RemoveMoney(int amount)
        {
            money.Value -= amount;
        }

        [Server]
        private void AddMoney(int amount)
        {
            money.Value += amount;
        }

        [Server]
        public void SetLobbyLeader()
        {
            isLobbyLeader.Value = true;
        }

        [Server]
        public bool IsLobbyLeader() => isLobbyLeader.Value;

        [ServerRpc]
        public void ServerPlayerSurrender(NetworkConnection conn = null)
        {
            if (conn != Owner) return;
            hp.Value = 0;
            TargetDisconnect(conn);
        }

        [TargetRpc]
        private void TargetDisconnect(NetworkConnection conn)
        {
            InstanceFinder.ClientManager.StopConnection();
        }

        private void OnIdPlayerChange(int prev, int next, bool asServer)
        {
            if (asServer) return;
            PlayerRegistry.Register(next, this);
        }



    }
