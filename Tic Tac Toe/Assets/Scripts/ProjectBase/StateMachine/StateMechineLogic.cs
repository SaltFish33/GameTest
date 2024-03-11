using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接口 用来承载状态的持有者
/// </summary>
public interface IStateOwner { };

/// <summary>
/// 通用状态机Logic
/// </summary>
public class StateMechineLogic
{
    
    /// <summary>
    /// 字典，用于存储所有的状态
    /// </summary>
    protected Dictionary<int, StateBase> statesDic;

    /// <summary>
    /// 当前所处状态
    /// </summary>
    protected StateBase curState;

    /// <summary>
    /// 当前所处状态的索引
    /// </summary>
    protected int curStateType = -1;

    /// <summary>
    /// 该状态机的持有者
    /// </summary>
    protected IStateOwner owner;

    /// <summary>
    /// 状态机初始化
    /// </summary>
    /// <param name="owner">状态机持有者</param>
    public virtual void Init(IStateOwner owner)
    {
        this.owner = owner;
        statesDic = new Dictionary<int, StateBase>();
        MonoMgr.Instance.AddUpdateListener(OnUpdateState);
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <typeparam name="T">状态类型，必须继承至StateBase</typeparam>
    /// <param name="type">该状态的索引</param>
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
    /// 更改状态
    /// </summary>
    /// <param name="targetType">要更改的状态索引</param>
    /// <param name="reLoad">如果该索引与当前索引相同，是否要执行</param>
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
    /// 执行当前状态的Update逻辑
    /// </summary>
    public virtual void OnUpdateState()
    {
        if(curState != null)
        {
            curState.OnUpdate();
        }
    }

    /// <summary>
    /// 取消Update事件监听
    /// </summary>
    public virtual void OnDestory()
    {
        MonoMgr.Instance.RemoveUpdateListener(OnUpdateState);
    }

    


}
