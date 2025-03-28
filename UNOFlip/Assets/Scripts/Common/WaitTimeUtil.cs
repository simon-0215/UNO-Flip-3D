using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitTimeManager
{
    //�ڲ���
    class TaskBehavior : MonoBehaviour { }
    private static TaskBehavior s_Tasks;

    static WaitTimeManager()
    {
        //����һ����ʱ���󣬰��ڲ���ר�����ڴ���Э������
        GameObject go = new GameObject("#WaitTimeManager#");
        GameObject.DontDestroyOnLoad(go);
        s_Tasks = go.AddComponent<TaskBehavior>();
    }

    static IEnumerator _Coroutine(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
    //��ʼ�ȴ�
    static public Coroutine WaitTime(float time, UnityAction callback)
    {
        if(s_Tasks == null)
        {
            //����һ����ʱ���󣬰��ڲ���ר�����ڴ���Э������
            GameObject go = new GameObject("#WaitTimeManager#");
            GameObject.DontDestroyOnLoad(go);
            s_Tasks = go.AddComponent<TaskBehavior>();
        }

        return s_Tasks.StartCoroutine(_Coroutine(time, callback));
    }
    //ȡ���ȴ�
    static public void CancelWait(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            s_Tasks.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}

//ʹ�ð���
public class WaitTimeUtil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*
        print("3��ȴ���ʼ");
        Coroutine coroutine = WaitTimeManager.WaitTime(3, () =>
        {
            print("3��ȴ�����");
        });
        //�ȴ�����ǰ���Թرյȴ�
        WaitTimeManager.CancelWait(ref coroutine);
        */
    }

}
