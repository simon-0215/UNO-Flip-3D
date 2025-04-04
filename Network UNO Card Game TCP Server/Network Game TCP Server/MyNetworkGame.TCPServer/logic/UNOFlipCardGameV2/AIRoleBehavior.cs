namespace MyNetworkGame.TCPServer.UnoFlipV2
{
    public class AIRoleBehavior
    {
        public static void SendMsg(UnoFlipGameData gameData, MsgBase msg)
        {
            NetManager.Send((gameData.roles[0] as Player).client, msg);
            NetManager.Send((gameData.roles[2] as Player).client, msg);
        }

        public static RoleAction GetNextAction(UnoFlipGameData gameData)
        {
            RoleAction action = null;

            UnoFlipV2.Card cardToPlay = null;
            List<UnoFlipV2.Card> playableCards = GetPlayableCards(gameData, gameData.roles[gameData.currentRole].handCards);

            //临时手牌（可能临时抽一张牌，但是不计入真实的手牌，因为后续对RoleAction的处理那里，才会真正抽牌到手牌）
            List<Card> tempHandCards = new List<Card>(gameData.roles[gameData.currentRole].handCards);

            bool hasDrawCard = false;
            if (playableCards.Count > 0)
            {
                cardToPlay = ChooseBestCard(playableCards, gameData);
                //判断是否要喊uno if (playerHand.Count == 2)
                if (gameData.roles[gameData.currentRole].handCards.Count == 3)
                {
                    SendMsg(gameData, new MsgRoleUno(gameData.currentRole));
                }

                action = new RoleAction(RoleActionType.playCard, cardToPlay.id);
            }
            else
            {
                //AI抽一张牌

                //tempHandCards.Add(gameData.RoleDrawCard());
                tempHandCards.Add(gameData.deck.QueryNextCardInfo());

                playableCards = GetPlayableCards(gameData, tempHandCards);
                if (playableCards.Count > 0)
                {
                    cardToPlay = ChooseBestCard(playableCards, gameData);
                }

                if (cardToPlay != null)
                {
                    action = new RoleAction(RoleActionType.drawCard, cardToPlay.id);
                }
                else
                {
                    action = new RoleAction(RoleActionType.drawCard);
                }
            }

            if (cardToPlay != null)
            {
                if (cardToPlay.GetData(gameData.side).color == CardColor.NONE)
                {
                    action.color = SelectBestColour(gameData, tempHandCards);
                }
            }

            Debug.Log($"--- AI GetNextAction: {action.ToStringDetail(gameData)}");
            return action;
        }


        static List<Card> GetPlayableCards(UnoFlipGameData gameData, List<Card> cards)
        {
            List<UnoFlipV2.Card> playableCards = new List<Card>();
            foreach(Card card in cards)
            {
                if(UnoFlipGameManager.IsPlayable(card, gameData))
                {
                    playableCards.Add(card);
                }
            }
            
            return playableCards;
        }

        static Card ChooseBestCard(List<Card> playableCards, UnoFlipGameData gameData)
        {
            Card bestActionCard = null;
            Card bestRegularCard = null;
            Card bestWildCard = null;

            int nextPlayerHandSize = gameData.roles[gameData.NextRole].handCards.Count;

            //BEST ACTION CARDS
            foreach (Card card in playableCards)
            {
                CardSideData data = card.GetData(gameData.side);

                if (data.type == CardType.WildDraw)
                {
                    if (nextPlayerHandSize <= 2 || bestActionCard == null)
                    {
                        bestActionCard = card;
                    }
                }
                else if (data.type == CardType.Draw)
                {
                    if (nextPlayerHandSize <= 2 || bestActionCard == null)
                    {
                        bestActionCard = card;
                    }
                }
                else if (data.type == CardType.Skip)
                {
                    if (nextPlayerHandSize <= 2 || bestActionCard == null)
                    {
                        bestActionCard = card;
                    }
                }
                else if (data.type == CardType.Reverse)
                {
                    if (nextPlayerHandSize <= 2 || bestActionCard == null)
                    {
                        bestActionCard = card;
                    }
                }
                else if (data.type == CardType.Wild)
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
                CardSideData data = card.GetData(gameData.side);

                if (bestRegularCard == null || data.type > bestRegularCard.GetData(gameData.side).type)
                {
                    bestRegularCard = card;
                }
            }
            //MAKE DECISION
            if (bestActionCard != null)
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

        static CardColor SelectBestColour(UnoFlipGameData gameData, List<Card> tempHandCards)
        {
            Dictionary<CardColor, int> colourCount = new Dictionary<CardColor, int>
            {
                {CardColor.RED, 0},
                {CardColor.BLUE, 0},
                {CardColor.GREEN, 0},
                {CardColor.YELLOW, 0}
            };

            foreach (Card card in tempHandCards)
            {
                if (card.GetData(gameData.side).color != CardColor.NONE)
                {
                    colourCount[card.GetData(gameData.side).color]++;
                }
            }

            CardColor bestColour = CardColor.RED;
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
}
