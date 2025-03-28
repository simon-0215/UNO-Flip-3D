
namespace MyTcpClient
{
    /// <summary>
    /// 匹配到对手
    /// </summary>
    public class MsgMatched : MsgBase
    {
        public MsgMatched() { protoName = "MsgMatched"; }
        //client1（先连接上服务器/进入房间的客户端是host）
        public bool isClientHost = false;

        /// <summary>
        /// 球拍移动速度
        /// </summary>
        public float paddleSpeed = 1;
    }

    public class MsgChooseLevel : MsgBase
    {
        public MsgChooseLevel() { protoName = "MsgChooseLevel"; }

        public int level = 1;//1、2、3 慢、中、快
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
    /// 断开匹配（可以理解为退出房间的意思）
    /// </summary>
    public class MsgDisMatched : MsgBase
    {
        public MsgDisMatched() { protoName = "MsgDisMatched"; }
    }

    /// <summary>
    /// 一局结束（已经匹配好的两个客户端）再来一局
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

        //位置
        public float x = 0;
        public float y = 0;

        //速度方向
        public float direX = 0;
        public float direY = 0;
    }

    /// <summary>
    /// 请求恢复暂停
    /// </summary>
    public class MsgResumeRequest : MsgBase
    {
        public MsgResumeRequest() { protoName = "MsgResumeRequest"; }
    }

    public class MsgResume : MsgBase
    {
        public MsgResume() { protoName = "MsgResume"; }

        //位置
        public float x = 0;
        public float y = 0;

        //速度方向
        public float direX = 0;
        public float direY = 0;
    }

}