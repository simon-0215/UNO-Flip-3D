using MyTcpClient;
using QFramework;

namespace UnoFlipV2
{
    public class UnoFlipAppV2 : Architecture<UnoFlipAppV2>
    {
        protected override void Init()
        {
            this.RegisterSystem(new UnoFlipGameSystemV2());//ºËÐÄUnoFlipÓÎÏ·Âß¼­system
            this.RegisterSystem(new NetworkSystem());//ÍøÂçsystem
            this.RegisterModel(new UnoFlipModelV2());
        }
    }

    public struct NetOnMsgEventV2
    {
        public MsgBase msg;
        public System.Type type;
    }
    /*
    public class NetOnMsgCommandV2 : AbstractCommand
    {
        protected override void OnExecute()
        {
            //var model = this.GetModel<UnoFlipModelV2>();
            //this.SendEvent(new NetOnMsgEvent() { msg = model.currentMsg, type = model.currentMsgType });
        }
    }*/

    public struct CardClickedEvent
    {
        public Card cardData;
        public CardDisplay cardDisplay;
    }

    public struct DeckClickedEvent
    {

    }
    public class DeckClickedCommand : AbstractCommand
    {
        UnoFlipModelV2 model;

        protected override void OnExecute()
        {
            model = this.GetModel<UnoFlipModelV2>();

            this.SendEvent<DeckClickedEvent>();
        }
    }

    public struct UnoClickEvent
    {
    }
}


