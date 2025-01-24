using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{

        [Header("Colours")]
        [SerializeField] Color32 red;
        [SerializeField] Color32 blue;
        [SerializeField] Color32 green;
        [SerializeField] Color32 yellow;
        [SerializeField] Color32 black;
        [Header("Sprites")]
        [SerializeField] Sprite reverse;
        [SerializeField] Sprite skip;
        [SerializeField] Sprite plusTwo;
        [SerializeField] Sprite plusFour;
        [Header("Center Card")]
        [SerializeField] Image baseCardColour; //main colour of the card
        [SerializeField] Image imageCenter; //colour of the center of the card
        [SerializeField] Image valueImageCenter; //skip, reverse, +2, +4
        [SerializeField] TMP_Text valueTextCenter; 
        [SerializeField] GameObject wildImageCenter;
        [SerializeField] Image topLeftCenter;
        [SerializeField] Image bottomLeftCenter;
        [SerializeField] Image topRightCenter;
        [SerializeField] Image bottomRightCenter;
        [Header("Top Left Corner")]
        [SerializeField] Image valueImageTL;
        [SerializeField] TMP_Text valueTextTL;
        [SerializeField] GameObject wildImageTL;
        [SerializeField] Image topLeftTL;
        [SerializeField] Image bottomLeftTL;
        [SerializeField] Image topRightTL;
        [SerializeField] Image bottomRightTL;
        [Header("Bottom Right Corner")]
        [SerializeField] Image valueImageBR;
        [SerializeField] TMP_Text valueTextBR;
        [SerializeField] GameObject wildImageBR;
        [SerializeField] Image topLeftBR;
        [SerializeField] Image bottomLeftBR;
        [SerializeField] Image topRightBR;
        [SerializeField] Image bottomRightBR;

        void SetAllColours(CardColour cardColour)
        {
            switch (cardColour)
            {
                case CardColour.RED:
                {
                    baseCardColour.colour = red;
                    imageCenter.colour = red;
                }
                break;
                case CardColour.BLUE:
                {
                    baseCardColour.colour = blue;
                    imageCenter.colour = blue;
                }
                break;
                case CardColour.GREEN:
                {
                    baseCardColour.colour = green;
                    imageCenter.colour = green;
                }
                break;
                case CardColour.YELLOW:
                {
                    baseCardColour.colour = yellow;
                    imageCenter.colour = yellow;
                }
                break;
                case CardColour.NONE:
                {
                    baseCardColour.colour = black;
                    imageCenter.colour = black;
                }
                break;
            }
        }        

}