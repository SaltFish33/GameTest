using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// 1.异步加载
/// 2.委托和lambda表达式
/// 3.协程
/// 4.泛型
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //同步加载资源
    public T Load<T>(string name) where T :Object
    {
        T res = Resources.Load<T>(name);
        //如果对象时一个GameObject类型的
        //我把它实例化后再返回出去
        //外部直接使用即可
        
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else//TextAsset AudioClip
            return res;
    }

    //异步加载资源
    public void LoadAsync<T>(string name,UnityAction<T> callBack)where T:Object
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(name,callBack));    
    }


    /// <summary>
    /// 利用UniTask实现的异步加载
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="path">对象路径</param>
    /// <returns></returns>
    public async UniTask<Object> LoadAsyncByUniTask<T>(string path) where T : Object
    {
        var loadOperation = Resources.LoadAsync<T>(path);
        return await loadOperation;

    }



    /// <summary>
    /// 真正的协程函数 用于异步加载真正的资源
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
