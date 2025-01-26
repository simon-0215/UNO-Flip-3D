using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class WildButton : MonoBehaviour, IPointerClickHandler
{
    public CardColour cardColour;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.ChosenColour(cardColour);
    }

    public void SetImageColour(Color32 colour)
    {
        GetComponent<Image>().color = colour;
    }
}