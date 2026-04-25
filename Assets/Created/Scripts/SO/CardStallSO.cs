using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

[CreateAssetMenu(fileName = "CardStallSO", menuName = "CardStallSO")]
public class CardStallSO : ScriptableObject
{
    public string cardStallID;
    public string cardStallName;
    public Color cardStallColor;
    public List<CardsSO> cardsSo;
    public GameObject cardStallPrefab;


}
