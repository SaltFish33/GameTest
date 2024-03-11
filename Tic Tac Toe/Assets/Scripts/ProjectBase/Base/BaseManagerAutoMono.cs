using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManagerAutoMono<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());

                obj.AddComponent<T>();
                //让这个单例对象过场景时不移除
                //因为单例模式对象往往 是存在整个程序生命周期的
                GameObject.DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
