using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using QFramework;
public class WildButton : MonoBehaviour, IPointerClickHandler, IController
{
    public CardColour cardColour;

    public IArchitecture GetArchitecture()
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var model = this.GetModel<CardGameModel>();
        model.currentCardColour = cardColour;
        this.SendCommand<ChosenColourCommand>();
    }

    public void SetImageColour(Color32 colour)
    {
        GetComponent<Image>().color = colour;
    }
}