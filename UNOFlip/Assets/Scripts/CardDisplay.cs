using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    Color32 red, blue, green, yellow, black;
   
    // ➕ Added new colors for UNO Flip Dark Side
    Color32 orange, teal, pink, purple;
 
    [Header("Sprites")]
    [SerializeField] Sprite reverse;
    [SerializeField] Sprite skip;
    [SerializeField] Sprite plusTwo;
    [SerializeField] Sprite plusFour;
 
    // ➕ Added new special card sprites for UNO Flip
    [SerializeField] Sprite plusFive;
    [SerializeField] Sprite skipAll;
    [SerializeField] Sprite pickUntil;
    [SerializeField] Sprite flip;
 
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
 
    [Header("Card Back")]
    [SerializeField] GameObject cardBack;
 
    // ➕ Added a flipped card back for UNO Flip
    [SerializeField] bool isFlipped;


    // void OnValidate()
    // {
    //     //SetAllColours(CardColour.RED);
    //     //SetAllColours(CardColour.BLUE);
    //     //SetAllColours(CardColour.GREEN);
    //     SetAllColours(CardColour.YELLOW);
    //     //SetAllColours(CardColour.NONE);
    // }

    Card myCard;
    public Card MyCard => myCard;
    Player cardOwner;
    public Player Owner => cardOwner;

    public void SetCard(Card card, Player owner) //PINK, TEAL, ORANGE, PURPLE
    {
        var Colours = GameManager.instance.GetColours();
        red = Colours.red;
        blue = Colours.blue;
        green = Colours.green;
        yellow = Colours.yellow;
        black = Colours.black;
        orange = Colours.orange;
        teal = Colours.teal;
        pink = Colours.pink;
        purple = Colours.purple;

        myCard = card;
        SetAllColours(card.lightCardColour, card.darkCardColour);
        SetValue(card.lightCardValue, card.darkCardValue);
        cardOwner = owner;
    }
    
    void SetAllColours(LightCardColour lightCardColour, DarkCardColour darkCardColour)
    {
        if (!isFlipped)
        {
            switch (lightCardColour)
            {
                case LightCardColour.RED:
                {
                    baseCardColour.color = red;
                    imageCenter.color = red;
                }
                break;
                case LightCardColour.BLUE:
                {
                    baseCardColour.color = blue;
                    imageCenter.color = blue;
                }
                break;
                case LightCardColour.GREEN:
                {
                    baseCardColour.color = green;
                    imageCenter.color = green;
                }
                break;
                case LightCardColour.YELLOW:
                {
                    baseCardColour.color = yellow;
                    imageCenter.color = yellow;
                }
                break;
                case LightCardColour.NONE:
                {
                    baseCardColour.color = black;
                    imageCenter.color = black;

                    //WILD CARDS
                    topLeftCenter.color = red;
                    bottomLeftCenter.color = yellow;
                    topRightCenter.color = blue;
                    bottomRightCenter.color = green;

                    topLeftTL.color = red;
                    bottomLeftTL.color = yellow;
                    topRightTL.color = blue;
                    bottomRightTL.color = green;

                    topLeftBR.color = red;
                    bottomLeftBR.color = yellow;
                    topRightBR.color = blue;
                    bottomRightBR.color = green;
                }
                break;
            }
        }
            
        else
        {
            switch (darkCardColour)
            {
                case DarkCardColour.PINK:
                {
                    baseCardColour.color = pink;
                    imageCenter.color = pink;
                }
                break;
                case DarkCardColour.TEAL:
                {
                    baseCardColour.color = teal;
                    imageCenter.color = teal;
                }
                break;
                case DarkCardColour.ORANGE:
                {
                    baseCardColour.color = orange;
                    imageCenter.color = orange;
                }
                break;
                case DarkCardColour.PURPLE:
                {
                    baseCardColour.color = purple;
                    imageCenter.color = purple;
                }
                break;
                case DarkCardColour.NONE:
                {
                    baseCardColour.color = black;
                    imageCenter.color = black;

                    //NEED TO CHANGE BASED ON DESING OF WILD CARD
                    
                    //WILD CARDS
                    topLeftCenter.color = red;
                    bottomLeftCenter.color = yellow;
                    topRightCenter.color = blue;
                    bottomRightCenter.color = green;

                    topLeftTL.color = red;
                    bottomLeftTL.color = yellow;
                    topRightTL.color = blue;
                    bottomRightTL.color = green;

                    topLeftBR.color = red;
                    bottomLeftBR.color = yellow;
                    topRightBR.color = blue;
                    bottomRightBR.color = green;
                }
                break;    
            }
        }

    }        

//NEED TO ADD SPRITES AND CONFIGURE THEM HERE FOR THE DARK SIDE
    void SetValue(LightCardValue lightCardValue, DarkCardValue darkCardValue)
    {
        //DEACTIVATE SPECIAL CARDS
        wildImageCenter.SetActive(false);
        wildImageTL.SetActive(false);
        wildImageBR.SetActive(false);
        valueImageCenter.gameObject.SetActive(false);
        valueImageTL.gameObject.SetActive(false);
        valueImageBR.gameObject.SetActive(false);
        if (!isFlipped)
        {
            switch (lightCardValue)
            {
                case LightCardValue.SKIP:
                {
                    valueImageCenter.sprite = skip;
                    valueImageCenter.gameObject.SetActive(true);
                    valueImageTL.sprite = skip;
                    valueImageTL.gameObject.SetActive(true);
                    valueImageBR.sprite = skip;
                    valueImageBR.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
                case LightCardValue.REVERSE:
                {
                    valueImageCenter.sprite = reverse;
                    valueImageCenter.gameObject.SetActive(true);
                    valueImageTL.sprite = reverse;
                    valueImageTL.gameObject.SetActive(true);
                    valueImageBR.sprite = reverse;
                    valueImageBR.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
                case LightCardValue.PLUS_TWO:
                {
                    valueImageCenter.sprite = plusTwo;
                    valueImageCenter.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "+2";
                    valueTextBR.text = "+2";
                }
                break;
                case LightCardValue.PLUS_FOUR:
                {
                    valueImageCenter.sprite = plusFour;
                    valueImageCenter.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "+4";
                    valueTextBR.text = "+4";
                }
                break;
                case LightCardValue.WILD:
                {
                    wildImageCenter.SetActive(true);
                    wildImageTL.SetActive(true);
                    wildImageBR.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
                default:
                {
                    valueTextCenter.text = ((int)lightCardValue).ToString();
                    valueTextTL.text = ((int)lightCardValue).ToString();
                    valueTextBR.text = ((int)lightCardValue).ToString();
                }
                break;
            }
        }
        else
        {
            switch (darkCardValue)
            {
                case DarkCardValue.DRAW_FIVE:
                {
                    valueImageCenter.sprite = skip;
                    valueImageCenter.gameObject.SetActive(true);
                    valueImageTL.sprite = skip;
                    valueImageTL.gameObject.SetActive(true);
                    valueImageBR.sprite = skip;
                    valueImageBR.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
                case DarkCardValue.SKIP_EVERYONE:
                {
                    valueImageCenter.sprite = reverse;
                    valueImageCenter.gameObject.SetActive(true);
                    valueImageTL.sprite = reverse;
                    valueImageTL.gameObject.SetActive(true);
                    valueImageBR.sprite = reverse;
                    valueImageBR.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
                case DarkCardValue.WILD_DRAW_COLOR:
                {
                    valueImageCenter.sprite = plusTwo;
                    valueImageCenter.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "+2";
                    valueTextBR.text = "+2";
                }
                break;
                default:
                {
                    valueTextCenter.text = ((int)lightCardValue).ToString();
                    valueTextTL.text = ((int)lightCardValue).ToString();
                    valueTextBR.text = ((int)lightCardValue).ToString();
                }
                break;
            }
        }
    }

    public void ShowCard()
    {
        cardBack.SetActive(false);
    }
    public void FlipCard()
    {
        if (isFlipped == true)
        {
            isFlipped = false;
        }
        else
        {
            isFlipped = true;
        }
    }
    public void HideCard()
    {
        cardBack.SetActive(true);
    }
    public void DiplayFlipped()
    {
        cardBack.SetActive(true);
    }

}