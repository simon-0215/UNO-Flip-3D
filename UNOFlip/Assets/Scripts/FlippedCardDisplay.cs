// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.UI;
// using UnityEngine;
// using TMPro;

// public class FlippedCardDisplay : MonoBehaviour
// {
//     [Header("Colors")]
//     [SerializeField] Color32 red;
//     [SerializeField] Color32 blue;
//     [SerializeField] Color32 green;
//     [SerializeField] Color32 yellow;
//     [SerializeField] Color32 black;
//     [Header("Sprites")]
//     [SerializeField] Sprite reverse;
//     [SerializeField] Sprite skip;
//     [SerializeField] Sprite plusTwo;
//     [SerializeField] Sprite plusFour;
//     [Header("Center Card")]
//     [SerializeField] Image baseCardColour; //main colour of the card
//     [SerializeField] Image imageCenter; //colour of the center of the card
//     [SerializeField] Image valueImageCenter; //skip, reverse, +2, +4
//     [SerializeField] TMP_Text valueTextCenter; 
//     [SerializeField] GameObject wildImageCenter;
//     [SerializeField] Image topLeftCenter;
//     [SerializeField] Image bottomLeftCenter;
//     [SerializeField] Image topRightCenter;
//     [SerializeField] Image bottomRightCenter;
//     [Header("Top Left Corner")]
//     [SerializeField] Image valueImageTL;
//     [SerializeField] TMP_Text valueTextTL;
//     [SerializeField] GameObject wildImageTL;
//     [SerializeField] Image topLeftTL;
//     [SerializeField] Image bottomLeftTL;
//     [SerializeField] Image topRightTL;
//     [SerializeField] Image bottomRightTL;
//     [Header("Bottom Right Corner")]
//     [SerializeField] Image valueImageBR;
//     [SerializeField] TMP_Text valueTextBR;
//     [SerializeField] GameObject wildImageBR;
//     [SerializeField] Image topLeftBR;
//     [SerializeField] Image bottomLeftBR;
//     [SerializeField] Image topRightBR;
//     [SerializeField] Image bottomRightBR;
//     [Header("Card Back")]
//     [SerializeField] GameObject cardBack;


//     // void OnValidate()
//     // {
//     //     //SetAllColours(CardColour.RED);
//     //     //SetAllColours(CardColour.BLUE);
//     //     //SetAllColours(CardColour.GREEN);
//     //     SetAllColours(CardColour.YELLOW);
//     //     //SetAllColours(CardColour.NONE);
//     // }

//     Card myCard;
//     public Card MyCard => myCard;
//     Player cardOwner;
//     public Player Owner => cardOwner;

//     public void SetCard(Card card, Player owner)
//     {
//         var Colours = GameManager.instance.GetColours();
//         red = Colours.red;
//         blue = Colours.blue;
//         green = Colours.green;
//         yellow = Colours.yellow;
//         black = Colours.black;

//         myCard = card;
//         SetAllColours(card.cardColour);
//         SetValue(card.cardValue);
//         cardOwner = owner;
//     }
    
//     void SetAllColours(CardColour cardColour)
//     {
//         switch (cardColour)
//         {
//             case CardColour.RED:
//             {
//                 baseCardColour.color = red;
//                 imageCenter.color = red;
//             }
//             break;
//             case CardColour.BLUE:
//             {
//                 baseCardColour.color = blue;
//                 imageCenter.color = blue;
//             }
//             break;
//             case CardColour.GREEN:
//             {
//                 baseCardColour.color = green;
//                 imageCenter.color = green;
//             }
//             break;
//             case CardColour.YELLOW:
//             {
//                 baseCardColour.color = yellow;
//                 imageCenter.color = yellow;
//             }
//             break;
//             case CardColour.NONE:
//             {
//                 baseCardColour.color = black;
//                 imageCenter.color = black;

//                 //WILD CARDS
//                 topLeftCenter.color = red;
//                 bottomLeftCenter.color = yellow;
//                 topRightCenter.color = blue;
//                 bottomRightCenter.color = green;

//                 topLeftTL.color = red;
//                 bottomLeftTL.color = yellow;
//                 topRightTL.color = blue;
//                 bottomRightTL.color = green;

//                 topLeftBR.color = red;
//                 bottomLeftBR.color = yellow;
//                 topRightBR.color = blue;
//                 bottomRightBR.color = green;
//             }
//             break;

            
//         }
//     }        

//     void SetValue(CardValue CardValue)
//     {
//         //DEACTIVATE SPECIAL CARDS
//         wildImageCenter.SetActive(false);
//         wildImageTL.SetActive(false);
//         wildImageBR.SetActive(false);
//         valueImageCenter.gameObject.SetActive(false);
//         valueImageTL.gameObject.SetActive(false);
//         valueImageBR.gameObject.SetActive(false);
//         switch (CardValue)
//         {
//             case CardValue.SKIP:
//             {
//                 valueImageCenter.sprite = skip;
//                 valueImageCenter.gameObject.SetActive(true);
//                 valueImageTL.sprite = skip;
//                 valueImageTL.gameObject.SetActive(true);
//                 valueImageBR.sprite = skip;
//                 valueImageBR.gameObject.SetActive(true);
//                 valueTextCenter.text = "";
//                 valueTextTL.text = "";
//                 valueTextBR.text = "";
//             }
//             break;
//             case CardValue.REVERSE:
//             {
//                 valueImageCenter.sprite = reverse;
//                 valueImageCenter.gameObject.SetActive(true);
//                 valueImageTL.sprite = reverse;
//                 valueImageTL.gameObject.SetActive(true);
//                 valueImageBR.sprite = reverse;
//                 valueImageBR.gameObject.SetActive(true);
//                 valueTextCenter.text = "";
//                 valueTextTL.text = "";
//                 valueTextBR.text = "";
//             }
//             break;
//             case CardValue.PLUS_TWO:
//             {
//                 valueImageCenter.sprite = plusTwo;
//                 valueImageCenter.gameObject.SetActive(true);
//                 valueTextCenter.text = "";
//                 valueTextTL.text = "+2";
//                 valueTextBR.text = "+2";
//             }
//             break;
//             case CardValue.PLUS_FOUR:
//             {
//                 valueImageCenter.sprite = plusFour;
//                 valueImageCenter.gameObject.SetActive(true);
//                 valueTextCenter.text = "";
//                 valueTextTL.text = "+4";
//                 valueTextBR.text = "+4";
//             }
//             break;
//             case CardValue.WILD:
//             {
//                 wildImageCenter.SetActive(true);
//                 wildImageTL.SetActive(true);
//                 wildImageBR.SetActive(true);
//                 valueTextCenter.text = "";
//                 valueTextTL.text = "";
//                 valueTextBR.text = "";
//             }
//             break;
//             default:
//             {
//                 valueTextCenter.text = ((int)CardValue).ToString();
//                 valueTextTL.text = ((int)CardValue).ToString();
//                 valueTextBR.text = ((int)CardValue).ToString();
//             }
//             break;
//         }
//     }

//     public void ShowCard()
//     {
//         cardBack.SetActive(false);
//     }
//     public void HideCard()
//     {
//         cardBack.SetActive(true);
//     }

// }