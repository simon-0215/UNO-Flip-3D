namespace MyNetworkGame.TCPServer
{

    public class MsgPlayerMatchRequest : MsgBase
    {
        public MsgPlayerMatchRequest() { protoName = "MsgPlayerMatchRequest"; }

        public string currentPlayerName = string.Empty;//C端发消息时：发起请求的玩家名称

        public bool isHost = false;//S端同步匹配结果时：告诉具体C端它是否host房主
        public string otherPlayerName = string.Empty;

        // 另外加的两个ai，AI-1、AI-2，4个角色的位置（顺时针）： host AI-1 otherPlayer AI-2
        //对于otherPlayer而言，看起来是这样： otherPlayer AI-2 host AI-1
    }


}