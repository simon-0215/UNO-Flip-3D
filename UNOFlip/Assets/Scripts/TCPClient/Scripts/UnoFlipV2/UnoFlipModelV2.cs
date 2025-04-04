using QFramework;
using MyTcpClient;
using System.Collections.Generic;
using UnoFlipV2;

public enum FlipGameState 
{
    /// <summary>
    /// ��ʼ���׶�
    /// </summary>
    init,
    /// <summary>
    /// �ȴ�������ɫ�ж�
    /// </summary>
    waitOtherRoleAction,
    /// <summary>
    /// �ȴ����ж�
    /// </summary>
    myAction,
    /// <summary>
    /// ���ж��󣬵ȴ�����˽��
    /// </summary>
    actionedWaitResult,

    finished,
}

public class UnoFlipModelV2 : AbstractModel
{
    /// <summary>
    /// ��Ϸ��ʼ����
    /// </summary>
    public MsgInitFlipGame initData;
    public int myIdx => initData.myIdx;

    /// <summary>
    /// ���ڵ�ǰ�ͻ�����Ҷ�������
    /// </summary>
    public int facingPlayer
    {
        get { return (initData.myIdx + 2) % 4; }
    }
    /// <summary>
    /// ��ǰ�ж���ɫ
    /// </summary>
    public int currentRoleIdx = -1;

    /// <summary>
    /// �����±��ж�һ����ɫ�Ƿ��������
    /// </summary>
    /// <param name="roleIdx"></param>
    /// <returns></returns>
    public bool isRoleHuman(int roleIdx)
    {
        return roleIdx == initData.myIdx || roleIdx == facingPlayer;
    }

    /// <summary>
    /// 4����ɫ������
    /// </summary>
    public List<List<UnoFlipV2.Card>> rolesHandCard;

    /// <summary>
    /// �ж����򣺿�ʼʱΪ˳ʱ��
    /// </summary>
    public PlayDirection direction = PlayDirection.clockwise;
    /// <summary>
    /// ��ǰ���棺�����棨���泯�ϣ�
    /// </summary>
    public Side side = Side.Light;

    private UnoFlipV2.Card _lastCard;
    /// <summary>
    /// ���������
    /// �����ж��Ƿ�ɳ���
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
    /// ����һ������Ҫ�жϵ���ɫ����Ϊ�еĹ�����/ȫ���ƻ�������ɫ������nextColor��һ������lastPlayedCard.color��
    /// </summary>
    public CardColor nextColor;

    public FlipGameState gameState = FlipGameState.init;
    /// <summary>
    /// �ҵ��Єӻغ�
    /// </summary>
    public bool isMyTurn => gameState == FlipGameState.myAction && myIdx == currentRoleIdx;

    /// <summary>
    /// ����uno
    /// </summary>
    public bool unoDeclared = false;

    /// <summary>
    /// ���ƶ���һ���ƣ������ݣ�
    /// </summary>
    public UnoFlipV2.Card nextDeckCard;
    //public string nextCardContent;

    protected override void OnInit()
    {
        
    }
}
