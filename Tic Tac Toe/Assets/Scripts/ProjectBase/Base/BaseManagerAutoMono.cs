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
                //������������������ʱ���Ƴ�
                //��Ϊ����ģʽ�������� �Ǵ������������������ڵ�
                GameObject.DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
