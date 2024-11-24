using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CardRendering : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int cardNumInput;
    public int cardColorInput;
    public static int cardNumInputS;
    public static int cardColorInputS;
    public Text numTextTop;
    public Text numTextBottom;
    public Text numTextMiddle;
    public Image backgroundColor;
    public Image blockImage;
    public void updateInfo(int num, int color) {
        cardNumInput = num;
        cardColorInput = color;
    }
    void Start()
    {
        cardColorInputS = cardColorInput;
        cardNumInputS = cardNumInput;
        numTextTop.text = " " + cardNumInput;
        numTextBottom.text = " " + cardNumInput;
        numTextMiddle.text = " " + cardNumInput;
        if (cardColorInput == 1) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(1f,0f,0f);
        }
        else if (cardColorInput == 2) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(1f,0.5f,0f);
        }
        else if (cardColorInput == 3) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(0f,1f,0f);
        }
        else if (cardColorInput == 4) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(0.3f,0.5f,1f);
        }
        else if (cardColorInput == -1) {
            blockImage.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        cardColorInputS = cardColorInput;
        cardNumInputS = cardNumInput;
        numTextTop.text = " " + cardNumInput;
        numTextBottom.text = " " + cardNumInput;
        numTextMiddle.text = " " + cardNumInput;
        if (cardColorInput == 1) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(1f,0f,0f);
        }
        else if (cardColorInput == 2) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(1f,0.5f,0f);
        }
        else if (cardColorInput == 3) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(0f,1f,0f);
        }
        else if (cardColorInput == 4) {
            blockImage.enabled = false;
            backgroundColor.color = new Color(0.3f,0.5f,1f);
        }
        else if (cardColorInput == -1) {
            blockImage.enabled = true;
        }
    }
}
