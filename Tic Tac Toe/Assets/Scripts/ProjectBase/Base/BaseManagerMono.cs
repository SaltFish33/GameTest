using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//C#中 泛型知识点
//设计模式 单例模式的知识点
//继承了MonoBehavior的单例模式对象 需要我们自己保证它的唯一性
public class BaseManagerMono<T> :  MonoBehaviour where T: MonoBehaviour
{
    private static T instance;

    //继承了Mono的脚本不能直接new
    //我们只能通过拖动到对象上 或者 通过 加脚本的api AddComponent

    public static T Instance
    {
        get
        {
            
            return instance;
        }
    }

    protected virtual void Awake()
    {
        instance = this as T;
    }

}
