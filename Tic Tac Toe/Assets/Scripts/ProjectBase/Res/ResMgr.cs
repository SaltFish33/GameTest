using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��Դ����ģ��
/// 1.�첽����
/// 2.ί�к�lambda���ʽ
/// 3.Э��
/// 4.����
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //ͬ��������Դ
    public T Load<T>(string name) where T :Object
    {
        T res = Resources.Load<T>(name);
        //�������ʱһ��GameObject���͵�
        //�Ұ���ʵ�������ٷ��س�ȥ
        //�ⲿֱ��ʹ�ü���
        
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else//TextAsset AudioClip
            return res;
    }

    //�첽������Դ
    public void LoadAsync<T>(string name,UnityAction<T> callBack)where T:Object
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(name,callBack));    
    }


    /// <summary>
    /// ����UniTaskʵ�ֵ��첽����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="path">����·��</param>
    /// <returns></returns>
    public async UniTask<Object> LoadAsyncByUniTask<T>(string path) where T : Object
    {
        var loadOperation = Resources.LoadAsync<T>(path);
        return await loadOperation;

    }



    /// <summary>
    /// ������Э�̺��� �����첽������������Դ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callBack) where T:Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callBack(GameObject.Instantiate(r.asset) as T);
        else
            callBack(r.asset as T);

    }


}
