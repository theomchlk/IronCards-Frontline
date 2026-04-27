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
        public readonly SyncList<string> cardStallsOnTable = new ();

        void Awake()
        {
            Instance = this;
        }

        [Server]
        private void AddNewCardStall(string id)
        {
            cardStallsOnTable.Add(id);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetStallsByDataBase();
        }

        private void OnChangeCardStallsOnTable()
        {
            
        }
        

        [Server]
        private void SetStallsByDataBase()
        {
            foreach (var cardStall in CardStallDataBase.Instance.GetAllCardsStall())
            {
                cardStallsOnTable.Add(cardStall);
            }
        }
    }
}