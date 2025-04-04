using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using QFramework;
using UnoFlipV2;
public class WildButton : MonoBehaviour, IPointerClickHandler, IController
{
    public CardColor cardColour;

    public IArchitecture GetArchitecture()
    {
        return UnoFlipAppV2.Interface;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var game = this.GetSystem<UnoFlipGameSystemV2>();
        game.WildSetColor(cardColour);
    }

    public void SetImageColour(Color32 colour)
    {
        GetComponent<Image>().color = colour;
    }
}