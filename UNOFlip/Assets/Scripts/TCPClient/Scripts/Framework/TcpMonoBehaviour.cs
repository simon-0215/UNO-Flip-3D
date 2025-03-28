using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTcpClient;

public class TcpMonoBehaviour : MonoBehaviour
{
    private Queue<Action> uiUpdateQueue = new Queue<Action>();
    private readonly object queueLock = new object();

    protected void Update()
    {
        NetManager.Update();

        // 在主线程中处理队列中的操作
        lock (queueLock)
        {
            while (uiUpdateQueue.Count > 0)
            {
                Action action = uiUpdateQueue.Dequeue();
                action?.Invoke();
            }
        }
    }

    /*
        由于线程安全的原因，直接在 NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, (string info) => {}); 事件回调中操作Unity主线程的逻辑，
        会产生线程安全问题，从而导致NetManager msgLisnter消息监听挂掉，
        所以这里换成了 线程安全的队列方案。

        使用示例：
        1、EnqueueUIUpdate(() =>
            {
                //UI操作或其他主线程中的操作
            });
        2、EnqueueUIUpdate(() => 方法名例如 UpdateUIOrMainThread());
    */
    protected void EnqueueUIUpdate(Action action)
    {
        lock (queueLock)
        {
            uiUpdateQueue.Enqueue(action);
        }
    }
}
