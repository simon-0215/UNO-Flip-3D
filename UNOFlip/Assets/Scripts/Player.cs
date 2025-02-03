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

    [Command]
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
        if (!isServer) return; // ֻ�з�����ִ��

        Card drawnCard = GameManager.instance.DrawCardFromDeck();
        if (drawnCard == null) return;

        RpcDrawCard(drawnCard); // �����пͻ���ͬ������
    }

    
    public void PlayCard(Card card)
    {
        playerHand.Remove(card);
    }

    public virtual void TakeTurn(Card topCard, CardColour topColour)
    {
        // ��������ʵ��
    }
    [ClientRpc]
    public void RpcDrawCard(Card card)
    {
        if (!isClient) return; // ȷ��ֻ�ڿͻ���ִ��

        playerHand.Add(card);
    }



}
