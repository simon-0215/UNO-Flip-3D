using QFramework;
using MyTcpClient;
using System.Collections.Generic;
using UnoFlipV2;

public enum FlipGameState 
{
    /// <summary>
    /// 初始化阶段
    /// </summary>
    init,
    /// <summary>
    /// 等待其他角色行动
    /// </summary>
    waitOtherRoleAction,
    /// <summary>
    /// 等待我行动
    /// </summary>
    myAction,
    /// <summary>
    /// 我行动后，等待服务端结果
    /// </summary>
    actionedWaitResult,

    finished,
}

public class UnoFlipModelV2 : AbstractModel
{
    /// <summary>
    /// 游戏初始数据
    /// </summary>
    public MsgInitFlipGame initData;
    public int myIdx => initData.myIdx;

    /// <summary>
    /// 坐在当前客户端玩家对面的玩家
    /// </summary>
    public int facingPlayer
    {
        get { return (initData.myIdx + 2) % 4; }
    }
    /// <summary>
    /// 当前行动角色
    /// </summary>
    public int currentRoleIdx = -1;

    /// <summary>
    /// 根据下标判断一个角色是否人类玩家
    /// </summary>
    /// <param name="roleIdx"></param>
    /// <returns></returns>
    public bool isRoleHuman(int roleIdx)
    {
        return roleIdx == initData.myIdx || roleIdx == facingPlayer;
    }

    /// <summary>
    /// 4个角色的手牌
    /// </summary>
    public List<List<UnoFlipV2.Card>> rolesHandCard;

    /// <summary>
    /// 行动方向：开始时为顺时针
    /// </summary>
    public PlayDirection direction = PlayDirection.clockwise;
    /// <summary>
    /// 当前牌面：光明面（正面朝上）
    /// </summary>
    public Side side = Side.Light;

    private UnoFlipV2.Card _lastCard;
    /// <summary>
    /// 最后打出的牌
    /// 用于判断是否可出牌
    /// </summary>
    public UnoFlipV2.Card lastPlayedCard
    {
        set
        {
            _lastCard = value;
            nextColor = side == Side.Light ? value.light.color : value.dark.color;
        }
        get
        {
            return _lastCard;
        }
    }
    public CardSideData lastSideData => side == Side.Light ? _lastCard.light : _lastCard.dark;

    /// <summary>
    /// 打下一张牌需要判断的颜色（因为有的功能牌/全能牌会设置颜色，所以nextColor不一定等于lastPlayedCard.color）
    /// </summary>
    public CardColor nextColor;

    public FlipGameState gameState = FlipGameState.init;
    /// <summary>
    /// 我的行動回合
    /// </summary>
    public bool isMyTurn => gameState == FlipGameState.myAction && myIdx == currentRoleIdx;

    /// <summary>
    /// 喊过uno
    /// </summary>
    public bool unoDeclared = false;

    /// <summary>
    /// 抽牌堆下一张牌（的内容）
    /// </summary>
    public UnoFlipV2.Card nextDeckCard;
    //public string nextCardContent;

    protected override void OnInit()
    {
        
    }
}
