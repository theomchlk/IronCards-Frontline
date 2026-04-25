using System;
using UnityEngine;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace Created.Scripts.IU.Shop
{
    public class CardStallTable : NetworkBehaviour
    {
        public static CardStallTable Instance;
        
        [SerializeField] private List<CardStallSO> cardStallsByDefault;
        public readonly SyncList<CardStallSO> cardStallsOnTable;

        void Awake()
        {
            Instance = this;
        }

        [Server]
        private void AddNewCardStall(string id)
        {
            var cardStall = CardStallDataBase.Instance.GetCardStall(id);
            cardStallsOnTable.Add(cardStall);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetAllDefaultStalls();
        }
        

        [Server]
        private void SetAllDefaultStalls()
        {
            foreach (var cardStall in cardStallsByDefault)
            {
                cardStallsOnTable.Add(cardStall);
            }
        }
    }
}