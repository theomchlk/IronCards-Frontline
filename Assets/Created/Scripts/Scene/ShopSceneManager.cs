using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class ShopSceneManager : MonoBehaviour
{
    [SerializeField] private Canvas shopCanvas;

    private void OnEnable()
    {
        InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoadEnd;
    }

    private void OnDisable()
    {
        InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoadEnd;
    }

    private void OnSceneLoadEnd(SceneLoadEndEventArgs args)
    {
        if (args.LoadedScenes == null) return;
        foreach (var scene in args.LoadedScenes)
        {
            if (scene.name == "Noah")
            {
                shopCanvas.gameObject.SetActive(false);
                return;
            }
        }
    }
}
