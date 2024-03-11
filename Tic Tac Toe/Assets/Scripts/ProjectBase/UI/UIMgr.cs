using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI�㼶
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}


/// <summary>
/// UI������
/// 1.����������ʾ�����
/// 2.�ṩ���ⲿ ��ʾ�����صȵȽӿ�
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    //��¼UI��canvas������
    public RectTransform canvas;

    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public UIMgr()
    {
        //ȥ�ҵ������е�Canvas
        GameObject obj = ResMgr.Instance.Load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        //����Canvas �����ڹ�����ʱ�����Ƴ�
        GameObject.DontDestroyOnLoad(obj);

        //�ҵ�����
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");

        //����EventSystem �����ڹ�����ʱ�����Ƴ�
        obj = ResMgr.Instance.Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
    }


    /// <summary>
    /// ��ʾ���
    /// </summary>
    /// <typeparam name="T">���ű�����</typeparam>
    /// <param name="panelName">�����</param>
    /// <param name="layer">���㼶</param>
    /// <param name="callBack">������ɺ��߼�</param>
    public void ShowPanel<T>(string panelName, E_UI_Layer layer,UnityAction<T> callBack = null) where T:BasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            if (callBack != null)
            {
                callBack(panelDic[panelName] as T);
            }
            //��������ظ�����
            return;
        }


        GameObject obj = ResMgr.Instance.Load<GameObject>("UI/" + panelName);
        //GameObject obj =  (GameObject)await AssetBundleHelper.LoadAsset(panelName, "ui", typeof(GameObject));
        //������ΪCanvas���Ӷ���
        //����Ҫ�������λ��

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

        //����Ҫ�������λ��
        obj.transform.SetParent(father);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;


        (obj.transform as RectTransform).offsetMax = Vector2.zero;
        (obj.transform as RectTransform).offsetMin = Vector2.zero;
        //�õ�Ԥ�������ϵ����ű�
        T panel = obj.GetComponent<T>();
        //�������ɺ���߼�
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
    /// �������
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
    /// �õ�ĳһ���Ѿ���ʾ����� �����ⲿʹ��
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
    /// ���ؼ�����Զ����¼�����
    /// </summary>
    /// <param name="control">�ؼ�����</param>
    /// <param name="type">�¼�����</param>
    /// <param name="callBack">�¼�����Ӧ����</param>
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
