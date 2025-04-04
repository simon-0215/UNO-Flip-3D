using UnityEngine;
using QFramework;
using UnoFlipV2;
using System.Collections.Generic;

public class UnoFlipGameSystemV2 : AbstractSystem
{
    UnoFlipModelV2 model;

    protected override void OnInit()
    {
        model = this.GetModel<UnoFlipModelV2>();
    }

    public void CardClicked(UnoFlipV2.Card cardData, CardDisplay cardDisplay)
    {
        this.SendEvent(new CardClickedEvent()
        {
            cardData = cardData,
            cardDisplay = cardDisplay
        });
    }
    public void WildSetColor(CardColor color)
    {
        this.SendEvent(new WildSetColor() { color = color });
    }

    public void UnoClick()
    {
        this.SendEvent<UnoClickEvent>();
    }
    /// <summary>
    /// �Ƿ�Ϸ����ƣ����ݵ�ǰ��Ϸ״̬�жϣ�
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public bool IsPlayable(UnoFlipV2.Card cardData, int roleIdx = -1)
    {
        CardSideData data = model.side == Side.Light ? cardData.light : cardData.dark;
        /*
            Number,
            //------------------ ������
            Skip,
            Reverse,
            Draw,
            //------------------- ������
            Wild,
            WildDraw,
            Flip,
        */
        
        switch (data.type)
        {
            case CardType.Number:
                return data.color == model.nextColor || data.value == model.lastSideData.value;
            //������
            case CardType.Skip:
            case CardType.Reverse:
            case CardType.Draw:
                return data.color == model.nextColor || data.type == model.lastSideData.type;
            //ȫ����
            case CardType.Wild:
            case CardType.Flip:
                return true;
            //����ȫ����  ֻ���Լ�����û������һ����ͬɫ����ʹ�ã��Ϸ����ƣ�
            case CardType.WildDraw:
                if (model.lastSideData.color == CardColor.NONE) return true;

                int role = roleIdx == -1 ? model.initData.myIdx : roleIdx;
                List<UnoFlipV2.Card> handCards = model.rolesHandCard[role];
                foreach(UnoFlipV2.Card card in handCards)
                {
                    CardSideData _cardData = model.side == Side.Light ? card.light : card.dark;
                    if (_cardData.color == model.lastSideData.color)
                        return false;
                }
                return true;
        }

        Debug.LogError($"wrong card type {data.type}");
        return false;
    }
}


public struct WildSetColor
{
    public CardColor color;
}
