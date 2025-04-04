using System.Collections.Generic;
using UnoFlipV2;

namespace MyTcpClient
{
    public class MsgBaseV2 : MsgBase
    {
        public ulong id = 0;
        /// <summary>
        /// 客户端确认回应某条消息
        /// </summary>
        public ulong confirmMsgId;
    }

    /// <summary>
    /// 初始化游戏：玩家名称列表、先手玩家、当前玩家在列表中的下标（位置）
    /// </summary>
    public class MsgInitFlipGame : MsgBaseV2
    {
        public MsgInitFlipGame(ulong confirmMsgId)
        {
            protoName = "MsgInitFlipGame";
            this.confirmMsgId = confirmMsgId;
        }

        /// <summary>
        /// 角色（玩家/AI）名称列表
        /// </summary>
        public List<string> roles;
        /// <summary>
        /// 先手玩家的下标
        /// </summary>
        public int firstToPlay;
        /// <summary>
        /// 当前玩家的下标
        /// </summary>
        public int myIdx;
        /// <summary>
        /// 初始手牌数
        /// </summary>
        public int startHandNum;

    }

    //初始发手牌
    public class MsgInitDealCards : MsgBaseV2
    {
        public MsgInitDealCards(ulong confirmMsgId)
        {
            protoName = "MsgInitDealCards";
            this.confirmMsgId = confirmMsgId;
        }

        /// <summary>
        /// 卡牌内容
        /// 格式 id,t1,c1,v1,t2,c2,v2_id,t1,c1,v1,t2,c2,v2_....|....|
        /// 角色1的卡牌id,（正面）卡牌type，color，value,（反面）卡牌type，color，value_手牌2_手牌3...|角色2|角色3|角色4
        /// </summary>
        public string content;
    }

    /// <summary>
    /// 翻开第一张卡牌
    /// </summary>
    public class MsgFirstPileCard : MsgBaseV2
    {
        public MsgFirstPileCard(ulong confirmMsgId)
        {
            protoName = "MsgFirstPileCard";
            this.confirmMsgId = confirmMsgId;
        }
        public int cardId;
        public string cardContent;//正反面的type color value，格式： t,c,v|t,c,v
        /// <summary>
        /// 抽牌堆下一张牌的内容
        /// </summary>
        public string nextCardContent;
    }

    // ------------------------------------- 打牌阶段

    /// <summary>
    /// 玩家动作请求
    /// </summary>
    public class MsgPlayerAction : MsgBaseV2
    {
        public MsgPlayerAction()
        {
            protoName = "MsgPlayerAction";
        }

        public RoleAction action;
    }

    /// <summary>
    /// 角色动作结果同步
    /// </summary>
    public class MsgRoleActionResult : MsgBaseV2
    {
        //public MsgRoleActionResult() { protoName = "MsgRoleActionResult"; }
        public MsgRoleActionResult(ulong confirmMsgId)
        {
            protoName = "MsgRoleActionResult";
            this.confirmMsgId = confirmMsgId;
        }
        public GameStateToSync gameState;
        public RoleAction action;

        //public string ToString() { }
    }

    /// <summary>
    /// 玩家喊Uno：条件（在手牌变为1张，并且下次轮到出牌之前可以喊uno）
    /// 判断是否喊的时机，在下次轮到出牌的时候
    /// </summary>
    public class MsgRoleUno : MsgBase
    {
        public MsgRoleUno(int roleIdx) { protoName = "MsgRoleUno"; this.roleIdx = roleIdx; }
        public int roleIdx;
    }
}