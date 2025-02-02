using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public Player(string name, bool isHuman)
    {
        playerName = name;
        playerHand = new List<Card>();
        IsHuman = isHuman;
    }

    [SyncVar] public string playerName;
    public List<Card> playerHand = new List<Card>();

    public bool IsHuman { get; private set; }

    public void DrawCard(Card card)
    {
        playerHand.Add(card);
    }


    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            playerName = "Player " + connectionToClient.connectionId;
        }
    }

    [Command]
    void CmdDrawCard()
    {
        if (!isServer) return; // 只有服务器执行

        Card drawnCard = GameManager.instance.DrawCardFromDeck();
        if (drawnCard == null) return;

        RpcDrawCard(drawnCard); // 让所有客户端同步手牌
    }





    public void PlayCard(Card card)
    {
        playerHand.Remove(card);
    }

    public virtual void TakeTurn(Card topCard, CardColour topColour)
    {
        // 仅供子类实现
    }
    [ClientRpc]
    public void RpcDrawCard(Card card)
    {
        if (!isClient) return; // 确保只在客户端执行

        playerHand.Add(card);
    }



}
