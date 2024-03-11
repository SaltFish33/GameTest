using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �����ṩ���ⲿ���֡�����¼��ķ���
/// �����ṩ���ⲿ��� Э�̵ķ���
/// </summary>
public class MonoMgr : BaseManager<MonoMgr>
{
    public MonoController controller;

    public MonoMgr()
    {
        //��֤��MonoController�����Ψһ��
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }


    /// <summary>
    /// Ϊ�ⲿ�ṩ�����֡�����¼��ĺ���
    /// </summary>
    /// <param name="function"></param>
    public void AddUpdateListener(UnityAction function)
    {
        controller.AddUpdateListener(function);
    }


    /// <summary>
    /// �ṩ���ⲿ�Ƴ�֡���º���
    /// </summary>
    /// <param name="function"></param>
    public void RemoveUpdateListener(UnityAction function)
    {
        controller.RemoveUpdateListener(function);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }

    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName,value);
    }

    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }


}
