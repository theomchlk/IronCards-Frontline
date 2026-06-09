using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class ShopSceneManager : MonoBehaviour
{
    [SerializeField] private Canvas shopCanvas;

    private void Start()
    {
        InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoadEnd;
        InstanceFinder.SceneManager.OnUnloadEnd += OnSceneUnloadEnd;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager == null) return;
        InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoadEnd;
        InstanceFinder.SceneManager.OnUnloadEnd -= OnSceneUnloadEnd;
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

    private void OnSceneUnloadEnd(SceneUnloadEndEventArgs args)
    {
        if (args.UnloadedScenesV2 == null) return;
        foreach (var scene in args.UnloadedScenesV2)
        {
            if (scene.Name == GameStateController.Instance.Scenes.War)
            {
                shopCanvas.gameObject.SetActive(true);
                return;
            }
        }
    }
}
