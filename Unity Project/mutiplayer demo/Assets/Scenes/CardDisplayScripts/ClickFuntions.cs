using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickFuntion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public MainControl cardList;
    public CardRendering currentCard;
    public void clickUnknownCard() {
        int ramdomNumber = Random.Range(0,10);
        int randomColor = Random.Range(1,5);
        cardList.addCard(ramdomNumber,randomColor);
    }
    public void clickCardOnDeck(int position) {
        int num = cardList.getNumOnList(position*2);
        int color = cardList.getNumOnList(position*2+1);
        if (num > currentCard.cardNumInput || color == currentCard.cardColorInput) {
            currentCard.updateInfo(num, color);
            cardList.deleteCard(position);
        }
    }
    public void getCard() {
        int num = currentCard.cardNumInput + 1;
        int color = Random.Range(1,5);
        currentCard.updateInfo(num, color);
    }
    public void clickReset() {
        cardList.resetData();
        currentCard.cardNumInput = 0;
        currentCard.cardColorInput = 1;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
