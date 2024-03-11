using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ΪMono�Ĺ�����
/// 1.�������ں���
/// 2.�¼�
/// 3.Э��
/// </summary>
public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
            updateEvent();
    }

    /// <summary>
    /// Ϊ�ⲿ�ṩ�����֡�����¼��ĺ���
    /// </summary>
    /// <param name="function"></param>
    public void AddUpdateListener(UnityAction function)
    {
        updateEvent += function;
    }


    /// <summary>
    /// �ṩ���ⲿ�Ƴ�֡���º���
    /// </summary>
    /// <param name="function"></param>
    public void RemoveUpdateListener(UnityAction function)
    {
        updateEvent -= function;
    }

}
