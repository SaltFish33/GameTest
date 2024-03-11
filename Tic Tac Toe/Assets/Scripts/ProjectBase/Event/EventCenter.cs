using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventInfo<T> :IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public interface IEventInfo
{

}


public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}


/// <summary>
/// �¼����� ����ģʽ����
/// 1.Dictionary
/// 2.ί��
/// 3.�۲������ģʽ
/// 4.����
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{



    


    //key ��Ӧ���� �¼�������(����������� ������� ͨ�صȵ�)
    //Value ��Ӧ���� ��������¼� ��ӦҪִ�еĺ����ǵ�ί��
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// ����¼�����
    /// </summary>
    /// <param name="�¼�������"></param>
    /// <param name="׼�����������¼��� ί�к���"></param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        //��û�ж�Ӧ���¼�����
        if (eventDic.ContainsKey(name))
        {
            //�е����
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        //û�е����
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }

    }

    /// <summary>
    /// ��������Ҫ�������ݵ��¼�
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener(string name, UnityAction action)
    {
        //��û�ж�Ӧ���¼�����
        if (eventDic.ContainsKey(name))
        {
            //�е����
            (eventDic[name] as EventInfo).actions += action;
        }
        //û�е����
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }

    }


    /// <summary>
    /// �Ƴ���Ӧ���¼�����
    /// </summary>
    /// <param name="�¼�������"></param>
    /// <param name="��Ӧ֮ǰ��ί�к���"></param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }

    /// <summary>
    /// �Ƴ�����Ҫ�������¼�
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }


    /// <summary>
    /// �¼�����
    /// </summary>
    /// <param name="Ҫ�������¼�����"></param>
    public void EventTrigger<T>(string name,T info)
    {
        if (eventDic.ContainsKey(name))
        {
            if((eventDic[name] as EventInfo<T>).actions != null)
            (eventDic[name] as EventInfo<T>).actions.Invoke(info);
        }
    }

    /// <summary>
    /// ��������Ҫ�������¼�
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo).actions != null)
                (eventDic[name] as EventInfo).actions.Invoke();
        }
    }

    /// <summary>
    /// ����¼�����
    /// ��Ҫ���ڳ����л�ʱ
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    
}
