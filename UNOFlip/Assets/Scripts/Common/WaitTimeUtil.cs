using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitTimeManager
{
    //内部类
    class TaskBehavior : MonoBehaviour { }
    private static TaskBehavior s_Tasks;

    static WaitTimeManager()
    {
        //创建一个临时对象，绑定内部类专门用于处理协程任务
        GameObject go = new GameObject("#WaitTimeManager#");
        GameObject.DontDestroyOnLoad(go);
        s_Tasks = go.AddComponent<TaskBehavior>();
    }

    static IEnumerator _Coroutine(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
    //开始等待
    static public Coroutine WaitTime(float time, UnityAction callback)
    {
        if(s_Tasks == null)
        {
            //创建一个临时对象，绑定内部类专门用于处理协程任务
            GameObject go = new GameObject("#WaitTimeManager#");
            GameObject.DontDestroyOnLoad(go);
            s_Tasks = go.AddComponent<TaskBehavior>();
        }

        return s_Tasks.StartCoroutine(_Coroutine(time, callback));
    }
    //取消等待
    static public void CancelWait(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            s_Tasks.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}

//使用案例
public class WaitTimeUtil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*
        print("3秒等待开始");
        Coroutine coroutine = WaitTimeManager.WaitTime(3, () =>
        {
            print("3秒等待结束");
        });
        //等待结束前可以关闭等待
        WaitTimeManager.CancelWait(ref coroutine);
        */
    }

}
