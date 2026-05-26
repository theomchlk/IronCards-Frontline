using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public static class LocalPlayerPrefs
{
    public static float MinColorValue, MaxColorValue;

    private static readonly List<string> Names = new()
    {
        "Juan","Anna","Fyl","Ben", "Iron", "Ash", "Kael", "Zyra", "Dusk", 
        "Rook", "Vex", "Nyx", "Bolt", "Cain"
    };

    private static readonly List<string> Titles = new()
        { "TheBreacher", "TheVoid", "TheCalamity", 
        "TheUndying", "TheWarlord", "TheReaper", 
        "TheCrusher", "TheHunter", "ThePhantom",
        "TheTyrant", "TheAvenger", "TheSavage",
        "TheGreat", "TheOutcast"
        };
    
    public static string PlayerName
    {
        get
        {
            if (!PlayerPrefs.HasKey("PlayerName"))
                PlayerName = GenerateRandomName(); // génère et sauvegarde
            return PlayerPrefs.GetString("PlayerName");
        }
        set
        { 
        PlayerPrefs.SetString("PlayerName", value);
        PlayerPrefs.Save();
        Debug.Log("Name player saved");
        }
    }

    public static Color PlayerColor
    {
        get => GetColorPlayer();
        set =>  SaveColorPlayer(value);
    }

    private static Color GetColorPlayer()
    {
        var r = GetColor("ColorR");
        var g = GetColor("ColorG");
        var b = GetColor("ColorB");
        PlayerPrefs.Save();
        return new Color(r, g, b);
    }

    private static float GetColor(string colorName)
    {
        float color;
        if (PlayerPrefs.HasKey(colorName)) color = PlayerPrefs.GetFloat(colorName);
        else
        {
            color = Random.Range(MinColorValue, MaxColorValue);
            PlayerPrefs.SetFloat(colorName, color);
        }
        return color;
    }

    public static Color GetRandomColor()
    {
        var r = Random.Range(MinColorValue, MaxColorValue);
        var g = Random.Range(MinColorValue, MaxColorValue);
        var b = Random.Range(MinColorValue, MaxColorValue);
        var color = new Color(r, g, b);
        SaveColorPlayer(color);
        return color;
        
    }

    private static void SaveColorPlayer(Color color)
    {
        PlayerPrefs.SetFloat("ColorR", color.r);
        PlayerPrefs.SetFloat("ColorG", color.g);
        PlayerPrefs.SetFloat("ColorB", color.b);
        PlayerPrefs.Save();
    }

    public static string GenerateRandomName()
    {
        var i = Random.Range(0, Names.Count);
        var j = Random.Range(0, Titles.Count);
        var name = Names[i] + Titles[j];
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
        return name;
    }
    
    
}
