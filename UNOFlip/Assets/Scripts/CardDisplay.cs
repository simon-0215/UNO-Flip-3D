using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using QFramework;
using UnoFlipV2;

public class CardDisplay : MonoBehaviour, IController
{
    #region
    private UnoFlipModelV2 model => this.GetModel<UnoFlipModelV2>();
    
    //light side color
    public static Color32 red;
    public static Color32 blue;
    public static Color32 green;
    public static Color32 yellow;
    public static Color32 black;

    //dark side color
    public static Color32 redDark;
    public static Color32 blueDark;
    public static Color32 greenDark;
    public static Color32 yellowDark;

    [HideInInspector] public static Color32 red1, red2;
    [HideInInspector] public static Color32 blue1, blue2;
    [HideInInspector] public static Color32 green1, green2;
    [HideInInspector] public static Color32 yellow1, yellow2;

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

    // void OnValidate()
    // {
    //     //SetAllColours(CardColour.RED);
    //     //SetAllColours(CardColour.BLUE);
    //     //SetAllColours(CardColour.GREEN);
    //     SetAllColours(CardColour.YELLOW);
    //     //SetAllColours(CardColour.NONE);
    // }
    #endregion
    #region old version
    Card myCard;
    public Card MyCard => myCard;
    Player cardOwner;

    public Player Owner => cardOwner;

    public void SetCard(Card card, Player owner)
    {
        var Colours = this.GetModel<CardGameModel>().Colours;
        red = Colours.red;
        blue = Colours.blue;
        green = Colours.green;
        yellow = Colours.yellow;
        black = Colours.black;

        myCard = card;

        //if (owner != null && owner.IsHuman)
        //    print($"set card {card.cardColour} {card.cardValue} {red}");

        SetAllColours(card.cardColour);
        SetValue(card.cardValue);
        cardOwner = owner;
    }
    
    void SetAllColours(CardColour cardColour)
    {
        switch (cardColour)
        {
            case CardColour.RED:
            {
                baseCardColour.color = red1;
                imageCenter.color = red1;
            }
            break;
            case CardColour.BLUE:
            {
                baseCardColour.color = blue1;
                imageCenter.color = blue1;
            }
            break;
            case CardColour.GREEN:
            {
                baseCardColour.color = green1;
                imageCenter.color = green1;
            }
            break;
            case CardColour.YELLOW:
            {
                baseCardColour.color = yellow1;
                imageCenter.color = yellow1;
            }
            break;
            case CardColour.NONE:
            {
                baseCardColour.color = black;
                imageCenter.color = black;

                //WILD CARDS
                topLeftCenter.color = red1;
                bottomLeftCenter.color = yellow1;
                topRightCenter.color = blue1;
                bottomRightCenter.color = green1;

                topLeftTL.color = red1;
                bottomLeftTL.color = yellow1;
                topRightTL.color = blue1;
                bottomRightTL.color = green1;

                topLeftBR.color = red1;
                bottomLeftBR.color = yellow1;
                topRightBR.color = blue1;
                bottomRightBR.color = green1;
            }
            break;
        }
    }        

    void SetValue(CardValue CardValue)
    {
        //DEACTIVATE SPECIAL CARDS
        wildImageCenter.SetActive(false);
        wildImageTL.SetActive(false);
        wildImageBR.SetActive(false);
        valueImageCenter.gameObject.SetActive(false);
        valueImageTL.gameObject.SetActive(false);
        valueImageBR.gameObject.SetActive(false);
        switch (CardValue)
        {
            case CardValue.SKIP:
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
            case CardValue.REVERSE:
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
            case CardValue.PLUS_TWO:
            {
                valueImageCenter.sprite = plusTwo;
                valueImageCenter.gameObject.SetActive(true);
                valueTextCenter.text = "";
                valueTextTL.text = "+2";
                valueTextBR.text = "+2";
            }
            break;
            case CardValue.PLUS_FOUR:
            {
                valueImageCenter.sprite = plusFour;
                valueImageCenter.gameObject.SetActive(true);
                valueTextCenter.text = "";
                valueTextTL.text = "+4";
                valueTextBR.text = "+4";
            }
            break;
            case CardValue.WILD:
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
                valueTextCenter.text = ((int)CardValue).ToString();
                valueTextTL.text = ((int)CardValue).ToString();
                valueTextBR.text = ((int)CardValue).ToString();
            }
            break;
        }
    }
    #endregion

    #region v2 (flip + network)
    //V2
    UnoFlipV2.Card cardData;
    public UnoFlipV2.Card CardData => cardData;
    int roleIdx;
    public int RoleIdx => roleIdx;

    CardDisplayFace2 face2 = null;

    void updateColor()
    {
        red1 = model.side == Side.Light ? red : redDark;
        blue1 = model.side == Side.Light ? blue : blueDark;
        green1 = model.side == Side.Light ? green : greenDark;
        yellow1 = model.side == Side.Light ? yellow : yellowDark;

        red2 = model.side == Side.Dark ? red : redDark;
        blue2 = model.side == Side.Dark ? blue : blueDark;
        green2 = model.side == Side.Dark ? green : greenDark;
        yellow2 = model.side == Side.Dark ? yellow : yellowDark;
    }

    public void SetCardDataV2(UnoFlipV2.Card data, int roleIdx)
    {
        if (face2 == null)
        {
            face2 = GetComponentInChildren<CardDisplayFace2>();
            updateColor();
        }

        this.roleIdx = roleIdx;
        cardData = data;

        SetAllColoursV2();
        SetValueV2();

        face2.SetCardDataV2(data);
        face2.ShowCard();
    }

    public void HideFace2()
    {
        if (face2 == null)
            face2 = GetComponentInChildren<CardDisplayFace2>();

        face2.gameObject.SetActive(false);
    }

    public void Flip()
    {
        updateColor();
        SetAllColoursV2();
        SetValueV2();

        face2.Flip();
    }

    void SetAllColoursV2()
    {
        //print($"--- model side {model.side} ");
        //print($"--- model side {model.side} {cardData.light.type} {cardData.light.value}");
        CardSideData side = model.side == Side.Light ? cardData.light : cardData.dark;
        SetAllColours((CardColour)side.color);
    }

    void SetValueV2()
    {
        CardSideData side = model.side == Side.Light ? cardData.light : cardData.dark;

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
                    valueTextTL.text = "Wild";
                    valueTextBR.text = "";
                }
                break;
            case CardType.WildDraw:
                {
                    wildImageCenter.SetActive(true);
                    wildImageTL.SetActive(true);
                    wildImageBR.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "WD";
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

    [SerializeField]
    private bool debugMode = false;
    [SerializeField]
    private bool isFlip = false;

    private void OnEnable()
    {
        if (debugMode)
        {
            // Ã÷ÁÁÑÕÉ«  
            red = new Color32(255, 0, 0, 255);       // ÁÁºì  
            blue = new Color32(0, 0, 255, 255);      // ÁÁÀ¶  
            green = new Color32(0, 255, 0, 255);     // ÁÁÂÌ  
            yellow = new Color32(255, 255, 0, 255);  // ÁÁ»Æ  
            black = new Color32(0, 0, 0, 255);       // ´¿ºÚ  

            // °µÉ«°æ±¾  
            redDark = new Color32(150, 0, 0, 255);      // °µºì  
            blueDark = new Color32(0, 0, 150, 255);     // °µÀ¶  
            greenDark = new Color32(0, 100, 0, 255);    // °µÂÌ  
            yellowDark = new Color32(150, 150, 0, 255); // °µ»Æ  

            CardSideData dark = new CardSideData(CardType.Number, CardColor.RED, 3);
            CardSideData light = new CardSideData(CardType.Skip, CardColor.BLUE, 1);

            UnoFlipV2.Card card = new UnoFlipV2.Card(light, dark);
            SetCardDataV2(card, 0);
            ShowCard();

            if (isFlip)
            {
                model.side = Side.Dark;
                Flip();
            }
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
        //return CardGameApp.Interface;
        return UnoFlipAppV2.Interface;
    }
}