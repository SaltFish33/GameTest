using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//C#�� ����֪ʶ��
//���ģʽ ����ģʽ��֪ʶ��
//�̳���MonoBehavior�ĵ���ģʽ���� ��Ҫ�����Լ���֤����Ψһ��
public class BaseManagerMono<T> :  MonoBehaviour where T: MonoBehaviour
{
    private static T instance;

    //�̳���Mono�Ľű�����ֱ��new
    //����ֻ��ͨ���϶��������� ���� ͨ�� �ӽű���api AddComponent

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
