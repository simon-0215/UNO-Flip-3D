using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace UnoFlipV2
{
    public enum CardType
    {
        //0到9数字牌
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

        /// <summary>
        /// 洗牌万能牌
        /// 是在2016年新增的新卡。出这张牌的玩家可以收集所有玩家的牌进行洗牌然后平均重新分发。
        /// 此牌的特点是出牌者可以选择任意颜色。
        /// 由于它可以重置游戏状态，因此在某人快要获胜时使用是非常有效的。
        /// 由于它可以在任何时候出牌，可能是获胜的重要策略。
        /// </summary>
        Shuffle, //暂不实现

        /// <summary>
        /// 翻转
        /// </summary>
        Flip,
    }
    /// <summary>
    /// 功能效果
    /// </summary>
    public enum FunctionEffect
    {
        /// <summary>
        /// 让某个角色抽牌 roleId cardCount
        /// </summary>
        draw,

        /// <summary>
        /// 让某个角色抽牌，直到抽到某个花色，或者达到上限数量 roleId cardColor cardCount
        /// </summary>
        drawUtilColorMatch,

        /// <summary>
        /// 光明面、黑暗面切换
        /// </summary>
        flip,

        /// <summary>
        /// 收集所有角色的牌进行洗牌然后平均分发
        /// </summary>
        shuffle,//暂不实现
    }

    public enum CardColor
    {
        RED,
        BLUE,
        GREEN,
        YELLOW,
        NONE,//超级牌是黑色（功能牌和数字牌一样，也有4种色）
    }

    public class CardSideData
    {
        public CardType type;

        public CardColor color;
        public int value;

        public CardSideData(CardType type, CardColor color, int value = -1)
        {
            this.type = type;
            this.color = color;
            this.value = value;
        }
    }
    /*
    public class Card
    {
        static int _nextId = 0;
        public static int nextId { get { _nextId++; return _nextId; } }//新建card时，自增

        public int id;
        public CardSideData light;
        public CardSideData dark;
        public Card(CardSideData light, CardSideData dark)
        {
            this.id = nextId;
            this.light = light;
            this.dark = dark;
        }

        public override string ToString()
        {
            return $"{id},{light.type},{light.color},{light.value},{dark.type},{dark.color},{dark.value}";
        }
        public string Encode()
        {
            //         0          1                  2               3                   4               5           6
            return $"{id},{(int)light.type},{(int)light.color},{light.value},{(int)dark.type},{(int)dark.color},{dark.value}";
        }
        public static Card Decode(string str)
        {
            string[] arr = str.Split(',');
            CardSideData light = new CardSideData(
                (CardType)int.Parse(arr[1]),
                (CardColor)int.Parse(arr[2]),
                int.Parse(arr[3])
                );
            CardSideData dark = new CardSideData(
                (CardType)int.Parse(arr[4]),
                (CardColor)int.Parse(arr[5]),
                int.Parse(arr[6])
                );
            Card card = new Card(light, dark);
            card.id = int.Parse(arr[0]);
            return card;
        }
    }*/
    public class Card
    {
        static int _nextId = 0;
        public static int nextId { get { _nextId++; return _nextId; } }//新建card时，自增

        public int id;
        public CardSideData light;
        public CardSideData dark;
        public Card(CardSideData light, CardSideData dark)
        {
            this.id = nextId;
            this.light = light;
            this.dark = dark;
        }

        public override string ToString()
        {
            return $"{id},{light.type},{light.color},{light.value},{dark.type},{dark.color},{dark.value}";
        }
        public string Encode()
        {
            //         0          1                  2               3                   4               5           6
            return $"{id},{(int)light.type},{(int)light.color},{light.value},{(int)dark.type},{(int)dark.color},{dark.value}";
        }
        public static Card Decode(string str)
        {
            string[] arr = str.Split(',');
            //Debug.Log($"Card Decode {str} {arr.Length} {arr}");

            CardSideData light = new CardSideData(
                (CardType)int.Parse(arr[1]),
                (CardColor)int.Parse(arr[2]),
                int.Parse(arr[3])
                );
            CardSideData dark = new CardSideData(
                (CardType)int.Parse(arr[4]),
                (CardColor)int.Parse(arr[5]),
                int.Parse(arr[6])
                );
            Card card = new Card(light, dark);
            card.id = int.Parse(arr[0]);
            return card;
        }

        public CardSideData GetData(Side side)
        {
            return side == Side.Light ? light : dark;
        }

        public static List<Card> ParseHandCards(string str)
        {
            string[] numbers = str.Split(',');

            // 2. 按 7 个一组分组（.NET 6+ 的 Chunk 方法）
            // id,type1,color1,value1,t2,c2,v2  固定7个一组
            var groups = numbers.Chunk(7);

            // 3. 将每组重新拼接成逗号分隔的字符串
            string[] result = groups.Select(g => string.Join(",", g)).ToArray();

            return result.Select(s => Decode(s)).ToList();
        }
    }
    /// <summary>
    /// 出牌顺序
    /// </summary>
    public enum PlayDirection
    {
        clockwise,
        antiClockwise
    }
    /// <summary>
    /// 光明面还是黑暗面
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// 光明面 front side
        /// </summary>
        Light,
        /// <summary>
        /// 黑暗面 back side
        /// </summary>
        Dark,
    }


    //角色行动、行动的结果
    public enum RoleActionType
    {
        /// <summary>
        /// 抽牌
        /// </summary>
        drawCard,
        /// <summary>
        /// 打出牌
        /// </summary>
        playCard,
        /// <summary>
        /// 选择下一个花色
        /// </summary>
        //chooseNextColor,
    }

    public class RoleAction
    {
        #region 
        static int _nextActionId = 0;
        // 全局的角色行动id自增实现
        static int nextActionId
        {
            get
            {
                _nextActionId++;
                return _nextActionId;
            }
        }
        public int actionId { get; private set; }
        public RoleAction(RoleActionType type, int cardId = -1, CardColor color = CardColor.NONE)
        {
            //actionId = nextActionId;

            this.type = type;
            this.cardId = cardId;
            this.color = color;
        }

        public override string ToString()
        {
            return $"{actionId} {type} {cardId} {color}";
        }
        /*public string ToStringDetail(UnoFlipGameData gameData)
        {
            return $"{type} {gameData.deck.CardsDic[cardId]} {color}";
        }*/

        #endregion
        public RoleActionType type = RoleActionType.drawCard;
        public int cardId = -1;
        public CardColor color;
    }
    /*
    public class RoleActionResult
    {
        public int actionId;

        /// <summary>
        /// 上一张打出的牌
        /// </summary>
        public int lastCardId = 0;
        public CardColor nextColor;

        /// <summary>
        /// 当前抽牌堆顶部的牌
        /// </summary>
        public Card NextDeckCard;

        //行动后，改角色即为上一个角色
        public int lastRoleIdx = 0;
        //行动后，更新出最新的【当前行动角色】
        public int currentRoleIdx = 0;
    }*/

    /// <summary>
    /// 广播的游戏状态，包括弃牌堆顶的牌、当前玩家顺序、手牌数量等
    /// todo：如果需要处理断线重连的情况，需要在连接时发送当前完整状态。
    /// 考虑性能优化，使用增量更新减少数据量，比如只发送变化的部分。
    /// 同时，保证TCP的可靠传输，处理可能的延迟和重传，确保所有客户端状态一致
    /// 
    /// 状态校验码​（哈希值校验数据一致性）
    /// </summary>
    public class GameStateToSync
    {
        /// <summary>
        /// 在本次行动第一阶段是否抽牌了（如果选择抽牌，则可以立即判断是否把抽到的牌打出）
        /// </summary>
        public bool hasDrawCard = false;
        /// <summary>
        /// 本次行动是否喊了uno
        /// </summary>
        //public bool hasUnoDeclear = false;

        /// <summary>
        /// 抽牌堆下一张牌的内容
        /// </summary>
        public string nextCardContent;

        /// <summary>
        /// 所有角色手牌数量
        /// </summary>
        public List<int> handCardsNum;
        /// <summary>
        /// 最后行动过的角色
        /// </summary>
        public int lastRoleIdx = 0;
        /// <summary>
        /// 上一个角色（即最后行动的角色）是否胜利
        /// </summary>
        public bool isLastRoleWin = false;
        /// <summary>
        /// 轮到行动权的角色
        /// </summary>
        public int currentRoleIdx = 0;

        /// <summary>
        /// 上一张打出的牌
        /// </summary>
        public int lastCardId = 0;
        public CardColor nextColor;
        public PlayDirection direction;
        public Side side;

        #region 让某个角色抽牌
        /// <summary>
        /// 是否让某个角色抽drawCardsCount张牌
        /// </summary>
        public int drawCardsCount = 0;
        /// <summary>
        /// 抽牌内容编码
        /// string.Join(",", cards.Select(c =>c.Encode())
        /// </summary>
        public string drawCards;
        /// <summary>
        /// 抽牌角色idx
        /// </summary>
        public int drawCardRoleIdx = -1;
        #endregion

        #region （下次）因未喊UNO抽牌
        /// <summary>
        /// 因未喊Uno罚抽牌Encode字符串，格式同 drawCards
        /// </summary>
        public string unoDrawCards;
        #endregion

        /// <summary>
        /// 当前抽牌堆数量
        /// </summary>
        //public int deckNum = 0;
        /// <summary>
        /// 当前弃牌堆数量
        /// </summary>
        //public int usedCardDeckNum = 0;

        public override string ToString()
        {
            if(drawCardsCount > 0)
            {
                List<Card> cards = Card.ParseHandCards(drawCards);
                Debug.Log($"抽牌{drawCardsCount}张,内容：{string.Join("__", cards.Select(c => c.ToString()))}");
            }

            return $"hasDrawCard {hasDrawCard}, nextCardContent {nextCardContent}, handCount {string.Join('_', handCardsNum)}, lastRoleIdx{lastRoleIdx}, " +
                $"currentRoleIdx{currentRoleIdx}, lastCardId{lastCardId}, nextColor {nextColor},direction {direction},side {side}, drawCount{drawCardsCount},drawcardRole{drawCardRoleIdx},drawCardsContent:{drawCards}";
        }
    }
}

