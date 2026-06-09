using UnityEngine;

/// <summary>
/// Source unique des noms de scènes du jeu. Évite les noms en dur dispersés
/// dans le code. Assigné sur le GameStateController et lu via
/// GameStateController.Instance.Scenes.
/// </summary>
[CreateAssetMenu(fileName = "SceneCatalog", menuName = "Game/Scene Catalog")]
public class SceneCatalog : ScriptableObject
{
    [SerializeField] private string shop = "Shop";
    [SerializeField] private string ui = "UI";
    [SerializeField] private string war = "War";

    public string Shop => shop;
    public string UI => ui;
    public string War => war;
}
