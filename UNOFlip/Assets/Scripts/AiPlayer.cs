using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class AiPlayer : Player
{
    public AiPlayer(string name) : base(name, false)
    {

    }

    // AI LOGIC
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
            // Draw a card and check again
            GameManager.instance.DrawCardFromDeck();
            playableCards = GetPlayableCards(topCard, topColour);
            if (playableCards.Count > 0)
            {
                cardToPlay = ChooseBestCard(playableCards);
            }
        }

        if (cardToPlay == null)
        {
            // No playable card, switch players
            Debug.Log(playerName + "HAS NO PLAYABLE CARD - SWITCH");
            GameManager.instance.UpdateMessageBox(playerName + "HAS NO PLAYABLE CARD - SWITCH");
            DrawCardAndSwitchPlayer();
        }
        else
        {
            // Play the card
            if (playerHand.Count == 2)
            {
                GameManager.instance.SetUnoByAi();
                Debug.Log(playerName + "HAS CALLED UNO");
            }
            PlayCardOnServer(cardToPlay);
        }
    }

    [Server]
    private void PlayCardOnServer(Card card)
    {
        // Server-side logic to play the card
        GameManager.instance.PlayCardWithCard(card);

        // Notify clients about the card played
        RpcNotifyCardPlayed(card);

        // Handle wildcard color selection
        if (card.cardColour == CardColour.NONE)
        {
            CardColour chosenColour = SelectBestColour();
            RpcNotifyChosenColour(chosenColour);
        }

        // Switch players if necessary
        if (card.cardValue != CardValue.SKIP)
        {
            DrawCardAndSwitchPlayer();
        }
    }

    [ClientRpc]
    private void RpcNotifyCardPlayed(Card card)
    {
        // Update clients about the card played
        Debug.Log($"{playerName} played {card.cardColour} {card.cardValue}");
    }

    [ClientRpc]
    private void RpcNotifyChosenColour(CardColour colour)
    {
        // Update clients about the chosen color for wildcards
        Debug.Log($"{playerName} chose {colour}");
    }

    [Server]
    private void DrawCardAndSwitchPlayer()
    {
        // Server-side logic to switch players
        GameManager.instance.AiSwitchPlayer();
        GameManager.instance.UpdatePlayerHighlights();

        // Notify clients about the player switch
        RpcNotifyPlayerSwitch();
    }

    [ClientRpc]
    private void RpcNotifyPlayerSwitch()
    {
        // Update clients about the player switch
        Debug.Log("Player switched");
    }

    List<Card> GetPlayableCards(Card topCard, CardColour topColour)
    {
        List<Card> playableCards = new List<Card>();

        foreach (Card card in playerHand)
        {
            if (card.cardColour == topColour || card.cardValue == topCard.cardValue || card.cardColour == CardColour.NONE)
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

        // BEST ACTION CARDS
        foreach (Card card in playableCards)
        {
            if (card.cardValue == CardValue.PLUS_FOUR)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
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

        // REGULAR CARDS
        foreach (Card card in playableCards)
        {
            if (bestRegularCard == null || card.cardValue > bestRegularCard.cardValue)
            {
                bestRegularCard = card;
            }
        }

        // MAKE DECISION
        if (bestActionCard != null)
        {
            return bestActionCard;
        }

        if (bestRegularCard != null)
        {
            return bestRegularCard;
        }

        // DEFAULT
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
        foreach (var colour in colourCount)
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