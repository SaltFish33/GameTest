using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// 知识点
/// 1.场景异步加载
/// 2.协程
/// 3.委托
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
    
    /// <summary>
    /// 切换场景 同步加载
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, UnityAction action)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //加载完成过后 才会去执行
        action();
    }

    /// <summary>
    /// 提供个外部的 异步加载的接口方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void LoadSceneAsyn(string name, UnityAction action)
    {
        //场景异步加载
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name,action));
       
    }


    /// <summary>
    /// 使用UniTask异步加载场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="callBack"></param>
    public async void LoadSceneByUniTask(string name,UnityAction callBack = null)
    {
        await SceneManager.LoadSceneAsync(name).ToUniTask(
            (Progress.Create<float>((p) =>
           {
               Debug.Log("加载场景中");
               EventCenter.Instance.EventTrigger<float>("Loading", p);
           }))
            );
        if(callBack != null)
        {
            callBack();
        }   
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsyn(string name,UnityAction action)
    {

        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //可以得到场景加载的进度
        while (!ao.isDone)
        {
            //希望在这里面去更新进度条
            //事件中心 向外分发 进度情况 外面想用就用
            EventCenter.Instance.EventTrigger("Loading", ao.progress);
            yield return ao.progress;
        }
        //加载完成过后 执行方法
        action();
    }


}
