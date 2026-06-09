using UnityEngine;

[CreateAssetMenu(fileName = "SceneCatalog", menuName = "Game/Scene Catalog")]
public class SceneCatalog : ScriptableObject
{
    [SerializeField] private string mainMenu = "MainMenu";
    [SerializeField] private string shop = "Shop";
    [SerializeField] private string planification = "Planification";
    [SerializeField] private string ui = "UI";
    [SerializeField] private string war = "War";
    [SerializeField] private string end = "End";

    public string Shop => shop;
    public string Planification => planification;
    public string UI => ui;
    public string War => war;
    public string End => end;
    public string MainMenu => mainMenu;
}
