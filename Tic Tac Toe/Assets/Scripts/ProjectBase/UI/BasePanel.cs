using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// 找到所有自己面板下的控件对象
/// 应该提供显示 或者隐藏的行为
/// </summary>
public class BasePanel : MonoBehaviour
{
    //通过里氏替换原则 来存储所有的控件
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        FindChildControl<Button>();
        FindChildControl<Image>();
        FindChildControl<Toggle>();
        FindChildControl<Text>();
        FindChildControl<ScrollRect>();
        FindChildControl<Slider>();

    }

    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T:UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDic[controlName].Count; ++i)
            {
                if(controlDic[controlName][i] is T)
                {
                    return controlDic[controlName][i] as T;
                }
            }
        }
        return null;
    }

    protected virtual void OnClick(string btnName)
    {

    }

    protected virtual void OnValueChanged(string toggleName,bool isSelect)
    {

    }




    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildControl<T>() where T:UIBehaviour
    {
        T[] controls = this.transform.GetComponentsInChildren<T>();
        
        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].name;
            if (controlDic.ContainsKey(controls[i].name))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(controls[i].name, new List<UIBehaviour>() { controls[i]});
            if(controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(()=> {
                    OnClick(objName);
                });
            }

            if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) => {
                    OnValueChanged(objName,value);
                });
            }

        }
    }

    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe()
    {
        
    }

    /// <summary>
    /// 隐藏自己
    /// </summary>
    public virtual void HideMe()
    {

    }

}
