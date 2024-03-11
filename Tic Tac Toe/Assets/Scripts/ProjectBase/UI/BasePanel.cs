using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ������
/// �ҵ������Լ�����µĿؼ�����
/// Ӧ���ṩ��ʾ �������ص���Ϊ
/// </summary>
public class BasePanel : MonoBehaviour
{
    //ͨ�������滻ԭ�� ���洢���еĿؼ�
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
    /// �õ���Ӧ���ֵĶ�Ӧ�ؼ��ű�
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
    /// �ҵ��Ӷ���Ķ�Ӧ�ؼ�
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
    /// ��ʾ�Լ�
    /// </summary>
    public virtual void ShowMe()
    {
        
    }

    /// <summary>
    /// �����Լ�
    /// </summary>
    public virtual void HideMe()
    {

    }

}
