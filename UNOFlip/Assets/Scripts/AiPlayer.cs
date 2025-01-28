using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiPlayer : Player
{
    public AiPlayer(string name) : base(name, false)
    {

    }

    //AI LOGIC
    public override void TakeTurn(Card topCard, CardColour topColour)
    {
        Card cardToPlay = null;
        Debug.Log(playerName + "AI TURN");

        List<Card> playableCards = GetPlayableCards(topCard, topColour);

        if (playableCards.Count > 0)
        {
            cardToPlay = ChooseBestCard(playableCards);
        }
        else
        {
            GameManager.instance.DrawCardFromDeck();
            playableCards = GetPlayableCards(topCard, topColour);
            if(playableCards.Count > 0)
            {
                cardToPlay = ChooseBestCard(playableCards);
            }
        }

        if (cardToPlay == null)
        {
            Debug.Log(playerName + "HAS NO PLAYABLE CARD - SWITCH");
            GameManager.instance.UpdateMessageBox(playerName + "HAS NO PLAYABLE CARD - SWITCH");
            //GameManager.instance.SwitchPlayer();
            GameManager.instance.AiSwitchPlayer();
        }
        else
        {
            if(playerHand.Count == 2)
            {
                GameManager.instance.SetUnoByAi();
                //MESSAGE FOR PLAYER
                Debug.Log(playerName + "HAS CALLED UNO");
            }
            GameManager.instance.PlayCard(null, cardToPlay);
            //IF WILDCARD CHOOSE BEST COLOUR
            if(cardToPlay.cardColour == CardColour.NONE)
            {
                GameManager.instance.ChosenColour(SelectBestColour());
            }
            //SWITCH PLAYER
            Debug.Log(playerName + "HAS PLAYED - " + cardToPlay.cardColour + cardToPlay.cardValue);
            if(cardToPlay.cardValue == CardValue.SKIP)
            {
                return;
            }
            //ELSE
            //GameManager.instance.SwitchPlayer();
            GameManager.instance.AiSwitchPlayer();
        }
    }

    List<Card> GetPlayableCards(Card topCard, CardColour topColour)
    {
        List<Card> playableCards = new List<Card>();

        foreach (Card card in playerHand)
        {
            if (card.cardColour == topColour|| card.cardValue == topCard.cardValue || card.cardColour == CardColour.NONE)
            {
                playableCards.Add(card);
            }
        }

        return playableCards;
    }

    Card ChooseBestCard(List<Card> playableCards)
    {
        Card bestActionCard = null;
        Card bestRegularCard = null;
        Card bestWildCard = null;
        int nextPlayerHandSize = GameManager.instance.GetNextPlayerHandSize();
    
    //BEST ACTION CARDS
    foreach (Card card in playableCards)
    {
        if (card.cardValue == CardValue.PLUS_FOUR)
        {
            if(nextPlayerHandSize <= 2 || bestActionCard == null)
            {
                bestActionCard = card;
            }

        }
            else if (card.cardValue == CardValue.PLUS_TWO)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if (card.cardValue == CardValue.SKIP)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if (card.cardValue == CardValue.REVERSE)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if (card.cardValue == CardValue.WILD)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestWildCard = card;
                }
            }
        }

        //REGULAR CARDS
        foreach (Card card in playableCards)
        {
            if (bestRegularCard == null || card.cardValue > bestRegularCard.cardValue)
            {
                bestRegularCard = card;
            }
        }
        //NO ACTION CARDS
        if (bestActionCard == null && bestWildCard == null)
        {
            bestActionCard = bestWildCard;
        }
        //MAKE DECISION
        if(bestActionCard != null)
        {
            return bestActionCard;
        }

        if (bestRegularCard != null)
        {
            return bestRegularCard;
        }
        //DEFAULT
        return playableCards[0];
    }

    CardColour SelectBestColour()
    {
        Dictionary<CardColour, int> colourCount = new Dictionary<CardColour, int>
        {
            {CardColour.RED, 0},
            {CardColour.BLUE, 0},
            {CardColour.GREEN, 0},
            {CardColour.YELLOW, 0}
        };

        foreach (Card card in playerHand)
        {
            if (card.cardColour != CardColour.NONE)
            {
                colourCount[card.cardColour]++;
            }
        }

        CardColour bestColour = CardColour.RED;
        int maxCount = 0;
        foreach(var colour in colourCount)
        {
            if (colour.Value > maxCount)
            {
                maxCount = colour.Value;
                bestColour = colour.Key;
            }
        }

        return bestColour;
    }
}