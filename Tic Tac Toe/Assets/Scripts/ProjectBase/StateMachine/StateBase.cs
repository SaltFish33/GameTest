using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase 
{
    /// <summary>
    /// ״̬������
    /// </summary>
    protected IStateOwner owner;
   
    /// <summary>
    /// ��Ӧ״̬��
    /// </summary>
    protected StateMechineLogic logic;

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="logic"></param>
    public virtual void Init(StateMechineLogic logic,IStateOwner owner)
    {
        this.logic = logic;
        this.owner = owner;
    }

    /// <summary>
    /// ����״̬�߼�
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// ����״̬�߼�
    /// </summary>
    public abstract void OnUpdate();


    /// <summary>
    /// �뿪״̬�߼�
    /// </summary>
    public abstract void OnExit();


}
