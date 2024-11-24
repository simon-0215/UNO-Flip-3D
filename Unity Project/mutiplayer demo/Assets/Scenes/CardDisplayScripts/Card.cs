using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Card : MonoBehaviour
{
    public int id;
    public int cardNum;
    public int cardColor;
    public int sepcialFeature;

    public Card(){}
    public Card(int Id, int CardNum, int CardColor, int SepcialFeature){
        id = Id;
        cardNum = CardNum;
        cardColor = CardColor;
        sepcialFeature = SepcialFeature;
    }
}
