using MyTcpClient;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IController
{
    CardDisplay cardDisplay;
    Vector3 originalPosition;
    float liftAmount = 30f;

    CardGameModel model;
    void Start()
    {
        model = this.GetModel<CardGameModel>();

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(cardDisplay.Owner.IsHuman && model.myTurn && cardDisplay.Owner.IsHost == model.isHost)
        {
            //PLAY THE CARD
            Debug.Log("clicked a: " + cardDisplay.MyCard.cardColour.ToString() + cardDisplay.MyCard.cardValue.ToString());
            LiftCard(false);

            //if (model.isHost)
            {
                model.currentCardDisplay = cardDisplay;
                model.currentCard = null;
                this.SendCommand<PlayCardCommand>();
            }         
        }
        
    }

    void LiftCard(bool lift)
    {
        if (lift && cardDisplay.Owner.IsHuman && cardDisplay.Owner.IsHost == model.isHost)
        {
            transform.localPosition = originalPosition + new Vector3(0,liftAmount,0);
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }

    public IArchitecture GetArchitecture()
    {
        return CardGameApp.Interface;
    }
}