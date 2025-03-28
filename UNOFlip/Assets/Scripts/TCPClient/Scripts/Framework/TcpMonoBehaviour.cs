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

        // �����߳��д�������еĲ���
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
        �����̰߳�ȫ��ԭ��ֱ���� NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, (string info) => {}); �¼��ص��в���Unity���̵߳��߼���
        ������̰߳�ȫ���⣬�Ӷ�����NetManager msgLisnter��Ϣ�����ҵ���
        �������ﻻ���� �̰߳�ȫ�Ķ��з�����

        ʹ��ʾ����
        1��EnqueueUIUpdate(() =>
            {
                //UI�������������߳��еĲ���
            });
        2��EnqueueUIUpdate(() => ���������� UpdateUIOrMainThread());
    */
    protected void EnqueueUIUpdate(Action action)
    {
        lock (queueLock)
        {
            uiUpdateQueue.Enqueue(action);
        }
    }
}
