
namespace MyTcpClient
{
    /// <summary>
    /// ƥ�䵽����
    /// </summary>
    public class MsgMatched : MsgBase
    {
        public MsgMatched() { protoName = "MsgMatched"; }
        //client1���������Ϸ�����/���뷿��Ŀͻ�����host��
        public bool isClientHost = false;

        /// <summary>
        /// �����ƶ��ٶ�
        /// </summary>
        public float paddleSpeed = 1;
    }

    public class MsgChooseLevel : MsgBase
    {
        public MsgChooseLevel() { protoName = "MsgChooseLevel"; }

        public int level = 1;//1��2��3 �����С���
    }


    public class MsgWin : MsgBase
    {
        public MsgWin() { protoName = "MsgWin"; }
    }

    public class MsgFail : MsgBase
    {
        public MsgFail() { protoName = "MsgFail"; }
    }

    /// <summary>
    /// �Ͽ�ƥ�䣨�������Ϊ�˳��������˼��
    /// </summary>
    public class MsgDisMatched : MsgBase
    {
        public MsgDisMatched() { protoName = "MsgDisMatched"; }
    }

    /// <summary>
    /// һ�ֽ������Ѿ�ƥ��õ������ͻ��ˣ�����һ��
    /// </summary>
    public class MsgAgain : MsgBase
    {
        public MsgAgain() { protoName = "MsgAgain"; }
    }

    public class MsgCountDown : MsgBase
    {
        public MsgCountDown() { protoName = "MsgCountDown"; }

        public long timestamp = NetManager.GetTimeStampMilliseconds();
        public int countDown = 3000;
    }

    public class MsgPause : MsgBase
    {
        public MsgPause() { protoName = "MsgPause"; }

        //λ��
        public float x = 0;
        public float y = 0;

        //�ٶȷ���
        public float direX = 0;
        public float direY = 0;
    }

    /// <summary>
    /// ����ָ���ͣ
    /// </summary>
    public class MsgResumeRequest : MsgBase
    {
        public MsgResumeRequest() { protoName = "MsgResumeRequest"; }
    }

    public class MsgResume : MsgBase
    {
        public MsgResume() { protoName = "MsgResume"; }

        //λ��
        public float x = 0;
        public float y = 0;

        //�ٶȷ���
        public float direX = 0;
        public float direY = 0;
    }

}