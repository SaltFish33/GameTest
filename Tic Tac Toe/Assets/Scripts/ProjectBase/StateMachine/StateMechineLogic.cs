using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ӿ� ��������״̬�ĳ�����
/// </summary>
public interface IStateOwner { };

/// <summary>
/// ͨ��״̬��Logic
/// </summary>
public class StateMechineLogic
{
    
    /// <summary>
    /// �ֵ䣬���ڴ洢���е�״̬
    /// </summary>
    protected Dictionary<int, StateBase> statesDic;

    /// <summary>
    /// ��ǰ����״̬
    /// </summary>
    protected StateBase curState;

    /// <summary>
    /// ��ǰ����״̬������
    /// </summary>
    protected int curStateType = -1;

    /// <summary>
    /// ��״̬���ĳ�����
    /// </summary>
    protected IStateOwner owner;

    /// <summary>
    /// ״̬����ʼ��
    /// </summary>
    /// <param name="owner">״̬��������</param>
    public virtual void Init(IStateOwner owner)
    {
        this.owner = owner;
        statesDic = new Dictionary<int, StateBase>();
        MonoMgr.Instance.AddUpdateListener(OnUpdateState);
    }

    /// <summary>
    /// ���״̬
    /// </summary>
    /// <typeparam name="T">״̬���ͣ�����̳���StateBase</typeparam>
    /// <param name="type">��״̬������</param>
    public virtual StateBase GetStates<T>(int type) where T : StateBase, new()
    {
        if (statesDic.ContainsKey(type)) return statesDic[type];
        T t = new T();
        t.Init(this,owner);
        statesDic.Add(type, t);
        if (curState == null)
        {
            t.OnEnter();
            curState = t;
            curStateType = type;
        }
        return t;
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    /// <param name="targetType">Ҫ���ĵ�״̬����</param>
    /// <param name="reLoad">����������뵱ǰ������ͬ���Ƿ�Ҫִ��</param>
    public virtual void ChangeState<T>(int targetType,bool reLoad = false)where T:StateBase,new()
    {
        if(curStateType == targetType)
        {
            if (reLoad)
            {
                curState.OnExit();
                curState.OnEnter();
            }
        }
        else
        {
            if (curState == null) curState = GetStates<T>(targetType);
            else
            {
                curState.OnExit();
                GetStates<T>(targetType);
                statesDic[targetType].OnEnter();
                curState = GetStates<T>(targetType);
                curStateType = targetType;
            }
            
        }
    }

    /// <summary>
    /// ִ�е�ǰ״̬��Update�߼�
    /// </summary>
    public virtual void OnUpdateState()
    {
        if(curState != null)
        {
            curState.OnUpdate();
        }
    }

    /// <summary>
    /// ȡ��Update�¼�����
    /// </summary>
    public virtual void OnDestory()
    {
        MonoMgr.Instance.RemoveUpdateListener(OnUpdateState);
    }

    


}
