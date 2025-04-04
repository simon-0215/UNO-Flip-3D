using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using QFramework;
using UnoFlipV2;
using Unity.VisualScripting;

public class CardDisplayFace2 : MonoBehaviour, IController
{
    #region
    private UnoFlipModelV2 model => this.GetModel<UnoFlipModelV2>();
    #endregion
    #region UI
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
    [Header("Card Back")]
    [SerializeField] GameObject cardBack;

    #endregion
    #region old version
    Card myCard;
    public Card MyCard => myCard;
    Player cardOwner;

    public Player Owner => cardOwner;

    void SetAllColours(CardColour cardColour)
    {
        switch (cardColour)
        {
            case CardColour.RED:
                {
                    baseCardColour.color = CardDisplay.red2;
                    imageCenter.color = CardDisplay.red2;
                }
                break;
            case CardColour.BLUE:
                {
                    baseCardColour.color = CardDisplay.blue2;
                    imageCenter.color = CardDisplay.blue2;
                }
                break;
            case CardColour.GREEN:
                {
                    baseCardColour.color = CardDisplay.green2;
                    imageCenter.color = CardDisplay.green2;
                }
                break;
            case CardColour.YELLOW:
                {
                    baseCardColour.color = CardDisplay.yellow2;
                    imageCenter.color = CardDisplay.yellow2;
                }
                break;
            case CardColour.NONE:
                {
                    baseCardColour.color = CardDisplay.black;
                    imageCenter.color = CardDisplay.black;

                    //WILD CARDS
                    topLeftCenter.color = CardDisplay.red2;
                    bottomLeftCenter.color = CardDisplay.yellow2;
                    topRightCenter.color = CardDisplay.blue2;
                    bottomRightCenter.color = CardDisplay.green2;

                    topLeftTL.color = CardDisplay.red2;
                    bottomLeftTL.color = CardDisplay.yellow2;
                    topRightTL.color = CardDisplay.blue2;
                    bottomRightTL.color = CardDisplay.green2;

                    topLeftBR.color = CardDisplay.red2;
                    bottomLeftBR.color = CardDisplay.yellow2;
                    topRightBR.color = CardDisplay.blue2;
                    bottomRightBR.color = CardDisplay.green2;
                }
                break;
        }
    }

    #endregion

    #region v2 (flip + network)
    //V2
    UnoFlipV2.Card cardData;
    public UnoFlipV2.Card CardData => cardData;

    public void SetCardDataV2(UnoFlipV2.Card data)
    {
        cardData = data;

        SetAllColoursV2();
        SetValueV2();
    }

    private bool showFlip = true;
    public void Flip()
    {
        //showFlip = !showFlip;
        SetAllColoursV2();
        SetValueV2();
    }
    CardSideData getData()
    {
        CardSideData side = showFlip ?
            (model.side == Side.Light ? cardData.dark : cardData.light)
            : (model.side == Side.Light ? cardData.light : cardData.dark);

        return side;
    }
    void SetAllColoursV2()
    {
        //print($"--- model side {model.side} ");
        //print($"--- model side {model.side} {cardData.light.type} {cardData.light.value}");
        CardSideData side = getData();
        SetAllColours((CardColour)side.color);
    }

    void SetValueV2()
    {
        CardSideData side = getData();

        wildImageCenter.SetActive(false);
        wildImageTL.SetActive(false);
        wildImageBR.SetActive(false);
        valueImageCenter.gameObject.SetActive(false);
        valueImageTL.gameObject.SetActive(false);
        valueTextTL.color = Color.white;
        valueImageBR.gameObject.SetActive(false);

        switch (side.type)
        {
            case CardType.Skip:
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
            case CardType.Reverse:
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
            case CardType.Draw:
                {
                    valueImageCenter.sprite = side.value == 2 ? plusTwo : plusFour;
                    valueImageCenter.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "+" + side.value;
                    valueTextBR.text = "+" + side.value;
                }
                break;
            case CardType.Wild:
                {
                    wildImageCenter.SetActive(true);
                    wildImageTL.SetActive(true);
                    wildImageBR.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
            case CardType.WildDraw:
                {
                    wildImageCenter.SetActive(true);
                    wildImageTL.SetActive(true);
                    wildImageBR.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
            case CardType.Flip:
                wildImageCenter.SetActive(false);
                wildImageTL.SetActive(false);
                wildImageBR.SetActive(false);

                valueTextCenter.text = "FLIP";
                valueTextTL.text = "Flip";
                valueTextTL.color = Color.red;
                valueTextBR.text = "";
                break;
            default:
                {
                    valueTextCenter.text = side.value.ToString();
                    valueTextTL.text = side.value.ToString();
                    valueTextBR.text = side.value.ToString();
                }
                break;
        }
    }

    #endregion

    public void ShowCard()
    {
        cardBack.SetActive(false);
    }
    public void HideCard()
    {
        cardBack.SetActive(true);
    }

    public IArchitecture GetArchitecture()
    {
        return UnoFlipAppV2.Interface;
    }
}