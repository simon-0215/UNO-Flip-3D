
using System.Reflection;

namespace MyNetworkGame.TCPServer.UnoFlipV2
{
    public class Config
    {
        /// <summary>
        /// 行动之前要等待N毫秒
        /// </summary>
        public static int WaitBeforeAct = 3000;

        /// <summary>
        /// 黑暗模式的wildDraw牌，如果一直没抽到指定花色，最大抽牌数
        /// </summary>
        public static int DarkWildDrawMaxCount = 9;

        /// <summary>
        /// 开启flip牌
        /// </summary>
        public static bool FlipEnabled = true;
        public static int FlipCardsCount = 1;//for debug: trible the number of flip-type cards
    }

    public class DeckData
    {
        UnoFlipGameData gameData;
        public DeckData(UnoFlipGameData gameData)
        {
            this.gameData = gameData;
            Init();
        }
        public static System.Random random = new System.Random();
        /*
        数字0   1 * 1 * 4（花色） = 4
        数字1到9  9 * 2 * 4 = 72
            数字牌 76张

        skip 2 * 4 = 8张
        reverse 2 * 4 = 8
        draw +2  2*4 = 8
            功能牌（也和数字牌一样有4个色） 24张

        wild 4张 
        wild draw +4  4
            万能牌8张

        普通Uno共108张牌

        flip版本再加4张flip牌，共112张， （另外可以加上4张洗牌shuffle万能牌） 共116张

        另外光明面和黑暗面（正反面）的牌类型、颜色、数值/符号的对应关系：
            未搜到正反面卡牌对应关系，那程序中就随机对应（在第一次初始化阶段就随机确定好，后面洗牌时正反面对应关系不变）

        +5:和大家熟知的+2一样，打出+5后，下个玩家要抽5张牌并跳过该回合出牌
        转向牌:和明面一样，改变当前牌局的出牌方向;
        全部跳过牌:除打出该牌玩家外，所有人暂停一轮出牌(等于可连出两张牌)，相当于一张强化型跳过牌;
        王牌(Wild Draw Color):打出该牌的玩家可指定1个颜色，下个玩家需要一直抽牌直到抽到该颜色为止，并跳过他的回合。

        其他：
        2016年加了新卡： 洗牌万能牌 以及自定义空白规则万能牌    
        */
        //光明、黑暗两幅牌
        private static List<CardSideData> light = new List<CardSideData>();
        private static List<CardSideData> dark = new List<CardSideData>();

        /// <summary>
        /// 完整一副牌的字典
        /// </summary>
        public Dictionary<int, Card> CardsDic = new Dictionary<int, Card>();

        /// <summary>
        /// 抽牌堆
        /// </summary>
        public List<Card> cards = new List<Card>();
        public List<Card> usedCardDeck = new List<Card>();//已打出的牌堆（当前弃牌堆，不包含最后一张）

        Card _lastCard;
        /// <summary>
        /// 最后打出的牌
        /// </summary>
        public Card lastPlayedCard
        {
            get { return _lastCard; }
            set
            {
                _lastCard = value;
                nextColor = gameData.side == Side.Light ? value.light.color : value.dark.color;
            }
        }
        public CardSideData lastSideData => gameData.side == Side.Light ? _lastCard.light : _lastCard.dark;

        public CardColor nextColor; //打下一张牌需要判断的颜色（因为有的功能牌/全能牌会设置颜色，所以nextColor不一定等于lastPlayedCard.color）

        //初始化生成正反（光明、黑暗）两副卡牌
        public static void InitSides()
        {
            CardSideData side;

            //0到9数字牌（有的地方也叫符号牌）
            //Number,
            foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
            {
                if (color != CardColor.NONE)
                {
                    #region 数字牌
                    //数字 0
                    side = new CardSideData(CardType.Number, color, 0);
                    light.Add(side);
                    side = new CardSideData(CardType.Number, color, 0);
                    dark.Add(side);
                    //数字 1到9
                    for (int value = 1; value <= 9; value++)
                    {
                        side = new CardSideData(CardType.Number, color, value);
                        light.Add(side);
                        side = new CardSideData(CardType.Number, color, value);
                        light.Add(side);
                        side = new CardSideData(CardType.Number, color, value);
                        dark.Add(side);
                        side = new CardSideData(CardType.Number, color, value);
                        dark.Add(side);
                    }
                    #endregion
                    #region 功能牌
                    side = new CardSideData(CardType.Skip, color, 1);
                    light.Add(side);
                    side = new CardSideData(CardType.Skip, color, 1);
                    light.Add(side);
                    side = new CardSideData(CardType.Skip, color, 3);//黑暗牌组的skip跳过3个玩家（目前版本设定玩家数固定为4）
                    dark.Add(side);
                    side = new CardSideData(CardType.Skip, color, 3);
                    dark.Add(side);

                    side = new CardSideData(CardType.Reverse, color);
                    light.Add(side);
                    side = new CardSideData(CardType.Reverse, color);
                    light.Add(side);
                    side = new CardSideData(CardType.Reverse, color);
                    dark.Add(side);
                    side = new CardSideData(CardType.Reverse, color);
                    dark.Add(side);

                    side = new CardSideData(CardType.Draw, color, 2);
                    light.Add(side);
                    side = new CardSideData(CardType.Draw, color, 2);
                    light.Add(side);
                    side = new CardSideData(CardType.Draw, color, 5);
                    dark.Add(side);
                    side = new CardSideData(CardType.Draw, color, 5);
                    dark.Add(side);
                    #endregion
                }
                else
                {
                    #region 万能牌
                    for (int i = 0; i < 4; i++)
                    {
                        side = new CardSideData(CardType.Wild, color);
                        light.Add(side);
                        side = new CardSideData(CardType.Wild, color);
                        dark.Add(side);

                        side = new CardSideData(CardType.WildDraw, color, 4);
                        light.Add(side);
                        side = new CardSideData(CardType.WildDraw, color, Config.DarkWildDrawMaxCount);
                        dark.Add(side);
                        /*
                        side = new CardSideData(CardType.Shuffle, color);
                        light.Add(side);
                        side = new CardSideData(CardType.Shuffle, color);
                        dark.Add(side);*/

                        if (Config.FlipEnabled)
                        {
                            for(int jj = 0; jj < Config.FlipCardsCount; jj++)
                            {
                                side = new CardSideData(CardType.Flip, color);
                                light.Add(side);
                                side = new CardSideData(CardType.Flip, color);
                                dark.Add(side);
                            }
                        }
                    }
                    #endregion
                }
            }

            Shuffle<CardSideData>(light);
            Shuffle<CardSideData>(dark);

            Debug.Log("Initialized CardSideData : light/dark side");
        }
        public void Init()
        {
            usedCardDeck.Clear();
            cards.Clear();
            for (int i = 0; i < light.Count; i++)
            {
                Card c = new Card(light[i], dark[i]);
                cards.Add(c);
                CardsDic.Add(c.id, c);
            }
            //Shuffle<Card>(cards);
        }

        /// <summary>
        /// 洗抽牌堆/洗光明、黑暗牌组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = random.Next(0, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        /// <summary>
        /// 抽一张牌
        /// </summary>
        /// <returns></returns>
        public Card GetDrawCard()
        {
            Card drawnCard = QueryNextCardInfo();
            cards.RemoveAt(0);
            return drawnCard;
        }

        /// <summary>
        /// 只是查询抽牌堆里下一张牌的信息
        /// </summary>
        /// <returns></returns>
        public Card QueryNextCardInfo()
        {
            if (cards.Count == 0)
            {
                //DECK IS EMPTY SHUFFLE IN USED CARDS
                cards.AddRange(usedCardDeck);
                usedCardDeck.Clear();
                Shuffle<Card>(cards);
            }

            return cards[0];
        }
    }

    /// <summary>
    /// 打牌的角色（玩家、AI）
    /// </summary>
    public class Role
    {
        public string name;
        public int idx;//在角色列表中的下标位置
        public UnoFlipGameData gameData;

        /// <summary>
        /// 手牌
        /// </summary>
        public List<Card> handCards = new List<Card>();
        public void DrawCard(int cardId)
        {
            Card c = handCards.Find(c => c.id == cardId);
            handCards.Remove(c);
        }
        public void DrawCard(Card c)
        {
            gameData.deck.usedCardDeck.Add(c);
            handCards.Remove(c);
        }

        /// <summary>
        /// 是否喊过UNO
        /// 打出牌后，手牌为一张时，到下次轮到出牌之前，这之间可以声明UNO；
        /// 下次轮到出牌的开始阶段判断，如果手牌只有一张，并且没喊过，则抽2张牌
        /// </summary>
        public bool hasUnoDeclared = false;
    }
    public class Player : Role
    {
        public ulong lastConfirmedMsgId = 0;//最后一次确认的消息id
        public ClientState client;

        public Player(ClientState client)
        {
            this.client = client;
        }
    }

    public class UnoFlipGameData
    {
        /// <summary>
        /// 抽牌堆/弃牌堆等
        /// </summary>
        public DeckData deck;

        public List<Role> roles = new List<Role>();

        /// <summary>
        /// 当前出牌玩家在玩家数组中的下标
        /// </summary>
        public int currentRole = 0;
        public int NextRole
        {
            get
            {
                return direction == PlayDirection.clockwise ? (currentRole + 1) % 4 : (currentRole + 4 - 1) % 4;
            }
        }
        /// <summary>
        /// 初始手牌数
        /// </summary>
        public int startHandNum = 7;
        /// <summary>
        /// 当前出牌方向
        /// </summary>
        public PlayDirection direction = PlayDirection.clockwise;
        public void SwitchDirection()
        {
            direction = direction == PlayDirection.clockwise ? PlayDirection.antiClockwise : PlayDirection.clockwise;
        }
        /// <summary>
        /// 当前卡牌面：光明/黑暗
        /// </summary>
        public Side side = Side.Light;
        public void SwitchSide()
        {
            Side newSide = side == Side.Light ? Side.Dark : Side.Light;
            Debug.Log($"~~~~~~ SwitchSide {side} to {newSide}");
            side = newSide;
        }

        public ulong lastSendMsgId = 0;

        /// <summary>
        /// 角色行动id
        /// （每个角色/每次行动后自增1）
        /// 记录游戏过程
        /// </summary>
        //public uint roleActionId = 0;

        GameStatus _state = GameStatus.init;
        public GameStatus state
        {
            get { return _state; }
            set
            {
                if(_state != value)
                {
                    Debug.Log($"--- switch state from {_state} to {value}");
                    _state = value;
                }
            }
        }

        //--------------------------------------------------------------------------------------------
        public UnoFlipV2.Card RoleDrawCard(int roleIdx = -1)
        {
            Card card = deck.GetDrawCard();
            if (roleIdx == -1)
                roleIdx = currentRole;
            roles[roleIdx].handCards.Add(card);
            return card;
        }

        /// <summary>
        /// 判断server是否都收到两个客户端的回复消息
        /// </summary>
        /// <returns></returns>
        public bool CheckConfirmMsg()
        {
            foreach(Role r in roles)
            {
                Player p = r as Player;
                if(p != null)
                {
                    if(p.lastConfirmedMsgId != lastSendMsgId)
                    {
                        return false;
                    }
                }
            }

            //如果返回true，在返回前重置一下confirmed id，避免某一条消息发重复了多次返回true
            (roles[0] as Player).lastConfirmedMsgId = 0;
            (roles[2] as Player).lastConfirmedMsgId = 0;
            return true;
        }

        //判断父类实例是否子类 if (roles[i] is Player) { }
        private void InitRoles(ClientState client1, ClientState client2)
        {
            Player p1 = new Player(client1);
            roles.Add(p1);
            p1.name = client1.PlayerName;
            p1.idx = 0;
            p1.gameData = this;
            client1.player = p1;
            
            Role ai1 = new Role();
            ai1.name = "AI-1";
            ai1.idx = 1;
            ai1.gameData = this;
            roles.Add(ai1);
            
            Player p2 = new Player(client2);
            roles.Add(p2);
            p2.name = client2.PlayerName;
            p2.idx = 2;
            p2.gameData = this;
            client2.player = p2;

            Role ai2 = new Role();
            ai2.name = "AI-2";
            ai2.idx = 3;
            ai2.gameData = this;
            roles.Add(ai2);
        }
        public void Init(ClientState client1, ClientState client2)
        {
            deck = new DeckData(this);
            startHandNum = 7;
            direction = PlayDirection.clockwise;
            side = Side.Light;

            InitRoles(client1, client2);
            currentRole = DeckData.random.Next(0, roles.Count);//随机出先手角色
            
            //发送初始游戏协议
            MsgInitFlipGame msg1 = new MsgInitFlipGame();
            msg1.roles = roles.Select(r => r.name).ToList();
            msg1.firstToPlay = currentRole;
            msg1.myIdx = 0;
            msg1.startHandNum = startHandNum;
            NetManager.Send(client1, msg1);

            MsgInitFlipGame msg2 = new MsgInitFlipGame();
            msg2.roles = msg1.roles;
            msg2.firstToPlay = currentRole;
            msg2.myIdx = 2;
            msg2.startHandNum = startHandNum;
            msg2.id = msg1.id;//这两条消息逻辑上对于server端算是同一条消息（虽然内容不同）
            NetManager.Send(client2, msg2);

            for(int i = 0; i < msg1.roles.Count; i++)
            {
                Debug.Log($"roles[{i}] {msg1.roles[i]} {msg2.roles[i]}");
            }
            lastSendMsgId = msg1.id;
            Debug.Log($"--- 发送协议 MsgInitFlipGame {msg1.id}");
        }

        public UnoFlipGameData(ClientState client1, ClientState client2)
        {
            Init(client1, client2);
        }

        void DealCardToNextRole(int prevRole)
        {
            int nextRoleIdx = (prevRole + 1) % 4;
            Card c = deck.GetDrawCard();
            roles[nextRoleIdx].handCards.Add(c);
        }
        /// <summary>
        /// 发起始手牌
        /// </summary>
        public void InitDealCards()
        {
            for(int i = 0; i < startHandNum; i++)
            {
                Card c = deck.GetDrawCard();
                roles[currentRole].handCards.Add(c);
                for(int j = 0; j < 3; j++)
                {
                    DealCardToNextRole(currentRole + j);
                }
            }
        }
        /// <summary>
        /// 把roles手牌编码为字符串
        /// </summary>
        /// <returns></returns>
        public string GetHandCardsEncode()
        {
            //除了card id，还要包含正反面的 type color value
            /// 卡牌内容
            /// 格式 id,t1,c1,v1,t2,c2,v2_id,t1,c1,v1,t2,c2,v2_....|....|
            /// 角色1的卡牌id,（正面）卡牌type，color，value,（反面）卡牌type，color，value_手牌2_手牌3...|角色2|角色3|角色4
            return string.Join("|", roles.Select(r => string.Join(",", r.handCards.Select(c =>
            {
                //return $"{c.id},{c.light.type},{c.light.color},{c.light.value},{c.dark.type},{c.dark.color},{c.dark.value}";
                return c.Encode();
            }))));
        }

        /// <summary>
        /// 发牌后翻开第一张牌（作为弃牌堆顶的牌）
        /// </summary>
        /// <returns></returns>
        public Card GetFirstPileCard()
        {
            Card card = deck.GetDrawCard();
            while(card.light.color == CardColor.NONE)
            {
                deck.usedCardDeck.Add(card);
                card = deck.GetDrawCard();
            }
            deck.lastPlayedCard = card;
            return card;
        }

        /// <summary>
        /// 执行一个回合
        /// </summary>
        public void ProcessOneAct()
        {
            Role role = roles[currentRole];
            if (role is Player)
            {
                //等待玩家行动
                Debug.Log($"--- ProcessOneAct：等待玩家行动");
                state = GameStatus.waitPlayerAction;
            }
            else //执行一次AI出牌逻辑
            {
                Debug.Log($"--- ProcessOneAct：执行AI出牌逻辑");
                state = GameStatus.waitAIAction;
                UnoFlipGameManager.AIAct(this);
            }
        }

        public void printHandCards()
        {
            for (int i = 0; i < roles.Count; i++)
            {
                Debug.Log($"role {roles[i].name} 手牌:{string.Join("__", roles[i].handCards.Select(c => c.ToString()))}");
            }
        }
    }

    /// <summary>
    /// 游戏状态（从匹配-ready进入游戏之后才开始，到游戏决出胜利玩家，结束游戏为止）
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// 初始化阶段
        /// </summary>
        init,
        /// <summary>
        /// 发牌中
        /// </summary>
        dealingCards,

        /// <summary>
        /// 等待玩家行动
        /// </summary>
        waitPlayerAction,

        /// <summary>
        /// 等待AI行动
        /// </summary>
        waitAIAction,

        /// <summary>
        /// 等待角色（玩家/AI）的行动被同步成功
        /// </summary>
        waitRoleActionSyncOK,
        /// <summary>
        /// 出现胜利玩家
        /// </summary>
        finish,
    }

}
