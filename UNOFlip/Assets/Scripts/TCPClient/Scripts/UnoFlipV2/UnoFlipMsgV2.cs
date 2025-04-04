using System.Collections.Generic;
using UnoFlipV2;

namespace MyTcpClient
{
    public class MsgBaseV2 : MsgBase
    {
        public ulong id = 0;
        /// <summary>
        /// �ͻ���ȷ�ϻ�Ӧĳ����Ϣ
        /// </summary>
        public ulong confirmMsgId;
    }

    /// <summary>
    /// ��ʼ����Ϸ����������б�������ҡ���ǰ������б��е��±꣨λ�ã�
    /// </summary>
    public class MsgInitFlipGame : MsgBaseV2
    {
        public MsgInitFlipGame(ulong confirmMsgId)
        {
            protoName = "MsgInitFlipGame";
            this.confirmMsgId = confirmMsgId;
        }

        /// <summary>
        /// ��ɫ�����/AI�������б�
        /// </summary>
        public List<string> roles;
        /// <summary>
        /// ������ҵ��±�
        /// </summary>
        public int firstToPlay;
        /// <summary>
        /// ��ǰ��ҵ��±�
        /// </summary>
        public int myIdx;
        /// <summary>
        /// ��ʼ������
        /// </summary>
        public int startHandNum;

    }

    //��ʼ������
    public class MsgInitDealCards : MsgBaseV2
    {
        public MsgInitDealCards(ulong confirmMsgId)
        {
            protoName = "MsgInitDealCards";
            this.confirmMsgId = confirmMsgId;
        }

        /// <summary>
        /// ��������
        /// ��ʽ id,t1,c1,v1,t2,c2,v2_id,t1,c1,v1,t2,c2,v2_....|....|
        /// ��ɫ1�Ŀ���id,�����棩����type��color��value,�����棩����type��color��value_����2_����3...|��ɫ2|��ɫ3|��ɫ4
        /// </summary>
        public string content;
    }

    /// <summary>
    /// ������һ�ſ���
    /// </summary>
    public class MsgFirstPileCard : MsgBaseV2
    {
        public MsgFirstPileCard(ulong confirmMsgId)
        {
            protoName = "MsgFirstPileCard";
            this.confirmMsgId = confirmMsgId;
        }
        public int cardId;
        public string cardContent;//�������type color value����ʽ�� t,c,v|t,c,v
        /// <summary>
        /// ���ƶ���һ���Ƶ�����
        /// </summary>
        public string nextCardContent;
    }

    // ------------------------------------- ���ƽ׶�

    /// <summary>
    /// ��Ҷ�������
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
    /// ��ɫ�������ͬ��
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
    /// ��Һ�Uno�������������Ʊ�Ϊ1�ţ������´��ֵ�����֮ǰ���Ժ�uno��
    /// �ж��Ƿ񺰵�ʱ�������´��ֵ����Ƶ�ʱ��
    /// </summary>
    public class MsgRoleUno : MsgBase
    {
        public MsgRoleUno(int roleIdx) { protoName = "MsgRoleUno"; this.roleIdx = roleIdx; }
        public int roleIdx;
    }
}