using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}


/// <summary>
/// UI管理器
/// 1.管理所有显示的面板
/// 2.提供给外部 显示和隐藏等等接口
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    //记录UI的canvas父对象
    public RectTransform canvas;

    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public UIMgr()
    {
        //去找到场景中的Canvas
        GameObject obj = ResMgr.Instance.Load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        //创建Canvas 让其在过场景时不被移除
        GameObject.DontDestroyOnLoad(obj);

        //找到各层
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");

        //创建EventSystem 让其在过场景时不被移除
        obj = ResMgr.Instance.Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
    }


    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">面板层级</param>
    /// <param name="callBack">面板生成后逻辑</param>
    public void ShowPanel<T>(string panelName, E_UI_Layer layer,UnityAction<T> callBack = null) where T:BasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            if (callBack != null)
            {
                callBack(panelDic[panelName] as T);
            }
            //避免面板重复加载
            return;
        }


        GameObject obj = ResMgr.Instance.Load<GameObject>("UI/" + panelName);
        //GameObject obj =  (GameObject)await AssetBundleHelper.LoadAsset(panelName, "ui", typeof(GameObject));
        //把它作为Canvas的子对象
        //并且要设置相对位置

        Transform father = bot;
        switch (layer)
        {
            case E_UI_Layer.Mid:
                father = mid;
                break;
            case E_UI_Layer.Top:
                father = top;
                break;
            case E_UI_Layer.System:
                father = system;
                break;
        }

        //并且要设置相对位置
        obj.transform.SetParent(father);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;


        (obj.transform as RectTransform).offsetMax = Vector2.zero;
        (obj.transform as RectTransform).offsetMin = Vector2.zero;
        //得到预设体身上的面板脚本
        T panel = obj.GetComponent<T>();
        //处理生成后的逻辑
        if (callBack != null)
            callBack(panel);

        panel.ShowMe();

        panelDic.Add(panelName, panel);

    }


    public Transform GetLayerFather(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return bot;
                
            case E_UI_Layer.Mid:
                return mid;
               
            case E_UI_Layer.Top:
                return top;
                
            case E_UI_Layer.System:
                return system;
                
            
        }
        return null;
    }


    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HideMe();
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }


    public void HidePanelBySelf(string panelName, UnityAction hideAction)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HideMe();
            hideAction?.Invoke();
            panelDic.Remove(panelName);
        }
    }

    /// <summary>
    /// 得到某一个已经显示的面板 方便外部使用
    /// </summary>
    public T GetPanel<T>(string name) where T:BasePanel
    {
        if (panelDic.ContainsKey(name))
        {
            return panelDic[name] as T;
        }
        else
        {
            return null;
        }
    }



    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if(trigger == null)
        {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);
        trigger.triggers.Add(entry);
    }

}
