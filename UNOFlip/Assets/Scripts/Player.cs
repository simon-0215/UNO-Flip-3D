using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public string playerName;
    public List<Card> playerHand;
    public bool IsHuman { get; private set; }

    public Player(string name, bool isHuman)
    {
        playerName = name;
        playerHand = new List<Card>();
        IsHuman = isHuman;
    }

    public void DrawCard(Card card)
    {
        playerHand.Add(card);
    }

    public void PlayCard(Card card)
    {
        playerHand.Remove(card);
    }
}
