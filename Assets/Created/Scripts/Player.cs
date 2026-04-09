using FishNet.Object;
using UnityEngine;

namespace Created.Scripts
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Canvas playerCanvas;

        public override void OnStartClient()
        {
            base.OnStartClient();

            playerCanvas.gameObject.SetActive(IsOwner);
        }
    }
}
