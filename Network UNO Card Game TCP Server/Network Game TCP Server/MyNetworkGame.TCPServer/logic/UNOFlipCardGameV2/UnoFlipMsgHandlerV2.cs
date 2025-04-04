using MyNetworkGame.TCPServer.UnoFlipV2;

namespace MyNetworkGame.TCPServer
{
    public partial class MsgHandler
    {
        //统计写入日志文件的RoleAction实例个数
        public static int logActionCount = 0;

        #region  init
        public static void MsgInitFlipGame(ClientState client, MsgBase msgBase)
        {
            MsgInitFlipGame msg = (MsgInitFlipGame)msgBase;
            client.player.lastConfirmedMsgId = msg.confirmMsgId;

            UnoFlipGameData gameData = client.room.gameData;
            //收到全部两个客户端的确认消息了
            if (gameData.CheckConfirmMsg())
            {
                Debug.Log("--- 卡牌游戏 初始化完成 ---");
                gameData.state = GameStatus.dealingCards;

                //发牌
                gameData.InitDealCards();//先把数据分配好
                MsgInitDealCards msgDealCards = new MsgInitDealCards();
                msgDealCards.content = gameData.GetHandCardsEncode();
                gameData.lastSendMsgId = msgDealCards.id;

                NetManager.Send(client, msgDealCards);
                NetManager.Send(client.Opponent, msgDealCards);
                Debug.Log($"--- 发送协议 MsgInitDealCards {msgDealCards.id}");
                Debug.Log($"--- 初始手牌 {msgDealCards.content}");
            }
        }

        public static void MsgInitDealCards(ClientState client, MsgBase msgBase)
        {
            MsgInitDealCards msg = (MsgInitDealCards)msgBase;
            client.player.lastConfirmedMsgId = msg.confirmMsgId;

            UnoFlipGameData gameData = client.room.gameData;
            //收到全部两个客户端的确认消息了
            if (gameData.CheckConfirmMsg())
            {
                Debug.Log("--- 初始发牌 完成 ---");
                MsgFirstPileCard msgFirstCard = new MsgFirstPileCard();
                Card c = gameData.GetFirstPileCard();
                msgFirstCard.cardId = c.id;
                msgFirstCard.cardContent = c.Encode();
                msgFirstCard.nextCardContent = gameData.deck.QueryNextCardInfo().Encode();
                gameData.lastSendMsgId = msgFirstCard.id;

                NetManager.Send(client, msgFirstCard);
                NetManager.Send(client.Opponent, msgFirstCard);

                CardSideData card = gameData.deck.lastPlayedCard.light;
                Debug.Log($"--- 发送协议 MsgFirstPileCard {msgFirstCard.id} {card.type} {card.color} {card.value}");
            }
        }

        public static void MsgFirstPileCard(ClientState client, MsgBase msgBase)
        {
            MsgFirstPileCard msg = (MsgFirstPileCard)msgBase;
            client.player.lastConfirmedMsgId = msg.confirmMsgId;

            UnoFlipGameData gameData = client.room.gameData;
            //收到全部两个客户端的确认消息了
            if (gameData.CheckConfirmMsg())
            {
                Debug.Log($"--- 开始第一次出牌 currentRole {gameData.currentRole}---");

                gameData.ProcessOneAct();
            }
        }
        #endregion

        #region Play Game
        public static void MsgPlayerAction(ClientState client, MsgBase msgBase)
        {
            UnoFlipGameData gameData = client.room.gameData;
            if (client.player.idx != gameData.currentRole)
            {
                Debug.Log($"***** game state error: 在非玩家回合收到玩家行动请求 client.player.idx != gameData.currentRole {client.player.idx} {gameData.currentRole}*****");
                return;
            }
            if (gameData.state != GameStatus.waitPlayerAction)
            {
                Debug.Log($"*************** game state error: 在非waitPlayerAction状态收到玩家行动请求 {gameData.state} ***************");
                //return; ##2 to fix bug
            }

            MsgPlayerAction msg = (MsgPlayerAction)msgBase;

            //判断玩家行动是否合法，不合法则，发送同步信息，强制刷新客户端的当前游戏状态
            if(msg.action.type == RoleActionType.drawCard || msg.action.type == RoleActionType.playCard)//抽牌请求
            {
                if (msg.action.type == RoleActionType.drawCard)
                    Debug.Log("--- 收到抽牌请求");
                else if(msg.action.cardId > 0)
                    Debug.Log("--- 收到出牌请求 ");

                if(msg.action.cardId > 0)
                {
                    Card card = gameData.deck.CardsDic[msg.action.cardId];
                    if (!UnoFlipGameManager.IsPlayable(card, gameData))
                    {
                        CardSideData data = gameData.side == Side.Light ? card.light : card.dark;
                        Debug.Log($"***** game state error: 玩家行动请求[出牌]非法 {card} {gameData.side} {gameData.deck.nextColor} {gameData.deck.lastPlayedCard} *****");
                                                        // 4,Number,RED,5,Number,GREEN,9    Light          BLUE                      11, Number,BLUE,0,  WildDraw,NONE,9
                                                        // 7,Number,GREEN,8,Draw,RED,5      Dark           GREEN                     4,Flip,NONE,-1,     Number,RED,4
                        //发送同步信息，强制刷新客户端的当前游戏状态
                        //todo

                        return;
                    }
                }

                //计算玩家行动的结果，更新服务端的游戏状态
                MsgRoleActionResult msgStateSync = new MsgRoleActionResult();
                msg.action.RefreshID();
                msgStateSync.action = msg.action;

                Debug.Log("");
                Debug.Log("");
                Debug.Log("");
                logActionCount++;
                Debug.Log($"action count:{logActionCount.ToString()}");
                Debug.Log("------------------ ------------------");
                Debug.Log($"***** 同步玩家行动结果 行动（前）:{msgStateSync.action.ToString()} *****");
                gameData.printHandCards();

                msgStateSync.gameState = UnoFlipGameManager.getAndUpdateState(msg.action, gameData, client.player.idx);
                

                //同步客户端的游戏状态
                gameData.state = GameStatus.waitRoleActionSyncOK;

                gameData.lastSendMsgId = msgStateSync.id;
                NetManager.Send(client, msgStateSync);
                NetManager.Send(client.Opponent, msgStateSync);

                Debug.Log("------------------ ------------------");
                Debug.Log($"***** 同步玩家行动结果 玩家{gameData.roles[msgStateSync.gameState.lastRoleIdx].name} 行动（后）:{msgStateSync.action.ToString()} *****");
                gameData.printHandCards();
            }
        }

        public static void MsgRoleActionResult(ClientState client, MsgBase msgBase)
        {
            MsgRoleActionResult msg = (MsgRoleActionResult)msgBase;

            client.player.lastConfirmedMsgId = msg.confirmMsgId;

            UnoFlipGameData gameData = client.room.gameData;
            //收到全部两个客户端的确认消息了
            if (gameData.CheckConfirmMsg())
            {
                DelayExcuteAct(client);
            }
        }

        static async void DelayExcuteAct(ClientState client)
        {
            // 等待 N 毫秒（非阻塞）
            await Task.Delay(Config.WaitBeforeAct);

            client.room.gameData.ProcessOneAct();
        }

        public static void MsgRoleUno(ClientState client, MsgBase msgBase)
        {
            MsgRoleUno msg = (MsgRoleUno)msgBase;

            client.room.gameData.roles[msg.roleIdx].hasUnoDeclared = true;

            NetManager.Send(client.Opponent, msgBase);
        }
        #endregion
    }
}