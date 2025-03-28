using MyTcpClient;
using QFramework;
using System;
using UnityEngine;

public struct NetOnMsgEvent
{
    public MsgBase msg;
    public Type type;
}
public class NetOnMsgCommand: AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<CardGameModel>();
        this.SendEvent(new NetOnMsgEvent() { msg = model.currentMsg, type = model.currentMsgType });
    }
}
