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
        public readonly SyncList<string> cardStallsOnTable = new SyncList<string>();
        

        /*[Server]
        private void AddNewCardStall(string id)
        {
            cardStallsOnTable.Add(id);
        }*/

        /*public override void OnStartServer()
        {
            base.OnStartServer();
            SetCardStallsOnTableByDataBase();
        }*/

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                SetUiChangeCardStalls();
            }
        }

        private void OnDestroy()
        {
            if (IsOwner)
            {
                DestroyCardStall();
            }
        }

        private void SetUiChangeCardStalls()
        {
            cardStallsOnTable.OnChange += OnCardStallsOnTableUpdated;
            
        }

        private void DestroyCardStall()
        {
            cardStallsOnTable.OnChange -= OnCardStallsOnTableUpdated;
        }


        private void OnCardStallsOnTableUpdated(SyncListOperation op, int index, string olditem, string newitem,
            bool asserver)
        {
            if (asserver) return;
            var uiManager = UIManager.Instance;
            var uiCardShop = uiManager.uiCardShop;
            switch (op)
            {
                case SyncListOperation.Add:
                    var cardStall = CardStallDataBase.Instance.GetCardStall(newitem);
                    uiCardShop.AddNewStall(cardStall);

                    break;

                case SyncListOperation.Complete:
                    break;
            }
        }




        [Server]
        public void SetCardStallsOnTableByDataBase()
        {
            foreach (var cardStall in CardStallDataBase.Instance.GetAllCardsStall())
            {
                cardStallsOnTable.Add(cardStall);
            }
            Debug.Log("SyncList des cartes sur la table mis a jour");
        }
    }
}