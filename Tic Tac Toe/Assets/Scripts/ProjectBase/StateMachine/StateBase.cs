using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase 
{
    /// <summary>
    /// 状态持有者
    /// </summary>
    protected IStateOwner owner;
   
    /// <summary>
    /// 对应状态机
    /// </summary>
    protected StateMechineLogic logic;

    /// <summary>
    /// 分配持有者
    /// </summary>
    /// <param name="logic"></param>
    public virtual void Init(StateMechineLogic logic,IStateOwner owner)
    {
        this.logic = logic;
        this.owner = owner;
    }

    /// <summary>
    /// 进入状态逻辑
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// 更新状态逻辑
    /// </summary>
    public abstract void OnUpdate();


    /// <summary>
    /// 离开状态逻辑
    /// </summary>
    public abstract void OnExit();


}
