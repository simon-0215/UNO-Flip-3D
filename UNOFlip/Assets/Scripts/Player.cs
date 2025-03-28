using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

[System.Serializable]
public class Player: IController
{
    public string playerName;
    public List<Card> playerHand;
    public bool IsHuman { get; private set; }
    public bool IsHost { get; private set; }

    public Player(string name, bool isHuman, bool isHost=true)
    {
        playerName = name;
        playerHand = new List<Card>();
        IsHuman = isHuman;
        IsHost = isHost;
    }

    public void DrawCard(Card card)
    {
        playerHand.Add(card);
    }

    public void PlayCard(Card card)
    {
        playerHand.Remove(card);
    }

    public virtual void TakeTurn(Card topCard, CardColour topColour)
    {
        //HUMAN PLAYER
    }

    public IArchitecture GetArchitecture()
    {
        return CardGameApp.Interface;
    }
}
