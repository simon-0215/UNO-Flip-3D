using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int[] displayInfo = {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
    int boundaryIndex = 0;
    public CardRendering card1;
    public CardRendering card2;
    public CardRendering card3;
    public CardRendering card4;
    public CardRendering card5;
    public CardRendering card6;
    public CardRendering card7;
    public CardRendering card8;
    public void addCard(int cardNum, int cardColor){
        if (boundaryIndex <= 15){
            displayInfo[boundaryIndex] = cardNum;
            displayInfo[boundaryIndex+1] = cardColor;
            boundaryIndex = boundaryIndex + 2;
        }
    }
    public void deleteCard(int index) {
        for (int i = index; i < 7; i++) {
            displayInfo[i*2] = displayInfo[i*2+2];
            displayInfo[i*2+1] = displayInfo[i*2+3];
        }
        displayInfo[14] = -1;
        displayInfo[15] = -1;
        boundaryIndex = boundaryIndex - 2;

    }
    void Start()
    {
        addCard(2,3);
        addCard(3,4);
        addCard(0,1);
        addCard(7,2);
        card1.updateInfo(displayInfo[0],displayInfo[1]);
        card2.updateInfo(displayInfo[2],displayInfo[3]);
        card3.updateInfo(displayInfo[4],displayInfo[5]);
        card4.updateInfo(displayInfo[6],displayInfo[7]);
        card5.updateInfo(displayInfo[8],displayInfo[9]);
        card6.updateInfo(displayInfo[10],displayInfo[11]);
        card7.updateInfo(displayInfo[12],displayInfo[13]);
        card8.updateInfo(displayInfo[14],displayInfo[15]);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        card1.updateInfo(displayInfo[0],displayInfo[1]);
        card2.updateInfo(displayInfo[2],displayInfo[3]);
        card3.updateInfo(displayInfo[4],displayInfo[5]);
        card4.updateInfo(displayInfo[6],displayInfo[7]);
        card5.updateInfo(displayInfo[8],displayInfo[9]);
        card6.updateInfo(displayInfo[10],displayInfo[11]);
        card7.updateInfo(displayInfo[12],displayInfo[13]);
        card8.updateInfo(displayInfo[14],displayInfo[15]);
        
    }
}
