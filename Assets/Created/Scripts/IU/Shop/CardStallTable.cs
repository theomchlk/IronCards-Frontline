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
        public readonly SyncList<string> cardStallsOnTable = new SyncList<string>();

        void Awake()
        {
            Instance = this;
        }

        /*[Server]
        private void AddNewCardStall(string id)
        {
            cardStallsOnTable.Add(id);
        }*/

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetCardStallsOnTableByDataBase();
        }

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
        

        private void OnCardStallsOnTableUpdated(SyncListOperation op, int index, string olditem, string newitem, bool asserver)
        {
            var uiCardShop = UIManager.Instance.uiCardShop;
            switch(op)
            {
                case SyncListOperation.Add:
                    var cardStall = CardStallDataBase.Instance.GetCardStall(newitem);
                    uiCardShop.AddNewStall(cardStall);

                    break;
                
                case SyncListOperation.Complete:
                    break;
            }
            /*Debug.Log("New cards in the shop");
            if (!IsOwner) return;
                UIManager.Instance.uiCardShop.SetUI()*/
        }

        /*private void _myCollection_OnChange(SyncListOperation op, int index,
            int oldItem, int newItem, bool asServer)
        {
            switch (op)
            {
                /* An object was added to the list. Index
                 * will be where it was added, which will be the end
                 * of the list, while newItem is the value added. #1#
                case SyncListOperation.Add:
                    break;
                /* An object was removed from the list. Index
                 * is from where the object was removed. oldItem
                 * will contain the removed item. #1#
                case SyncListOperation.RemoveAt:
                    break;
                /* An object was inserted into the list. Index
                 * is where the obejct was inserted. newItem
                 * contains the item inserted. #1#
                case SyncListOperation.Insert:
                    break;
                /* An object replaced another. Index
                 * is where the object was replaced. oldItem
                 * is the item that was replaced, while
                 * newItem is the item which now has it's place. #1#
                case SyncListOperation.Set:
                    break;
                /* All objects have been cleared. Index, oldValue,
                 * and newValue are default. #1#
                case SyncListOperation.Clear:
                    break;
                /* When complete calls all changes have been
                 * made to the collection. You may use this
                 * to refresh information in relation to
                 * the list changes, rather than doing so
                 * after every entry change. Like Clear
                 * Index, oldItem, and newItem are all default. #1#
                case SyncListOperation.Complete:
                    break;
            }
        }*/
        
        

        [Server]
        private void SetCardStallsOnTableByDataBase()
        {
            foreach (var cardStall in CardStallDataBase.Instance.GetAllCardsStall())
            {
                cardStallsOnTable.Add(cardStall);
            }
            Debug.Log("SyncList des cartes sur la table mis a jour");
        }
    }
}