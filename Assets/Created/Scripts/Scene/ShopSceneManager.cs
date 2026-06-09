using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class ShopSceneManager : MonoBehaviour
{
    [SerializeField] private Canvas shopCanvas;

    private void Start()
    {
        InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoadEnd;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoadEnd;
    }

    private void OnSceneLoadEnd(SceneLoadEndEventArgs args)
    {
        if (args.LoadedScenes == null) return;
        foreach (var scene in args.LoadedScenes)
        {
            if (scene.name == GameStateController.Instance.Scenes.War)
            {
                shopCanvas.gameObject.SetActive(false);
                return;
            }
        }
    }
}
