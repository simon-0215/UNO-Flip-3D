/*
Number,

        //------------------ 功能牌
        /// <summary>
        /// 下面N家跳过出牌（光明面跳过1，黑暗面跳过所有其他玩家，即自己一个回合后，再来一个回合）
        /// </summary>
        Skip,
        /// <summary>
        /// 翻转出牌顺序
        /// </summary>
        Reverse,
        /// <summary>
        /// 下家抽N张牌：N=2或者5（根据flip状态）
        /// </summary>
        Draw,

        //------------------- 万能牌

        /// <summary>
        /// 变色牌。
        /// 不论上一张牌是什么颜色，都可以随意指定下家出牌的颜色。该牌即使在你手中有其它可出的牌时也可以打出。
        /// 如果该牌是抽出的首张牌，那么由庄家左边玩家任意指定颜色出牌。
        /// </summary>
        Wild,
        /// <summary>
        /// 只有自己手中没有与上一张牌同色(不包括同数字或功能）才能使用（合法出牌）,
        /// 下家需从牌堆中罚摸4张牌，且不能再出牌，
        /// 可随意指定再下一家出牌颜色，。
        /// </summary>
        WildDraw,

        /// 洗牌万能牌
        Shuffle, //暂不实现

        /// <summary>
        /// 翻转
        /// </summary>
        Flip, 
*/
using System.Reflection;
using System.Runtime.InteropServices;

namespace MyNetworkGame.TCPServer.UnoFlipV2
{
    public class UnoFlipGameManager
    {
        /// <summary>
        /// 是否合法出牌（根据当前游戏状态判断）
        /// </summary>
        /// <param name="cardData"></param>
        /// <returns></returns>
        public static bool IsPlayable(Card cardData, UnoFlipGameData gameData, int roleIdx = -1)
        {
            CardSideData data = gameData.side == Side.Light ? cardData.light : cardData.dark;
            /*
                Number,
                //------------------ 功能牌
                Skip,
                Reverse,
                Draw,
                //------------------- 万能牌
                Wild,
                WildDraw,
                Flip,
            */

            switch (data.type)
            {
                case CardType.Number:
                    return data.color == gameData.deck.nextColor || data.value == gameData.deck.lastSideData.value;
                //功能牌
                case CardType.Skip:
                case CardType.Reverse:
                case CardType.Draw:
                    return data.color == gameData.deck.nextColor || data.type == gameData.deck.lastSideData.type;
                //全能牌
                case CardType.Wild:
                case CardType.Flip:
                    return true;
                //特殊全能牌  只有自己手中没有与上一张牌同色才能使用（合法出牌）
                case CardType.WildDraw:
                    if (gameData.deck.lastSideData.color == CardColor.NONE) return true;

                    int role = roleIdx == -1 ? gameData.currentRole : roleIdx;
                    List<UnoFlipV2.Card> handCards = gameData.roles[role].handCards;
                    foreach (UnoFlipV2.Card card in handCards)
                    {
                        CardSideData _cardData = gameData.side == Side.Light ? card.light : card.dark;
                        if (_cardData.color == gameData.deck.lastSideData.color)
                            return false;
                    }
                    return true;
            }

            Debug.Log($"***** wrong card type {data.type} *****");
            return false;
        }

        /// <summary>
        /// 根据当前打牌方向、当前角色位置，移位步数，计算下一个行动角色
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="moveStep"></param>
        /// <returns></returns>
        static int getNextRoleIdx(UnoFlipGameData gameData, int moveStep)
        {
            if (gameData.direction == PlayDirection.clockwise)
                return (gameData.currentRole + moveStep) % 4;
            else
                return (gameData.currentRole - moveStep + 4) % 4;
        }

        public static GameStateToSync getAndUpdateState(RoleAction action, UnoFlipGameData gameData, int roleIdx = -1)
        {
            #region init
            GameStateToSync state = new GameStateToSync();
            int role = roleIdx == -1 ? gameData.currentRole : roleIdx;
            //init return value
            state.handCardsNum = gameData.roles.Select(r => r.handCards.Count).ToList();
            state.lastRoleIdx = role;
            state.currentRoleIdx = role;//
            state.lastCardId = gameData.deck.lastPlayedCard.id;
            state.nextColor = gameData.deck.nextColor;
            state.direction = gameData.direction;
            state.side = gameData.side;
            state.drawCardsCount = 0;
            state.drawCards = string.Empty;
            state.drawCardRoleIdx = -1;
            state.nextCardContent = gameData.deck.QueryNextCardInfo().Encode();

            void drawCards(int drawNum, int pRoleId = -1)
            {
                int _role = pRoleId == -1 ? gameData.currentRole : pRoleId;
                state.drawCardsCount = drawNum;
                List<Card> cards = new List<Card>();
                for (int i = 0; i < drawNum; i++)
                {
                    Card c = gameData.deck.GetDrawCard();
                    cards.Add(c);
                    gameData.roles[_role].handCards.Add(c);
                }
                state.drawCards = string.Join(",", cards.Select(c => c.Encode()));
                Debug.Log($"--- drawCardsContent {drawNum} {state.drawCards}");
                state.drawCardRoleIdx = _role;
            }
            #endregion
            if (action.type == RoleActionType.drawCard)
            {
                //todo：处理抽牌逻辑
                Card c = gameData.deck.GetDrawCard();
                gameData.roles[gameData.currentRole].handCards.Add(c);
                state.hasDrawCard = true;
                state.handCardsNum = gameData.roles.Select(r => r.handCards.Count).ToList();
                state.nextCardContent = gameData.deck.QueryNextCardInfo().Encode();
                //return state;
            }
            if (action.cardId < 1) //cardId无效则不出牌（只抽牌，没出牌）
            {
                state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, 1);
                return state;
            }
            //处理出牌逻辑
            CardColor color = action.color;
            Card card = gameData.deck.CardsDic[action.cardId];
            CardSideData data = gameData.side == Side.Light ? card.light : card.dark;

            gameData.deck.lastPlayedCard = card;
            state.lastCardId = gameData.deck.lastPlayedCard.id;
            state.nextColor = gameData.deck.nextColor;
            gameData.roles[gameData.currentRole].DrawCard(card);
            state.handCardsNum = gameData.roles.Select(r => r.handCards.Count).ToList();
            //判断手牌数是否为0，胜利
            if (state.handCardsNum[gameData.currentRole] == 0)
            {
                state.isLastRoleWin = true;
                gameData.state = GameStatus.finish;
            }

            switch (data.type)
            {
                case CardType.Number:
                    state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, 1);
                    break;

                case CardType.Skip:
                    state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, data.value + 1);
                    break;
                case CardType.Reverse:
                    gameData.SwitchDirection();
                    state.direction = gameData.direction;
                    state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, 1);
                    break;
                case CardType.Draw:
                    state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, 1);
                    drawCards(data.value);
                    state.nextCardContent = gameData.deck.QueryNextCardInfo().Encode();
                    break;

                case CardType.Wild:
                    state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, 1);
                    gameData.deck.nextColor = color;
                    state.nextColor = gameData.deck.nextColor;
                    break;
                case CardType.WildDraw:
                    gameData.deck.nextColor = color;
                    state.nextColor = gameData.deck.nextColor;

                    //下家抽牌
                    int _drawCardsRole = getNextRoleIdx(gameData, 1);
                    if (gameData.side == Side.Light)
                    {
                        drawCards(data.value, _drawCardsRole);
                        state.nextCardContent = gameData.deck.QueryNextCardInfo().Encode();
                    }
                    else
                    {
                        List<Card> cards = new List<Card>();
                        Card c = gameData.deck.GetDrawCard();
                        cards.Add(c);
                        gameData.roles[_drawCardsRole].handCards.Add(c);
                        int drawCount = 1;
                        while (c.GetData(gameData.side).color != color && drawCount < data.value)
                        {
                            c = gameData.deck.GetDrawCard();
                            cards.Add(c);
                            gameData.roles[_drawCardsRole].handCards.Add(c);
                            drawCount++;
                        }
                        state.drawCards = string.Join(",", cards.Select(c => c.Encode()));
                        state.drawCardRoleIdx = _drawCardsRole;
                        state.drawCardsCount = drawCount;

                        state.nextCardContent = gameData.deck.QueryNextCardInfo().Encode();
                    }
                    state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, 2);//下家不能出牌，即skip 1的效果
                    break;
                case CardType.Flip:
                    state.currentRoleIdx = gameData.currentRole = getNextRoleIdx(gameData, 1);
                    
                    gameData.SwitchSide();
                    state.side = gameData.side;///

                    gameData.deck.lastPlayedCard = card;
                    state.nextColor = gameData.deck.nextColor = color;
                    
                    break;
            }

            //处理完上一个角色行动，接下来切换行动角色，先判是否未喊UNO，并且手牌数为1：罚抽2张牌
            if(gameData.roles[gameData.currentRole].hasUnoDeclared == false && gameData.roles[gameData.currentRole].handCards.Count == 1)
            {
                List<Card> cards = new List<Card>();
                Card c = gameData.deck.GetDrawCard();
                cards.Add(c);
                gameData.roles[gameData.currentRole].handCards.Add(c);

                c = gameData.deck.GetDrawCard();
                cards.Add(c);
                gameData.roles[gameData.currentRole].handCards.Add(c);

                state.unoDrawCards = string.Join(",", cards.Select(c => c.Encode()));

                state.nextCardContent = gameData.deck.QueryNextCardInfo().Encode();
            }
            state.handCardsNum = gameData.roles.Select(r => r.handCards.Count).ToList();

            //记录日志 行动、结果、前后状态
            Debug.Log("------------------");
            //Debug.Log($"")
            return state;
        }

        /// <summary>
        /// AI行动一次
        /// </summary>
        /// <param name="gameData"></param>
        public static void AIAct(UnoFlipGameData gameData)
        {
            int roleId = gameData.currentRole;

            Debug.Log("");
            Debug.Log("");
            Debug.Log("");
            MsgHandler.logActionCount++;
            Debug.Log($"action count:{MsgHandler.logActionCount.ToString()}");
            Debug.Log("------------------ ------------------");
            Debug.Log($"--- AI {gameData.roles[roleId].name} 行动一次（前）");
            gameData.printHandCards();

            RoleAction roleAction = AIRoleBehavior.GetNextAction(gameData);
            Debug.Log($"### [{gameData.roles[roleId].name}] AI action:{roleAction.ToString()} ");

            MsgRoleActionResult msgStateSync = new MsgRoleActionResult();
            msgStateSync.gameState = getAndUpdateState(roleAction, gameData);
            msgStateSync.action = roleAction;
            //同步客户端的游戏状态
            gameData.state = GameStatus.waitRoleActionSyncOK;

            gameData.lastSendMsgId = msgStateSync.id;
            AIRoleBehavior.SendMsg(gameData, msgStateSync);

            Debug.Log($"--- AI {gameData.roles[roleId].name} 行动一次（后） {roleAction.ToString()} ");
            gameData.printHandCards();
        }

        /// <summary>
        /// 从角色手牌中找出可以出的牌
        /// </summary>
        /// <param name="roleIdx"></param>
        /// <returns></returns>
        public static UnoFlipV2.Card getDrawableCard(int roleIdx)
        {
            return null;
        }
    }
}
