using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1.C#中 泛型的知识
//2.设计模式中 单例模式的知识
//不继承Mono的单例模式

public class BaseManager<T> where T:new ()
{
    private static T instance;

    public static T Instance
    {
        get
         {
            if (instance == null)
                instance = new T();
            return instance;
        }
    }
    
}


