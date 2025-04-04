using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnoFlipV2;

public class CardInteractionV2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IController
{
    CardDisplay cardDisplay;
    Vector3 originalPosition;
    float liftAmount = 30f;

    bool canBeClick
    {
        get
        {
            return cardDisplay.RoleIdx == model.initData.myIdx;//自己的牌才可以点击
        }
    }

    UnoFlipModelV2 model;
    UnoFlipGameSystemV2 game;

    public IArchitecture GetArchitecture()
    {
        return UnoFlipAppV2.Interface;
    }
    
    void Start()
    {
        model = this.GetModel<UnoFlipModelV2>();
        game = this.GetSystem<UnoFlipGameSystemV2>();

        cardDisplay = GetComponent<CardDisplay>();
        originalPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LiftCard(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LiftCard(false);
    }

    void LiftCard(bool lift)
    {
        if (lift && canBeClick)
        {
            transform.localPosition = originalPosition + new Vector3(0, liftAmount, 0);
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("card clicked " + cardDisplay.CardData);
        game.CardClicked(cardDisplay.CardData, cardDisplay);
    }
}